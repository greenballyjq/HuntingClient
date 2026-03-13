using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
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

    /// <summary>
    /// 陷阱道具参数
    /// </summary>
    private PropTrap _trapParam;

    /// <summary>
    /// 陷阱预制体
    /// </summary>
    private GameObject _trapPrefab;

    private EventManager _eventManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private HuntingConfigManager _configManager;

    public void Init(RoundContext context)
    {
        RegisterServices();

        var propData = _configManager.GetProp(EPropType.Trap);
        _trapParam = _configManager.GetPropTrap(propData.ParamTableID);
        _trapPrefab = _configManager.PropRefSo.GetPropPrefab(propData.ID);

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

    public void ReInit(RoundContext context) { }

    public void DoUpdate(float dt)
    {
        ProcessPendingRemovals();
    }

    #region 公共方法
    /// <summary>
    /// 生成陷阱
    /// </summary>
    /// <param name="position">陷阱位置</param>
    public GameObject SpawnTrap(Vector3 position)
    {
        GameObject trap = _gameObjectPoolManager.Spawn(_trapPrefab);
        trap.transform.position = position;
        trap.transform.rotation = Quaternion.identity;

        var trapBehaviour = trap.GetComponent<TrapBehaviour>();
        trapBehaviour.Init(_trapParam.AttractRadius, _trapParam.TriggerRadius, _trapParam.AttractRadiusRangeByVolume);

        _activeTraps.Add(trapBehaviour);

        return trap;
    }

    /// <summary>
    /// 获取所有陷阱位置
    /// </summary>
    public void GetTrapPositions(List<Vector3> positions)
    {
        positions.Clear();
        foreach (var trap in _activeTraps)
            positions.Add(trap.transform.position);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _configManager = GameServiceLocator.ConfigManager;
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
        _eventManager.AddListener(PropEvents.TrapDestroyed, OnTrapDestoryed);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(PropEvents.TrapDestroyed, OnTrapDestoryed);
    }

    /// <summary>
    /// 陷阱触发事件回调
    /// </summary>
    private void OnTrapDestoryed(TrapDestroyedEventArgs args)
    {
        _pendingRemovalTraps.Add(args.Trap);
    }
    #endregion
}
