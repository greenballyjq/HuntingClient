using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using UnityEngine;
using GameFramework.Manager;
using System;
using System.Linq;
using Hunting.Game.Animal;

/// <summary>
/// 动物管理器
/// </summary>
public class AnimalManager : IRoundManager, IRoundResettable, IRoundUpdatable
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 对象池管理器
    /// </summary>
    private GameObjectPoolManager _gameObjectPoolManager => GameServiceLocator.GameObjectPoolManager;

    /// <summary>
    /// 资源加载管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    /// <summary>
    /// 追踪所有活跃的动物
    /// </summary>
    private readonly HashSet<BaseAnimalBehaviour> _activeAnimals = new HashSet<BaseAnimalBehaviour>();
    
    /// <summary>
    /// 待移除的动物列表
    /// </summary>
    private readonly List<BaseAnimalBehaviour> _animalsToRemove = new List<BaseAnimalBehaviour>();

    public void Init(RoundContext context)
    {
        RegisterEvents();
        Debug.Log("[AnimalManager] 初始化完成");
    }

    public void Dispose()
    {
        RecycleAllAnimals();
        UnregisterEvents();
        Debug.Log("[AnimalManager] 已释放");
    }

    public void Cleanup()
    {
        RecycleAllAnimals();
    }

    public void ReInit(RoundContext context){}

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void DoUpdate(float dt)
    {
        _animalsToRemove.Clear();
        
        foreach (var animal in _activeAnimals)
            animal.DoUpdate(dt);
        
        foreach (var animal in _animalsToRemove)
            _activeAnimals.Remove(animal);

        _animalsToRemove.Clear();
    }

    #region 公共方法
    /// <summary>
    /// 生成动物
    /// </summary>
    public async UniTask<BaseAnimalBehaviour> GenerateAnimalAsync(Specie specieData, Vector3 position, Vector3 direction, bool inHiddenMap = false)
    {
        var go = await _gameObjectPoolManager.SpawnAsync(specieData.PrefabResourcePath);
        var animal = go.GetComponent<BaseAnimalBehaviour>();
        go.transform.position = position;

        animal.Init(specieData);
        animal.GetComponent<IMoveable>().SetDirection(direction);

        // 添加到活跃动物集合
        _activeAnimals.Add(animal);
        return animal;
    }

    /// <summary>
    /// 生成Boss
    /// </summary>
    /// <returns>Boss实例</returns>
    //public async UniTask<BossBehaviour> GenerateBossAsync()
    //{
    //    var configManager = GameServiceLocator.ConfigManager;
    //    var bossSpecie = configManager.GetRandomBoss();
    //    var spawnPoint = GameObject.Find("BossSpawnerPoint");
    //    var prefab = await _resourceManager.LoadAssetAsync<GameObject>(bossSpecie.PrefabResourcePath);
    //    var go = Object.Instantiate(prefab, spawnPoint.transform);
    //    var boss = go.GetComponent<BossBehaviour>();
    //    go.transform.position = spawnPoint.transform.position;
    //    boss.Init(bossSpecie);
    //    return boss;
    //}
    
    /// <summary>
    /// 是否还有活跃的动物
    /// </summary>
    /// <returns></returns>
    public bool HasActiveAnimal() => _activeAnimals.Count > 0;
    
    /// <summary>
    /// 获取活跃动物数量
    /// </summary>
    /// <returns></returns>
    public int GetActiveAnimalCount() => _activeAnimals.Count;

    public List<BaseAnimalBehaviour> GetCloseAnimalsFromTargetPosition(Vector3 position, int animalCount)
    {
        if (animalCount <= 0) return null;
        if (_activeAnimals.Count <= 0) return null;

        animalCount = Math.Min(_activeAnimals.Count, animalCount);
        return _activeAnimals
            .OrderBy(animal => Vector3.SqrMagnitude(animal.transform.position - position))
            .Take(animalCount)
            .ToList();
    }
    
    #endregion

    #region 私有方法
    /// <summary>
    /// 回收所有动物
    /// </summary>
    private void RecycleAllAnimals()
    {
        foreach (var animal in _activeAnimals)
            if (animal != null && animal.gameObject != null)
                _gameObjectPoolManager.Despawn(animal.gameObject);
                
        _activeAnimals.Clear();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.AddListener(AnimalEvents.AnimalFled, OnAnimalFled);
        _eventManager.AddListener(AnimalEvents.AnimalReachedWall, OnAnimalReachedWall);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.RemoveListener(AnimalEvents.AnimalFled, OnAnimalFled);
        _eventManager.RemoveListener(AnimalEvents.AnimalReachedWall, OnAnimalReachedWall);
    }

    /// <summary>
    /// 动物死亡事件回调
    /// </summary>
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        _animalsToRemove.Add(args.Animal);
        _gameObjectPoolManager.Despawn(args.Animal.gameObject);
    }

    /// <summary>
    /// 动物逃跑事件回调
    /// </summary>
    private void OnAnimalFled(AnimalFledEventArgs args)
    {
        _animalsToRemove.Add(args.Animal);
        _gameObjectPoolManager.Despawn(args.Animal.gameObject);
    }
    
    /// <summary>
    /// 动物到达边界事件回调
    /// </summary>
    private void OnAnimalReachedWall(AnimalReachedWallEventArgs args)
    {
        _animalsToRemove.Add(args.Animal);
        _gameObjectPoolManager.Despawn(args.Animal.gameObject);
    }
    #endregion
}
