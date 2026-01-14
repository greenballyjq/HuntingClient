using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Pool;
using GameFramework.Manager;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 陷阱管理器
/// </summary>
public class TrapManager : IRoundManager, IRoundResettable
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
    /// 场上所有陷阱列表
    /// </summary>
    private List<GameObject> _activeTraps = new List<GameObject>();

    public void Init(RoundContext context)
    {
        RegisterEvents();
        Debug.Log("[TrapManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();
        ClearAllTraps();
        Debug.Log("[TrapManager] 已释放");
    }

    public void Cleanup()
    {
        ClearAllTraps();
    }

    public void ReInit(RoundContext context){}

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
        Dictionary<EVolumeType, float[]> attractRadiusRangeByVolume,
        string prefabPath
    )
    {
        // 从对象池获取
        GameObject trap = await _gameObjectPoolManager.SpawnAsync(prefabPath);

        // 初始化陷阱
        IPoolItem poolItem = trap.GetComponent<IPoolItem>();

        trap.transform.position = position;
        trap.transform.rotation = Quaternion.identity;

        TrapBehavior trapBehavior = trap.GetComponent<TrapBehavior>();
        trapBehavior.Init(attractRadius, triggerRadius, attractRadiusRangeByVolume);

        // 注册到列表
        _activeTraps.Add(trap);

        return trap;
    }

    /// <summary>
    /// 获取所有陷阱位置
    /// </summary>
    /// <returns>陷阱位置列表</returns>
    public List<Vector3> GetAllTrapPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject trap in _activeTraps)
            positions.Add(trap.transform.position);

        return positions;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 清理所有陷阱
    /// </summary>
    private void ClearAllTraps()
    {
        foreach (GameObject trap in _activeTraps)
            _gameObjectPoolManager.Despawn(trap);

        _activeTraps.Clear();
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
        _activeTraps.Remove(args.Trap.gameObject);
        _gameObjectPoolManager.Despawn(args.Trap.gameObject);
    }
    #endregion
}

