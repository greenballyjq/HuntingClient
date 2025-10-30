using System;
using System.Collections.Generic;
using RVO;
using UnityEngine;

/// <summary>
/// RVO代理配置
/// </summary>
[System.Serializable]
public class RVOAgentConfig
{
    public float neighborDist = 10.0f;
    public int maxNeighbors = 6;
    public float timeHorizon = 10.0f;
    public float timeHorizonObst = 10.0f;
    public float radius = 3f;
    public float maxSpeed = 8f;
}

/// <summary>
/// RVO代理句柄
/// </summary>
public class RVOAgentHandle
{
    public int agentId;
    public bool isActive;
    public object owner;
    public float createTime;
}

/// <summary>
/// RVO更新模式
/// </summary>
public enum RVOUpdateMode
{
    /// <summary>
    /// 动态步长
    /// 步长 = Time.deltaTime
    /// </summary>
    DynamicStep,

    /// <summary>
    /// 固定步长
    /// 步长 = 固定时间间隔
    /// </summary>
    FixedStep
}

/// <summary>
/// RVO管理器
/// </summary>
public class RVOManager : MonoBehaviour
{
    private static RVOManager _instance;

    public static RVOManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("RVOManager");
                _instance = go.AddComponent<RVOManager>();
                DontDestroyOnLoad(go);
                _instance.Initialize();
            }
            return _instance;
        }
    }

    /// <summary>
    /// RVO模拟器实例
    /// </summary>
    private Simulator _simulator;

    /// <summary>
    /// 代理句柄字典
    /// </summary>
    private readonly Dictionary<int, RVOAgentHandle> _agentHandles = new Dictionary<int, RVOAgentHandle>();

    /// <summary>
    /// 代理复用队列
    /// </summary>
    private readonly Queue<int> _reuseQueue = new Queue<int>();

    /// <summary>
    /// 待处理的障碍（仅管理器侧缓存，调用ProcessAllObstacles前有效）
    /// </summary>
    private class PendingObstacle
    {
        public readonly List<Vector3> verticesUnity = new List<Vector3>();
        public object owner;
        public bool active = true;
    }

    /// <summary>
    /// 待处理障碍列表
    /// </summary>
    private readonly List<PendingObstacle> _pendingObstacles = new List<PendingObstacle>();

    /// <summary>
    /// 更新模式
    /// </summary>
    private RVOUpdateMode _updateMode = RVOUpdateMode.DynamicStep;

    /// <summary>
    /// 固定时间步长（仅在FixedStep模式下使用）
    /// </summary>
    private float _fixedTimeStep = 0.1f;

    /// <summary>
    /// 时间累积器（仅在FixedStep模式下使用）
    /// </summary>
    private float _timeAccumulator = 0f;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    private bool _isInitialized = false;

    /// <summary>
    /// RVO模拟步进完成事件
    /// </summary>
    public event Action OnRVOStepCompleted;

    private void Initialize()
    {
        if (_isInitialized)
            return;

        Debug.Log("[RVOManager] 初始化 RVO 管理器");

        // 获取模拟器实例
        _simulator = Simulator.Instance;

        // 清空模拟器
        _simulator.Clear();

        // 设置默认参数
        SetGlobalDefaults(new RVOAgentConfig());

        // 设置Works
        _simulator.SetNumWorkers(0);

        // 初始化障碍数据
        _pendingObstacles.Clear();

        _isInitialized = true;
    }

    public void Release()
    {
        Debug.Log("[RVOManager] 释放 RVO 管理器");

        _agentHandles.Clear();
        _reuseQueue.Clear();
        OnRVOStepCompleted = null;

        // 清空模拟器
        if (_simulator != null)
            _simulator.Clear();

        // 清理障碍数据
        _pendingObstacles.Clear();

        _isInitialized = false;
    }
   
    private void LateUpdate()
    {
        if (!_isInitialized)
            return;

        if (_updateMode == RVOUpdateMode.DynamicStep)
        {
            // 动态步长模式
            _simulator.setTimeStep(Time.deltaTime);
            _simulator.doStep();
            OnRVOStepCompleted?.Invoke();
        }
        else
        {
            // 固定步长模式
            _timeAccumulator += Time.deltaTime;

            while (_timeAccumulator >= _fixedTimeStep)
            {
                _simulator.setTimeStep(_fixedTimeStep);
                _simulator.doStep();
                OnRVOStepCompleted?.Invoke();
                _timeAccumulator -= _fixedTimeStep;
            }
        }
    }
    
    private void OnDestroy()
    {
        if (_instance == this)
        {
            Release();
            _instance = null;
        }
    }

    #region 公共方法
    /// <summary>
    /// 添加代理
    /// </summary>
    public int AddAgent(Vector3 position, RVOAgentConfig config, object owner = null)
    {
        if (config == null)
        {
            Debug.LogError("[RVOManager] 代理配置不能为空");
            return -1;
        }

        if (!_isInitialized || _simulator == null)
        {
            Debug.LogError("[RVOManager] 管理器未初始化");
            return -1;
        }

        int agentId;

        // 尝试复用已删除的代理
        if (_reuseQueue.Count > 0)
        {
            agentId = _reuseQueue.Dequeue();
            ResetAgent(agentId, position, config);

            Debug.Log($"[RVOManager] 复用代理 ID: {agentId}");
        }
        else
        {
            // 创建新代理
            RVOVector2 rvoPos = ToRVO(position);
            agentId = _simulator.addAgent(
                rvoPos,
                config.neighborDist,
                config.maxNeighbors,
                config.timeHorizon,
                config.timeHorizonObst,
                config.radius,
                config.maxSpeed,
                new RVOVector2(0, 0)
            );

            // 重建Workers
            _simulator.SetNumWorkers(0);

            Debug.Log($"[RVOManager] 创建新代理 ID: {agentId}");
        }

        // 创建句柄
        if (!_agentHandles.ContainsKey(agentId))
            _agentHandles[agentId] = new RVOAgentHandle();

        RVOAgentHandle handle = _agentHandles[agentId];
        handle.agentId = agentId;
        handle.isActive = true;
        handle.owner = owner;
        handle.createTime = Time.time;

        return agentId;
    }

    /// <summary>
    /// 移除代理（伪删除，实际移到场景外并加入复用队列）
    /// </summary>
    public void RemoveAgent(int agentId)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试移除无效代理: {agentId}");
            return;
        }

        // 移到场景外并重置状态
        _simulator.setAgentPosition(agentId, new RVOVector2(99999, 99999));
        _simulator.setAgentMaxSpeed(agentId, 0);
        _simulator.setAgentPrefVelocity(agentId, new RVOVector2(0, 0));
        _simulator.setAgentRadius(agentId, 0.01f);
        _simulator.setAgentNeighborDist(agentId, 0);

        // 标记为非激活
        _agentHandles[agentId].isActive = false;
        _agentHandles[agentId].owner = null;

        // 加入复用队列
        _reuseQueue.Enqueue(agentId);

        Debug.Log($"[RVOManager] 移除代理 ID: {agentId}");
    }
  
    /// <summary>
    /// 设置代理位置
    /// </summary>
    public void SetAgentPosition(int agentId, Vector3 position)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试设置无效代理 {agentId} 的位置");
            return;
        }

        _simulator.setAgentPosition(agentId, ToRVO(position));
    }

    /// <summary>
    /// 设置代理期望速度
    /// </summary>
    public void SetAgentPrefVelocity(int agentId, Vector3 velocity)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试设置无效代理 {agentId} 的期望速度");
            return;
        }

        _simulator.setAgentPrefVelocity(agentId, ToRVO(velocity));
    }

    /// <summary>
    /// 设置代理最大速度
    /// </summary>
    public void SetAgentMaxSpeed(int agentId, float maxSpeed)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试设置无效代理 {agentId} 的最大速度");
            return;
        }

        _simulator.setAgentMaxSpeed(agentId, maxSpeed);
    }

    /// <summary>
    /// 设置代理半径
    /// </summary>
    public void SetAgentRadius(int agentId, float radius)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试设置无效代理 {agentId} 的半径");
            return;
        }

        _simulator.setAgentRadius(agentId, radius);
    }

    /// <summary>
    /// 获取代理位置
    /// </summary>
    public Vector3 GetAgentPosition(int agentId)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试获取无效代理 {agentId} 的位置");
            return Vector3.zero;
        }

        return ToUnity(_simulator.getAgentPosition(agentId));
    }

    /// <summary>
    /// 获取代理速度
    /// </summary>
    public Vector3 GetAgentVelocity(int agentId)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试获取无效代理 {agentId} 的速度");
            return Vector3.zero;
        }

        return ToUnity(_simulator.getAgentVelocity(agentId));
    }

    /// <summary>
    /// 获取代理最大速度
    /// </summary>
    public float GetAgentMaxSpeed(int agentId)
    {
        if (!IsAgentValid(agentId))
        {
            Debug.LogWarning($"[RVOManager] 尝试获取无效代理 {agentId} 的最大速度");
            return 0;
        }

        return _simulator.getAgentMaxSpeed(agentId);
    }

    /// <summary>
    /// 获取代理句柄信息
    /// </summary>
    public RVOAgentHandle GetAgentHandle(int agentId)
    {
        if (_agentHandles.ContainsKey(agentId))
            return _agentHandles[agentId];

        return null;
    }

    /// <summary>
    /// 设置全局默认参数
    /// </summary>
    public void SetGlobalDefaults(RVOAgentConfig config)
    {
        if (config == null)
        {
            Debug.LogError("[RVOManager] 配置不能为空");
            return;
        }

        _simulator.setAgentDefaults(
            config.neighborDist,
            config.maxNeighbors,
            config.timeHorizon,
            config.timeHorizonObst,
            config.radius,
            config.maxSpeed,
            new RVOVector2(0, 0)
        );

        Debug.Log("[RVOManager] 设置全局默认参数");
    }

    /// <summary>
    /// 设置更新模式
    /// </summary>
    public void SetUpdateMode(RVOUpdateMode mode)
    {
        _updateMode = mode;
        Debug.Log($"[RVOManager] 设置更新模式: {mode}");
    }

    /// <summary>
    /// 设置固定时间步长（仅在 FixedStep 模式下有效）
    /// </summary>
    public void SetFixedTimeStep(float timeStep)
    {
        if (timeStep <= 0)
        {
            Debug.LogError("[RVOManager] 时间步长必须大于0");
            return;
        }

        _fixedTimeStep = timeStep;
        _simulator.setTimeStep(timeStep);
        Debug.Log($"[RVOManager] 设置固定时间步长: {timeStep}");
    }

    /// <summary>
    /// 注册多边形障碍
    /// </summary>
    public int RegisterObstaclePolygon(List<Vector3> vertices, object owner = null)
    {
        if (vertices == null || vertices.Count < 2)
        {
            Debug.LogWarning("[RVOManager] RegisterObstaclePolygon 顶点数不足");
            return -1;
        }

        var po = new PendingObstacle
        {
            owner = owner,
            active = true,
        };
        po.verticesUnity.AddRange(vertices);
        _pendingObstacles.Add(po);
        return _pendingObstacles.Count - 1;
    }

    /// <summary>
    /// 处理所有待处理障碍并构建障碍树
    /// </summary>
    public void ProcessAllObstacles()
    {
        if (!_isInitialized || _simulator == null)
        {
            Debug.LogError("[RVOManager] 管理器未初始化，无法处理障碍");
            return;
        }

        int added = 0;
        for (int i = 0; i < _pendingObstacles.Count; i++)
        {
            var po = _pendingObstacles[i];
            if (!po.active) continue;

            var rvoVerts = new List<RVOVector2>(po.verticesUnity.Count);
            for (int k = 0; k < po.verticesUnity.Count; k++)
            {
                var v = po.verticesUnity[k];
                rvoVerts.Add(new RVOVector2(v.x, v.z));
            }

            int startIdx = _simulator.addObstacle(rvoVerts);
            if (startIdx >= 0)
            {
                added++;
            }
        }

        if (added > 0)
        {
            _simulator.processObstacles();
            Debug.Log($"[RVOManager] 障碍处理完成，新增: {added}");
        }
        else
        {
            Debug.Log("[RVOManager] 无需处理障碍");
        }

        _pendingObstacles.Clear();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// Unity 坐标转 RVO 坐标
    /// </summary>
    private RVOVector2 ToRVO(Vector3 unityPos)
    {
        return new RVOVector2(unityPos.x, unityPos.z);
    }

    /// <summary>
    /// RVO 坐标转 Unity 坐标
    /// </summary>
    private Vector3 ToUnity(RVOVector2 rvoPos)
    {
        return new Vector3(rvoPos.x(), 0, rvoPos.y());
    }

    /// <summary>
    /// 重置代理属性
    /// </summary>
    private void ResetAgent(int agentId, Vector3 position, RVOAgentConfig config)
    {
        _simulator.setAgentPosition(agentId, ToRVO(position));
        _simulator.setAgentPrefVelocity(agentId, new RVOVector2(0, 0));
        _simulator.setAgentNeighborDist(agentId, config.neighborDist);
        _simulator.setAgentMaxNeighbors(agentId, config.maxNeighbors);
        _simulator.setAgentTimeHorizon(agentId, config.timeHorizon);
        _simulator.setAgentTimeHorizonObst(agentId, config.timeHorizonObst);
        _simulator.setAgentRadius(agentId, config.radius);
        _simulator.setAgentMaxSpeed(agentId, config.maxSpeed);
    }

    /// <summary>
    /// 检查代理是否有效
    /// </summary>
    private bool IsAgentValid(int agentId)
    {
        return _agentHandles.ContainsKey(agentId) && _agentHandles[agentId].isActive;
    }
    #endregion
}