using System.Collections.Generic;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using UnityEngine;
using GameFramework.Manager;
using GameFramework.Utility;
using System;
using System.Linq;
using Hunting.Game.Animal;

/// <summary>
/// 动物管理器
/// </summary>
public class AnimalManager : IRoundManager, IRoundResettable, IRoundUpdatable
{
    /// <summary>
    /// 活跃动物集合
    /// </summary>
    private readonly HashSet<BaseAnimalBehaviour> _activeAnimals = new HashSet<BaseAnimalBehaviour>();

    /// <summary>
    /// 待移除动物列表
    /// </summary>
    private readonly List<BaseAnimalBehaviour> _pendingRemovalAnimals = new List<BaseAnimalBehaviour>();

    /// <summary>
    /// 动物配置缓存
    /// </summary>
    private readonly Dictionary<int, Specie> _animalDatas = new Dictionary<int, Specie>();
    public  Dictionary<int, Specie> AnimalDatas => _animalDatas;

    /// <summary>
    /// 动物预制体缓存
    /// </summary>
    private readonly Dictionary<int, GameObject> _animalPrefabs = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> AnimalPrefabs => _animalPrefabs;

    /// <summary>
    /// 动物击杀数量
    /// </summary>
    private readonly Dictionary<int, int> _huntingCounts = new Dictionary<int, int>();

    /// <summary>
    /// 未进入死亡状态动物集合
    /// </summary>
    private readonly HashSet<BaseAnimalBehaviour> _unDeathAnimals = new HashSet<BaseAnimalBehaviour>();

    private EventManager _eventManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private HuntingConfigManager _configManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        RegisterEvents();
        CacheAnimalDatas(context.MapData.ID);
        Log.Info("[AnimalManager] 初始化完成");
    }

    public void DoUpdate(float dt)
    {
        foreach (var animal in _activeAnimals)
            animal.DoUpdate(dt);

        ProcessPendingRemovals();
    }

    public void Dispose()
    {
        RecycleAllAnimals();
        UnregisterEvents();
        Log.Info("[AnimalManager] 已释放");
    }

    public void Cleanup()
    {
        RecycleAllAnimals();
        ClearAnimalCache();
    }

    public void ReInit(Map mapData) 
    {
        CacheAnimalDatas(mapData.ID);
    }

    #region 公共方法
    /// <summary>
    /// 获取最近且在屏幕内的动物
    /// </summary>
    /// <param name="fromPosition">参考位置</param>
    /// <param name="camera">相机</param>
    /// <returns>最近的可见动物</returns>
    public BaseAnimalBehaviour GetNearestVisibleAnimal(Vector3 fromPosition)
    {
        BaseAnimalBehaviour nearestAnimal = null;
        float nearestSq = float.MaxValue;

        foreach (var animal in _activeAnimals)
        {
            if (_pendingRemovalAnimals.Contains(animal))
                continue;
            if (animal.Health.IsDead)
                continue;
            if (!ScreenUtils.IsVisible(animal.transform, Camera.main))
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
    /// 获取动物肉量
    /// </summary>
    public int GetMeatAmount(int specieId, int count = 1)
    {
        return _animalDatas[specieId].DropRewards[EDropType.Meat] * count;
    }

    /// <summary>
    /// 获取未进入死亡状态的动物数量
    /// </summary>
    /// <returns></returns>
    public int GetUnDeathAnimalCount() => _unDeathAnimals.Count;
    
    /// <summary>
    /// 获取活跃动物数量
    /// </summary>
    /// <returns></returns>
    public int GetActiveAnimalCount() => _activeAnimals.Count;

    
    /// <summary>
    /// 获取最后一只活跃动物位置
    /// </summary>
    public Vector3 GetLastActiveAnimalPosition()
    {
        BaseAnimalBehaviour lastAnimal = null;
        foreach (var animal in _activeAnimals)
            lastAnimal = animal;

        return lastAnimal.transform.position;
    }

    public List<BaseAnimalBehaviour> GetCloseAnimalsFromTargetPosition(Vector3 position, int animalCount)
    {
        if (animalCount <= 0) return null;

        var validAnimals = _activeAnimals.Except(_pendingRemovalAnimals).ToList();
        if (validAnimals.Count <= 0) return null;

        animalCount = Math.Min(validAnimals.Count, animalCount);
        return validAnimals
            .OrderBy(animal => Vector3.SqrMagnitude(animal.transform.position - position))
            .Take(animalCount)
            .ToList();
    }
    #endregion

    #region 私有方法
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _configManager = GameServiceLocator.ConfigManager;
    }

    /// <summary>
    /// 缓存动物配置
    /// </summary>
    /// <param name="mapId">地图ID</param>
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
    /// 处理待移除的动物
    /// </summary>
    private void ProcessPendingRemovals()
    {
        foreach (var animal in _pendingRemovalAnimals)
        {
            _activeAnimals.Remove(animal);
            
            _unDeathAnimals.Remove(animal);
            _gameObjectPoolManager.Despawn(animal.gameObject);
        }
        _pendingRemovalAnimals.Clear();
    }

    /// <summary>
    /// 回收所有动物
    /// </summary>
    private void RecycleAllAnimals()
    {
        foreach (var animal in _activeAnimals)
            _gameObjectPoolManager.Despawn(animal.gameObject);

        _activeAnimals.Clear();
        _pendingRemovalAnimals.Clear();

        _unDeathAnimals.Clear();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
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
    private void UnregisterEvents()
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
        _activeAnimals.Add(args.Animal);
        _unDeathAnimals.Add(args.Animal);
    }

    /// <summary>
    /// 动物进入死亡状态事件回调
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        _unDeathAnimals.Remove(args.Animal);

        int id = args.SpecieData.ID;
        _huntingCounts[id] = _huntingCounts.GetValueOrDefault(id) + 1;
    }

    /// <summary>
    /// 动物死亡事件回调
    /// </summary>
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        _pendingRemovalAnimals.Add(args.Animal);
    }

    /// <summary>
    /// 动物逃跑事件回调
    /// </summary>
    private void OnAnimalLeft(AnimalLeftEventArgs args)
    {

        _pendingRemovalAnimals.Add(args.Animal);
    }

    /// <summary>
    /// Boss死亡事件回调
    /// </summary>
    private void OnBossDied(BossDiedEventArgs args)
    {
        _pendingRemovalAnimals.Add(args.Boss);
    }
    #endregion
}
