using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using UnityEngine;
using GameFramework.Manager;

/// <summary>
/// 动物管理器
/// </summary>
public class AnimalManager : IRoundManager
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
    /// 追踪所有活跃的动物
    /// </summary>
    private readonly HashSet<AnimalBehavior> _activeAnimals = new HashSet<AnimalBehavior>();

    /// <summary>
    /// 初始化单局管理器
    /// </summary>
    /// <param name="context">单局上下文</param>
    public void Init(RoundContext context)
    {
        RegisterEvents();
        Debug.Log("[AnimalManager] 初始化完成");
    }

    /// <summary>
    /// 释放单局管理器
    /// </summary>
    public void Dispose()
    {
        // 回收所有活跃的动物
        foreach (var animal in _activeAnimals)
            if (animal != null && animal.gameObject != null)
                _gameObjectPoolManager.Despawn(animal.gameObject);
                
        _activeAnimals.Clear();
        
        UnregisterEvents();
        Debug.Log("[AnimalManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 生成动物
    /// </summary>
    public async UniTask<AnimalBehavior> SpawnAnimalAsync(Specie specieData, Vector3 position, Vector3 direction, float stayTime, Spawner spawner = null)
    {
        var go = await _gameObjectPoolManager.SpawnAsync(specieData.PrefabResourcePath);
        if (go == null)
        {
            Debug.LogError($"[AnimalManager] 从对象池获取失败: {specieData.PrefabResourcePath}");
            return null;
        }

        var animal = go.GetComponent<AnimalBehavior>();
        if (animal == null)
        {
            Debug.LogError("[AnimalManager] 预制体缺少 AnimalBehavior 组件");
            _gameObjectPoolManager.Despawn(go);
            return null;
        }

        go.transform.position = position;
        if (direction != Vector3.zero)
            go.transform.rotation = Quaternion.LookRotation(direction);

        animal.Init(specieData, stayTime);

        // 添加到活跃动物集合
        _activeAnimals.Add(animal);

        TriggerAnimalSpawned(new AnimalSpawnedEventArgs
        {
            Sender = this,
            Spawner = spawner,
            Animal = animal,
            SpecieData = specieData,
            Position = position,
            Direction = direction,
            StayTime = stayTime
        });

        return animal;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.AddListener(AnimalEvents.AnimalFled, OnAnimalFled);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.RemoveListener(AnimalEvents.AnimalFled, OnAnimalFled);
    }

    /// <summary>
    /// 物种派发回调
    /// </summary>
    private async void OnSpeciesSpawned(SpeciesSpawnEventArgs args)
    {
        await SpawnAnimalAsync(args.SpecieData, args.Position, args.Direction, args.StayTime, args.Spawner);
    }

    /// <summary>
    /// 动物死亡事件回调
    /// </summary>
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        // 从活跃集合中移除
        _activeAnimals.Remove(args.Animal);
        _gameObjectPoolManager.Despawn(args.Animal.gameObject);
    }

    /// <summary>
    /// 动物逃跑事件回调
    /// </summary>
    private void OnAnimalFled(AnimalFledEventArgs args)
    {
        // 从活跃集合中移除
        _activeAnimals.Remove(args.Animal);
        _gameObjectPoolManager.Despawn(args.Animal.gameObject);
    }

    /// <summary>
    /// 触发动物生成完成事件
    /// </summary>
    private void TriggerAnimalSpawned(AnimalSpawnedEventArgs args)
    {
        _eventManager.Trigger(AnimalEvents.AnimalSpawned, args);
    }
    #endregion

    #region 测试
    /// <summary>
    /// 获取当前活跃动物数量
    /// </summary>
    public int GetActiveAnimalCount()
    {
        return _activeAnimals.Count;
    }
    #endregion
}
