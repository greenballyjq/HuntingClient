using UnityEngine;

/// <summary>
/// RVO 避障移动组件
/// 负责将 RVO 代理的避障计算结果应用到 Unity 对象上
/// </summary>
public class RVOMovement : MonoBehaviour
{
    #region 配置参数
    [Header("RVO 代理参数")]
    [Tooltip("邻居检测距离")]
    public float neighborDist = 5f;
    
    [Tooltip("最多考虑的邻居数量")]
    public int maxNeighbors = 5;
    
    [Tooltip("时间视界（预测与代理碰撞的时间）")]
    public float timeHorizon = 10f;
    
    [Tooltip("障碍物时间视界（预测与障碍物碰撞的时间）")]
    public float timeHorizonObst = 10f;
    
    [Tooltip("代理半径")]
    public float radius = 0.8f;

    [Header("旋转参数")]
    [Tooltip("旋转平滑速度")]
    public float rotationSmoothSpeed = 5f;
    #endregion

    #region 私有字段
    /// <summary>
    /// RVO 代理 ID
    /// </summary>
    private int _agentId = -1;
    
    /// <summary>
    /// 目标移动方向
    /// </summary>
    private Vector3 _targetDirection = Vector3.zero;
    
    /// <summary>
    /// 上一帧的位置（用于计算移动方向）
    /// </summary>
    private Vector3 _lastPosition;
    
    /// <summary>
    /// 平滑旋转向量（避免朝向抖动）
    /// </summary>
    private Vector3 _smoothRotationVector;
    
    /// <summary>
    /// 最大移动速度
    /// </summary>
    private float _maxSpeed;
    
    /// <summary>
    /// 是否已初始化
    /// </summary>
    private bool _isInitialized = false;
    #endregion

    #region Unity 生命周期
    void OnEnable()
    {
        Initialize();
    }

    void OnDisable()
    {
        Release();
    }

    void Update()
    {
        if (!_isInitialized || _agentId < 0)
            return;

        // 每帧设置期望速度到 RVO
        UpdatePrefVelocity();
    }

    void OnDestroy()
    {
        Release();
    }
    #endregion

    #region 初始化和释放
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Initialize()
    {
        if (_isInitialized)
            return;

        // 注意：maxSpeed 需要在调用 Initialize 前通过 SetMaxSpeed 设置
        // 如果未设置，使用默认值 2.0f
        if (_maxSpeed <= 0)
        {
            _maxSpeed = 2.0f;
            Debug.LogWarning($"[RVOMovement] {gameObject.name} 未设置最大速度，使用默认值: {_maxSpeed}");
        }

        // 创建 RVO 代理配置
        RVOAgentConfig config = new RVOAgentConfig
        {
            radius = radius,
            maxSpeed = _maxSpeed,
            neighborDist = neighborDist,
            maxNeighbors = maxNeighbors,
            timeHorizon = timeHorizon,
            timeHorizonObst = timeHorizonObst
        };

        // 向 RVOManager 注册代理
        _agentId = RVOManager.Instance.AddAgent(transform.position, config, owner: this);
        
        if (_agentId < 0)
        {
            Debug.LogError($"[RVOMovement] {gameObject.name} 注册代理失败");
            return;
        }

        // 订阅 RVO 步进完成事件
        RVOManager.Instance.OnRVOStepCompleted += OnRVOStepCompleted;

        // 初始化状态
        _lastPosition = transform.position;
        _smoothRotationVector = Vector3.zero;
        _targetDirection = Vector3.zero;
        _isInitialized = true;

        Debug.Log($"[RVOMovement] {gameObject.name} 初始化完成，代理 ID: {_agentId}");
    }

    /// <summary>
    /// 释放组件
    /// </summary>
    private void Release()
    {
        if (!_isInitialized)
            return;

        // 取消订阅事件
        if (RVOManager.Instance != null)
            RVOManager.Instance.OnRVOStepCompleted -= OnRVOStepCompleted;

        // 移除代理
        if (_agentId >= 0 && RVOManager.Instance != null)
        {
            RVOManager.Instance.RemoveAgent(_agentId);
            Debug.Log($"[RVOMovement] {gameObject.name} 释放代理 ID: {_agentId}");
        }

        // 重置状态
        _agentId = -1;
        _isInitialized = false;
        _maxSpeed = 0;
    }
    #endregion

    #region RVO 更新逻辑
    /// <summary>
    /// 每帧更新期望速度
    /// </summary>
    private void UpdatePrefVelocity()
    {
        // 同步当前位置到 RVO
        RVOManager.Instance.SetAgentPosition(_agentId, transform.position);

        // 计算期望速度：方向 * 速度
        Vector3 prefVelocity = _targetDirection.magnitude > 0.001f
            ? _targetDirection.normalized * _maxSpeed
            : Vector3.zero;

        // 设置到 RVO
        RVOManager.Instance.SetAgentPrefVelocity(_agentId, prefVelocity);
    }

    /// <summary>
    /// RVO 模拟步进完成回调
    /// 将 RVO 计算的结果同步到 Unity 对象
    /// </summary>
    private void OnRVOStepCompleted()
    {
        if (!_isInitialized || _agentId < 0)
            return;

        // 获取 RVO 计算后的新位置
        Vector3 newPos = RVOManager.Instance.GetAgentPosition(_agentId);
        newPos.y = transform.position.y;

        // 计算实际移动方向
        Vector3 actualMoveDirection = newPos - _lastPosition;

        // 初始化平滑向量（首次调用时使用当前朝向）
        if (_smoothRotationVector == Vector3.zero)
            _smoothRotationVector = transform.forward;

        // 平滑过渡旋转方向
        if (actualMoveDirection.magnitude > 0.01f)
        {
            _smoothRotationVector = Vector3.Lerp(
                _smoothRotationVector,
                actualMoveDirection,
                Time.deltaTime * rotationSmoothSpeed
            );
        }

        // 应用新位置
        transform.position = newPos;

        // 应用平滑后的旋转
        if (_smoothRotationVector.magnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(_smoothRotationVector);

        // 记录位置供下一帧使用
        _lastPosition = newPos;
    }
    #endregion

    #region 公共接口
    /// <summary>
    /// 设置移动方向
    /// </summary>
    /// <param name="direction">移动方向</param>
    public void SetMoveDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            _targetDirection = direction.normalized;
        else
            _targetDirection = Vector3.zero;
    }

    /// <summary>
    /// 设置最大速度
    /// </summary>
    /// <param name="speed">最大速度值</param>
    public void SetMaxSpeed(float speed)
    {
        if (speed < 0){
            Debug.LogWarning($"[RVOMovement] 速度不能为负数: {speed}");
            return;
        }
            
        _maxSpeed = speed;

        if (_agentId >= 0 && RVOManager.Instance != null)
            RVOManager.Instance.SetAgentMaxSpeed(_agentId, speed);
    }

    /// <summary>
    /// 获取当前实际速度
    /// </summary>
    public Vector3 GetVelocity()
    {
        if (_agentId >= 0 && RVOManager.Instance != null)
            return RVOManager.Instance.GetAgentVelocity(_agentId);

        return Vector3.zero;
    }

    /// <summary>
    /// 获取当前最大速度配置
    /// </summary>
    public float GetMaxSpeed()
    {
        return _maxSpeed;
    }
    #endregion

    #region 调试可视化
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Vector3 pos = transform.position;

        // 期望移动方向（绿色箭头）
        if (_targetDirection.sqrMagnitude > 0.001f)
        {
            Gizmos.color = Color.green;
            Vector3 targetEnd = pos + _targetDirection * 2f;
            Gizmos.DrawLine(pos, targetEnd);
            Gizmos.DrawSphere(targetEnd, 0.1f);
        }

        // 平滑旋转方向（白色箭头）
        if (_smoothRotationVector.magnitude > 0.01f)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos, pos + _smoothRotationVector.normalized * 1.5f);
        }

        // 代理半径（绿色圈）
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(pos, radius);

        // 邻居检测距离（黄色圈）
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(pos, neighborDist);
    }
    #endregion
}