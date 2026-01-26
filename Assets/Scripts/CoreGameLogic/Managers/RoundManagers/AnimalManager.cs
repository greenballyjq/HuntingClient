using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using UnityEngine;
using GameFramework.Manager;
using System;
using System.Linq;
using Hunting.Game.Animal;
using Object = UnityEngine.Object;

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
        // _animalsToRemove.Clear();
        
        foreach (var animal in _activeAnimals)
            animal.DoUpdate(dt);
        
        foreach (var animal in _animalsToRemove)
            RemoveAndRecycleAnimal(animal);

        _animalsToRemove.Clear();
    }

    #region 公共方法
    
    /// <summary>
    /// 是否还有活跃的动物
    /// </summary>
    /// <returns></returns>
    public bool HasActiveAnimal() => GetActiveAnimalCount() > 0;
    
    /// <summary>
    /// 获取活跃动物数量
    /// </summary>
    /// <returns></returns>
    public int GetActiveAnimalCount() => _activeAnimals.Count;
    
    /// <summary>
    /// 获取真实活跃动物数量
    /// </summary>
    /// <returns></returns>
    public int GetRealActiveAnimalCount() => _activeAnimals.Count - _animalsToRemove.Count;

    public List<BaseAnimalBehaviour> GetCloseAnimalsFromTargetPosition(Vector3 position, int animalCount)
    {
        if (animalCount <= 0) return null;
        
        var validAnimals = _activeAnimals.Except(_animalsToRemove).ToList();
        if (validAnimals.Count <= 0) return null;

        animalCount = Math.Min(validAnimals.Count, animalCount);
        return validAnimals
            .OrderBy(animal => Vector3.SqrMagnitude(animal.transform.position - position))
            .Take(animalCount)
            .ToList();
    }

    /// <summary>
    /// 获取BossAnimalBehaviour，没有则返回空
    /// </summary>
    /// <returns></returns>
    public BossAnimalBehaviour GetBossAnimalBehaviour()
    {
        BossAnimalBehaviour result = null;
        foreach (var animal in _activeAnimals)
        {
            if (animal is BossAnimalBehaviour bossAnimalBehaviour)
            {
                result = bossAnimalBehaviour;
                break;
            }
        }
        return result;
    }
    
    #endregion

    #region 私有方法
    /// <summary>
    /// 回收所有动物
    /// </summary>
    private void RecycleAllAnimals()
    {
        foreach (var animal in _activeAnimals)
            _gameObjectPoolManager.Despawn(animal.gameObject);
                
        _activeAnimals.Clear();
        _animalsToRemove.Clear();
    }

    private void RemoveAndRecycleAnimal(BaseAnimalBehaviour animal)
    {
        var remove = _activeAnimals.Remove(animal);
        if (!remove) return;
        _eventManager.Trigger(AnimalEvents.AnimalRemoved, new AnimalRemovedEventArgs { Animal = animal});
        _gameObjectPoolManager.Despawn(animal.gameObject);
    }
    
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.AddListener(BossEvents.BossDied, OnBossDied);
        _eventManager.AddListener(AnimalEvents.AnimalFled, OnAnimalFled);
        _eventManager.AddListener(AnimalEvents.AnimalReachedWall, OnAnimalReachedWall);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.RemoveListener(BossEvents.BossDied, OnBossDied);
        _eventManager.RemoveListener(AnimalEvents.AnimalFled, OnAnimalFled);
        _eventManager.RemoveListener(AnimalEvents.AnimalReachedWall, OnAnimalReachedWall);
    }

    /// <summary>
    /// 动物生成事件回调
    /// </summary>
    private void OnAnimalGenerated(AnimalGeneratedEventArgs args)
    {
        _activeAnimals.Add(args.Animal);
    }

    /// <summary>
    /// 动物死亡事件回调
    /// </summary>
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        _animalsToRemove.Add(args.Animal);
    }
    
    /// <summary>
    /// Boss死亡事件回调
    /// </summary>
    /// <param name="args"></param>
    private void OnBossDied(BossDiedEventArgs args)
    {
        _animalsToRemove.Add(args.Boss);
    }

    /// <summary>
    /// 动物逃跑事件回调
    /// </summary>
    private void OnAnimalFled(AnimalFledEventArgs args)
    {
        _animalsToRemove.Add(args.Animal);
    }
    
    /// <summary>
    /// 动物到达边界事件回调
    /// </summary>
    private void OnAnimalReachedWall(AnimalReachedWallEventArgs args)
    {
        _animalsToRemove.Add(args.Animal);
    }
    #endregion
}
