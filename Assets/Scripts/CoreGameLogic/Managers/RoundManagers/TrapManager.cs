using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Pool;
using GameFramework.Manager;
using GameFramework.Utility;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 陷阱管理器
/// </summary>
public class TrapManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 活跃陷阱集合
    /// </summary>
    private readonly HashSet<TrapBehaviour> _activeTraps = new HashSet<TrapBehaviour>();

    /// <summary>
    /// 待移除陷阱列表
    /// </summary>
    private readonly List<TrapBehaviour> _pendingRemovalTraps = new List<TrapBehaviour>();

    private EventManager _eventManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    public void Init(RoundContext context)
    {
        RegisterServices();
        RegisterEvents();
        Log.Info("[TrapManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();
        RecycleAllTraps();
        Log.Info("[TrapManager] 已释放");
    }
    
    public void Cleanup()
    {
        RecycleAllTraps();
    }
    
    public void ReInit(RoundContext context){}
    
    public void DoUpdate(float dt)
    {
        foreach (var trap in _activeTraps)
            trap.DoUpdate(dt);

        ProcessPendingRemovals();
    }

    #region 公共方法
    /// <summary>
    /// 创建陷阱
    /// </summary>
    /// <param name="position">陷阱位置</param>
    /// <param name="attractRadius">吸引半径</param>
    /// <param name="triggerRadius">触发半径</param>
    /// <param name="attractRadiusRangeByVolume">按体型划分的吸引半径范围比例</param>
    /// <param name="prefabPath">预制体资源路径</param>
    /// <returns>陷阱游戏对象</returns>
    public async UniTask<GameObject> CreateTrapAsync(
        Vector3 position,
        float attractRadius,
        float triggerRadius,
        Dictionary<ESpecieType, float[]> attractRadiusRangeByVolume,
        string prefabPath
    )
    {
        // 从对象池获取
        GameObject trap = await _gameObjectPoolManager.SpawnAsync(prefabPath);

        // 初始化陷阱
        trap.transform.position = position;
        trap.transform.rotation = Quaternion.identity;

        TrapBehaviour trapBehavior = trap.GetComponent<TrapBehaviour>();
        trapBehavior.Init(attractRadius, triggerRadius, attractRadiusRangeByVolume);

        // 注册到活跃集合
        _activeTraps.Add(trapBehavior);

        return trap;
    }

    /// <summary>
    /// 获取所有陷阱位置
    /// </summary>
    /// <returns>陷阱位置列表</returns>
    public List<Vector3> GetAllTrapPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var trap in _activeTraps)
            positions.Add(trap.transform.position);

        return positions;
    }
    #endregion

    #region 私有方法
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
    }

    /// <summary>
    /// 回收所有陷阱
    /// </summary>
    private void RecycleAllTraps()
    {
        foreach (var trap in _activeTraps)
            _gameObjectPoolManager.Despawn(trap.gameObject);

        _activeTraps.Clear();
        _pendingRemovalTraps.Clear();
    }

    /// <summary>
    /// 处理待移除的陷阱
    /// </summary>
    private void ProcessPendingRemovals()
    {
        foreach (var trap in _pendingRemovalTraps)
        {
            _activeTraps.Remove(trap);
            _gameObjectPoolManager.Despawn(trap.gameObject);
        }

        _pendingRemovalTraps.Clear();
    }

    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(PropEvents.TrapTriggered, OnTrapTriggered);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(PropEvents.TrapTriggered, OnTrapTriggered);
    }

    /// <summary>
    /// 陷阱触发事件回调
    /// </summary>
    private void OnTrapTriggered(TrapTriggeredEventArgs args)
    {
        _pendingRemovalTraps.Add(args.Trap);
    }
    #endregion
}

