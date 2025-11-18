using UnityEngine;

/// <summary>
/// RVO避障移动组件 - 提供基于RVO2的自动避障移动能力
/// </summary>
public class RVOMovement : MonoBehaviour
{
    [Header("RVO代理参数")]
    [Tooltip("邻居检测距离")]
    [SerializeField]
    /// <summary>
    /// 邻居检测距离
    /// </summary>
    private float _neighborDist = 10.0f;

    [Tooltip("最大邻居数量")]
    [SerializeField]
    /// <summary>
    /// 最大邻居数量
    /// </summary>
    private int _maxNeighbors = 6;

    [Tooltip("与代理碰撞预测时间")]
    [SerializeField]
    /// <summary>
    /// 与代理碰撞预测时间
    /// </summary>
    private float _timeHorizon = 8.0f;

    [Tooltip("与障碍物碰撞预测时间")]
    [SerializeField]
    /// <summary>
    /// 与障碍物碰撞预测时间
    /// </summary>
    private float _timeHorizonObst = 1000.0f;

    [Tooltip("代理半径")]
    [SerializeField]
    /// <summary>
    /// 代理半径
    /// </summary>
    private float _radius = 3.0f;

    [Tooltip("最大移动速度")]
    [SerializeField]
    /// <summary>
    /// 最大移动速度
    /// </summary>
    private float _maxSpeed = 8.0f;

    [Header("旋转参数")]
    [Tooltip("是否启用自动旋转")]
    [SerializeField]
    /// <summary>
    /// 是否启用自动旋转
    /// </summary>
    private bool _enableAutoRotation = true;

    [Tooltip("旋转平滑速度")]
    [SerializeField]
    /// <summary>
    /// 旋转平滑速度
    /// </summary>
    private float _rotationSmoothSpeed = 5.0f;

    /// <summary>
    /// RVO代理ID
    /// </summary>
    private int _agentId = -1;

    /// <summary>
    /// 当前最大移动速度
    /// </summary>
    private float _currentMaxSpeed;

    /// <summary>
    /// 目标移动方向
    /// </summary>
    private Vector3 _targetDirection = Vector3.zero;

    /// <summary>
    /// 实际移动速度
    /// </summary>
    private Vector3 _actualVelocity = Vector3.zero;

    /// <summary>
    /// 当前是否已启用RVO
    /// </summary>
    private bool _isRVOEnabled = false;

    /// <summary>
    /// RVO管理器引用
    /// </summary>
    private RVOManager _rvoManager => RVOManager.Instance;

    /// <summary>
    /// 是否已注册到RVO系统
    /// </summary>
    public bool IsRegistered => _agentId != -1;

    /// <summary>
    /// Awake时注册RVO代理
    /// </summary>
    private void Awake()
    {
        RegisterAgent();
    }

    /// <summary>
    /// OnDestroy时释放RVO代理
    /// </summary>
    private void OnDestroy()
    {
        if (!IsRegistered)
            return;

        // 禁用RVO
        DisableRVO();

        // 移除代理
        _rvoManager.RemoveAgent(_agentId);

        // 重置状态
        _agentId = -1;
        _currentMaxSpeed = 0;
        _actualVelocity = Vector3.zero;
    }

    /// <summary>
    /// Update中更新期望速度和旋转
    /// </summary>
    private void Update()
    {
        if (!IsRegistered || !_isRVOEnabled)
            return;

        // 更新期望速度
        UpdatePrefVelocity();

        // 更新旋转
        if (_enableAutoRotation)
            UpdateRotation();
    }

    #region 公共方法
    /// <summary>
    /// 设置移动方向
    /// </summary>
    /// <param name="direction">移动方向（会自动归一化）</param>
    public void SetMoveDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            _targetDirection = direction.normalized;
        }
        else
        {
            _targetDirection = Vector3.zero;
        }
    }

    /// <summary>
    /// 设置最大移动速度
    /// </summary>
    /// <param name="speed">最大速度值（必须非负）</param>
    public void SetMaxSpeed(float speed)
    {
        if (speed < 0)
        {
            Debug.LogWarning($"[RVOMovement] {gameObject.name} 设置的速度值不能为负: {speed}");
            return;
        }

        _currentMaxSpeed = speed;

        if (IsRegistered)
        {
            _rvoManager.SetAgentMaxSpeed(_agentId, speed);
        }
    }

    /// <summary>
    /// 启用RVO（订阅步进事件，重置运行时状态）
    /// </summary>
    public void EnableRVO()
    {
        if (!IsRegistered || _isRVOEnabled)
            return;

        _rvoManager.OnRVOStepCompleted += OnRVOStepCompleted;

        if (_currentMaxSpeed <= 0f)
            _currentMaxSpeed = _maxSpeed;

        _isRVOEnabled = true;
        UpdatePrefVelocity();

        Debug.Log($"[RVOMovement] {gameObject.name} 已启用，代理 ID: {_agentId}");
    }

    /// <summary>
    /// 禁用RVO（取消订阅并停止移动）
    /// </summary>
    public void DisableRVO()
    {
        if (!IsRegistered || !_isRVOEnabled)
            return;

        _rvoManager.OnRVOStepCompleted -= OnRVOStepCompleted;
        _rvoManager.SetAgentPrefVelocity(_agentId, Vector3.zero);
        _actualVelocity = Vector3.zero;
        _isRVOEnabled = false;

        Debug.Log($"[RVOMovement] {gameObject.name} 已禁用，代理 ID: {_agentId}");
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册RVO代理
    /// </summary>
    private void RegisterAgent()
    {
        // 构建RVO代理配置
        RVOAgentConfig config = new RVOAgentConfig
        {
            neighborDist = _neighborDist,
            maxNeighbors = _maxNeighbors,
            timeHorizon = _timeHorizon,
            timeHorizonObst = _timeHorizonObst,
            radius = _radius,
            maxSpeed = _maxSpeed,
        };

        // 注册代理到RVO系统
        _agentId = _rvoManager.AddAgent(transform.position, config, owner: this);
        if (_agentId < 0)
        {
            Debug.LogError($"[RVOMovement] {gameObject.name} 注册代理失败");
            return;
        }

        // 初始化运行时状态
        _currentMaxSpeed = _maxSpeed;

        Debug.Log($"[RVOMovement] {gameObject.name} 注册代理成功，代理 ID: {_agentId}");

        // 默认启用RVO
        EnableRVO();
    }

    /// <summary>
    /// 更新期望速度到RVO系统
    /// </summary>
    private void UpdatePrefVelocity()
    {
        if (!IsRegistered || !_isRVOEnabled)
            return;

        // 计算期望速度
        Vector3 prefVelocity = _targetDirection.normalized * _currentMaxSpeed;

        // 设置到RVO系统
        _rvoManager.SetAgentPrefVelocity(_agentId, prefVelocity);
    }

    /// <summary>
    /// 更新旋转（基于实际移动方向）
    /// </summary>
    private void UpdateRotation()
    {
        // 计算目标朝向（基于实际移动方向）
        Vector3 targetDirection = _actualVelocity.normalized;

        // 平滑旋转
        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSmoothSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// RVO模拟步进完成回调（从RVOManager订阅）
    /// </summary>
    private void OnRVOStepCompleted()
    {
        if (!IsRegistered)
            return;

        // 获取实际速度（RVO计算后的）
        _actualVelocity = _rvoManager.GetAgentVelocity(_agentId);

        // 应用位置
        Vector3 newPos = _rvoManager.GetAgentPosition(_agentId);
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
    #endregion

    #region 调试可视化
    /// <summary>
    /// 绘制调试信息（Gizmos）
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !IsRegistered)
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

        // 实际移动方向（青色箭头）
        if (_actualVelocity.sqrMagnitude > 0.001f)
        {
            Gizmos.color = Color.cyan;
            Vector3 actualEnd = pos + _actualVelocity.normalized * 2.5f;
            Gizmos.DrawLine(pos, actualEnd);
            Gizmos.DrawSphere(actualEnd, 0.15f);
        }

        // 代理半径（绿色圈）
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(pos, _radius);

        // 邻居检测距离（黄色圈）
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(pos, _neighborDist);
    }
    #endregion
}
