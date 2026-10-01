using System.Collections.Generic;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using UnityEngine;
using GameFramework.Manager;
using GameFramework.Utility;
using System;
using System.Linq;
using Hunting.Game.Animal;
using Cysharp.Threading.Tasks;

/// <summary>
/// 动物管理器
/// </summary>
public class AnimalManager : IMapWorld, IRoundUpdatable
{
    /// <summary>
    /// 场上实体，含尸体
    /// </summary>
    private readonly List<BaseAnimalBehaviour> _onField = new List<BaseAnimalBehaviour>();

    /// <summary>
    /// 本帧待回收
    /// </summary>
    private readonly HashSet<BaseAnimalBehaviour> _toDespawn = new HashSet<BaseAnimalBehaviour>();

    /// <summary>
    /// 动物配置缓存
    /// </summary>
    private readonly Dictionary<int, Specie> _animalDatas = new Dictionary<int, Specie>();
    public Dictionary<int, Specie> AnimalDatas => _animalDatas;

    /// <summary>
    /// 动物预制体缓存
    /// </summary>
    private readonly Dictionary<int, GameObject> _animalPrefabs = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> AnimalPrefabs => _animalPrefabs;

    /// <summary>
    /// 动物击杀数量
    /// </summary>
    private readonly Dictionary<int, int> _huntingCounts = new Dictionary<int, int>();

    private EventManager _eventManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private HuntingConfigManager _configManager;
    private CameraManager _cameraManager;

    public UniTask InitAsync(RoundContext context)
    {
        BindServices();
        SubscribeEvents();
        CacheAnimalDatas(context.MapData.ID);
        Log.Info("[AnimalManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void DoUpdate(float dt)
    {
        for (int i = 0; i < _onField.Count; i++)
            _onField[i].DoUpdate(dt);

        ProcessDespawns();
    }

    public void Dispose()
    {
        RecycleAllAnimals();
        UnsubscribeEvents();
        Log.Info("[AnimalManager] 已释放");
    }

    public void Unbind()
    {
        RecycleAllAnimals();
        ClearAnimalCache();
    }

    public void Bind(Map mapData)
    {
        CacheAnimalDatas(mapData.ID);
    }

    #region 公共方法
    /// <summary>
    /// 获取最近且在屏幕内的活体动物
    /// </summary>
    public BaseAnimalBehaviour GetNearestVisibleAnimal(Vector3 fromPosition)
    {
        BaseAnimalBehaviour nearestAnimal = null;
        float nearestSq = float.MaxValue;

        for (int i = 0; i < _onField.Count; i++)
        {
            var animal = _onField[i];
            if (!IsAlive(animal))
                continue;
            if (!ScreenUtils.IsVisible(animal.transform, _cameraManager.MainCamera))
                continue;

            float sq = Vector3.SqrMagnitude(animal.transform.position - fromPosition);
            if (sq < nearestSq)
            {
                nearestSq = sq;
                nearestAnimal = animal;
            }
        }

        return nearestAnimal;
    }

    /// <summary>
    /// 获取动物狩猎数量
    /// </summary>
    public int GetHuntingCount(int specieId)
    {
        return _huntingCounts.GetValueOrDefault(specieId, 0);
    }

    /// <summary>
    /// 获取狩猎统计
    /// </summary>
    public void GetHuntStatistics(List<(int specieId, int count)> results, ESpecieType[] specieTypes)
    {
        results.Clear();
        for (int t = 0; t < specieTypes.Length; t++)
        {
            ESpecieType want = specieTypes[t];
            foreach (Specie s in _animalDatas.Values.Where(x => x.SpecieType == want).OrderBy(x => x.ID))
                results.Add((s.ID, _huntingCounts.GetValueOrDefault(s.ID, 0)));
        }
    }

    /// <summary>
    /// 获取动物肉量
    /// </summary>
    public int GetMeatAmount(int specieId)
    {
        return _animalDatas[specieId].DropRewards[EDropType.Meat];
    }

    /// <summary>
    /// 活体数量
    /// </summary>
    public int GetAliveAnimalCount()
    {
        int count = 0;
        for (int i = 0; i < _onField.Count; i++)
        {
            if (IsAlive(_onField[i]))
                count++;
        }
        return count;
    }

    /// <summary>
    /// 场上是否还有实体
    /// </summary>
    public bool HasRemainingOnField(BaseAnimalBehaviour excluding)
    {
        for (int i = 0; i < _onField.Count; i++)
        {
            var animal = _onField[i];
            if (animal == excluding)
                continue;
            if (_toDespawn.Contains(animal))
                continue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 击杀除指定外的全部活体
    /// </summary>
    public void KillAllAliveExcept(BaseAnimalBehaviour except)
    {
        for (int i = 0; i < _onField.Count; i++)
        {
            var animal = _onField[i];
            if (animal == except)
                continue;
            if (!IsAlive(animal))
                continue;
            animal.Health.Kill();
        }
    }

    /// <summary>
    /// 获取靠近目标位置的活体动物
    /// </summary>
    public List<BaseAnimalBehaviour> GetCloseAnimalsFromTargetPosition(Vector3 position, int animalCount)
    {
        if (animalCount <= 0)
            return null;

        var validAnimals = new List<BaseAnimalBehaviour>();
        for (int i = 0; i < _onField.Count; i++)
        {
            if (IsAlive(_onField[i]))
                validAnimals.Add(_onField[i]);
        }

        if (validAnimals.Count <= 0)
            return null;

        animalCount = Math.Min(validAnimals.Count, animalCount);
        validAnimals.Sort((a, b) =>
            Vector3.SqrMagnitude(a.transform.position - position)
                .CompareTo(Vector3.SqrMagnitude(b.transform.position - position)));
        return validAnimals.GetRange(0, animalCount);
    }
    #endregion

    #region 私有方法
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _configManager = GameServiceLocator.ConfigManager;
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();
    }

    private bool IsAlive(BaseAnimalBehaviour animal)
    {
        return !animal.Health.IsDead && !_toDespawn.Contains(animal);
    }

    /// <summary>
    /// 缓存动物配置
    /// </summary>
    private void CacheAnimalDatas(int mapId)
    {
        ClearAnimalCache();

        var mapSpecies = _configManager.GetMapSpecies(mapId);
        foreach (var specieIds in mapSpecies.Values)
        {
            foreach (var specieId in specieIds)
            {
                _animalDatas[specieId] = _configManager.GetSpecie(specieId);
                _animalPrefabs[specieId] = _configManager._AnimalRefSo.GetAnimalPrefab(specieId);
            }
        }
    }

    /// <summary>
    /// 清理动物配置缓存
    /// </summary>
    private void ClearAnimalCache()
    {
        _animalDatas.Clear();
        _animalPrefabs.Clear();
    }

    /// <summary>
    /// 回收本帧待移除动物
    /// </summary>
    private void ProcessDespawns()
    {
        if (_toDespawn.Count == 0)
            return;

        foreach (var animal in _toDespawn)
        {
            _onField.Remove(animal);
            _gameObjectPoolManager.Despawn(animal.gameObject);
        }
        _toDespawn.Clear();
    }

    /// <summary>
    /// 回收所有动物
    /// </summary>
    private void RecycleAllAnimals()
    {
        for (int i = 0; i < _onField.Count; i++)
            _gameObjectPoolManager.Despawn(_onField[i].gameObject);

        _onField.Clear();
        _toDespawn.Clear();
    }

    private void MarkForDespawn(BaseAnimalBehaviour animal)
    {
        _toDespawn.Add(animal);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void SubscribeEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.AddListener(AnimalEvents.AnimalLeft, OnAnimalLeft);
        _eventManager.AddListener(BossEvents.BossDied, OnBossDied);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnsubscribeEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.RemoveListener(AnimalEvents.AnimalLeft, OnAnimalLeft);
        _eventManager.RemoveListener(BossEvents.BossDied, OnBossDied);
    }

    /// <summary>
    /// 动物生成事件回调
    /// </summary>
    private void OnAnimalGenerated(AnimalGeneratedEventArgs args)
    {
        _onField.Add(args.Animal);
    }

    /// <summary>
    /// 动物进入死亡状态事件回调
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        int id = args.SpecieData.ID;
        _huntingCounts[id] = _huntingCounts.GetValueOrDefault(id) + 1;
    }

    /// <summary>
    /// 动物死亡事件回调
    /// </summary>
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        MarkForDespawn(args.Animal);
    }

    /// <summary>
    /// 动物逃跑事件回调
    /// </summary>
    private void OnAnimalLeft(AnimalLeftEventArgs args)
    {
        MarkForDespawn(args.Animal);
    }

    /// <summary>
    /// Boss死亡事件回调
    /// </summary>
    private void OnBossDied(BossDiedEventArgs args)
    {
        MarkForDespawn(args.Boss);
    }
    #endregion
}
