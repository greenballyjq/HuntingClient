using UnityEngine;

/// <summary>
/// RVO避障移动组件
/// </summary>
public class RVOMovement : MonoBehaviour
{
    [Header("RVO代理参数")]
    [Tooltip("邻居检测距离")]
    public float neighborDist = 10.0f;

    [Tooltip("最多考虑的邻居数量")]
    public int maxNeighbors = 6;

    [Tooltip("预测与代理碰撞的时间")]
    public float timeHorizon = 3f;

    [Tooltip("预测与障碍物碰撞的时间")]
    public float timeHorizonObst = 3f;

    [Tooltip("代理半径")]
    public float radius = 3.0f;

    [Header("旋转参数")]
    [Tooltip("是否启用自动旋转")]
    public bool enableAutoRotation = true;

    [Tooltip("旋转平滑速度")]
    public float rotationSmoothSpeed = 5.0f;

    [Tooltip("最小旋转速度阈值（低于此速度不旋转）")]
    public float minRotationSpeed = 0.1f;

    /// <summary>
    /// RVO代理ID
    /// </summary>
    private int _agentId = -1;

    /// <summary>
    /// 最大移动速度
    /// </summary>
    private float _maxSpeed;

    /// <summary>
    /// 目标移动方向
    /// </summary>
    private Vector3 _targetDirection;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    private bool _isInitialized = false;

    /// <summary>
    /// 是否已暂停
    /// </summary>
    private bool _isPaused = false;

    /// <summary>
    /// 实际移动速度（RVO计算后的）
    /// </summary>
    private Vector3 _actualVelocity;

    /// <summary>
    /// RVO管理器
    /// </summary>
    private RVOManager _rvoManager => RVOManager.Instance;

    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Init()
    {
        if (_isInitialized)
            return;

        // 注册代理
        RVOAgentConfig config = new RVOAgentConfig
        {
            neighborDist = neighborDist,
            maxNeighbors = maxNeighbors,
            timeHorizon = timeHorizon,
            timeHorizonObst = timeHorizonObst,
            radius = radius,
            maxSpeed = _maxSpeed,
        };
        _agentId = _rvoManager.AddAgent(transform.position, config, owner: this);
        if (_agentId < 0)
        {
            Debug.LogError($"[RVOMovement] {gameObject.name} 注册代理失败");
            return;
        }

        // 订阅步进回调
        _rvoManager.OnRVOStepCompleted += OnRVOStepCompleted;

        // 初始化状态
        _isPaused = false;
        _actualVelocity = Vector3.zero;

        // 设置初始化标识
        _isInitialized = true;

        Debug.Log($"[RVOMovement] {gameObject.name} 初始化完成，代理 ID: {_agentId}");
    }

    /// <summary>
    /// 初始化组件并设置初始朝向
    /// </summary>
    /// <param name="initialDirection">初始朝向</param>
    public void Init(Vector3 initialDirection, float initialMaxSpeed)
    {
        // 设置初始方向和速度
        SetMoveDirection(initialDirection);
        SetMaxSpeed(initialMaxSpeed);
        Init();

        // 设置依附对象初始朝向
        if (initialDirection.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(initialDirection);
    }

    /// <summary>
    /// 释放组件
    /// </summary>
    public void Release()
    {
        if (!_isInitialized)
            return;

        Debug.Log($"[RVOMovement] {gameObject.name} 释放代理 ID: {_agentId}");

        // 移除代理
        _rvoManager.RemoveAgent(_agentId);

        // 取消订阅步进回调
        _rvoManager.OnRVOStepCompleted -= OnRVOStepCompleted;

        // 重置状态
        _maxSpeed = 0;
        _targetDirection = Vector3.zero;
        _actualVelocity = Vector3.zero;
        _isPaused = false;
        _isInitialized = false;
        _agentId = -1;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        // 更新期望速度
        UpdatePrefVelocity();

        // 更新旋转
        if (enableAutoRotation)
            UpdateRotation();
    }

    private void OnDestroy()
    {
        Release();
    }

    #region 公共方法
    /// <summary>
    /// 设置移动方向
    /// </summary>
    public void SetMoveDirection(Vector3 direction)
    {
        _targetDirection = direction.normalized;
    }

    /// <summary>
    /// 设置最大速度
    /// </summary>
    public void SetMaxSpeed(float speed)
    {
        _maxSpeed = speed;
        if (_agentId >= 0 && !_isPaused)
            _rvoManager.SetAgentMaxSpeed(_agentId, speed);
    }

    /// <summary>
    /// 暂停移动
    /// </summary>
    public void PauseMovement()
    {
        if (!_isInitialized || _isPaused)
            return;

        _isPaused = true;

        // 将速度设为0
        if (_agentId >= 0)
        {
            _rvoManager.SetAgentMaxSpeed(_agentId, 0);
            _rvoManager.SetAgentPrefVelocity(_agentId, Vector3.zero);
        }

        Debug.Log($"[RVOMovement] {gameObject.name} 暂停移动");
    }

    /// <summary>
    /// 恢复移动
    /// </summary>
    public void ResumeMovement()
    {
        if (!_isInitialized || !_isPaused)
            return;

        _isPaused = false;

        // 恢复速度
        if (_agentId >= 0)
            _rvoManager.SetAgentMaxSpeed(_agentId, _maxSpeed);

        Debug.Log($"[RVOMovement] {gameObject.name} 恢复移动");
    }

    /// <summary>
    /// 获取当前是否暂停
    /// </summary>
    public bool IsPaused()
    {
        return _isPaused;
    }

    /// <summary>
    /// 获取实际移动速度（RVO计算后的）
    /// </summary>
    public Vector3 GetActualVelocity()
    {
        return _actualVelocity;
    }
    #endregion

    #region 私有方法    
    /// <summary>
    /// 同步期望速度
    /// </summary>
    private void UpdatePrefVelocity()
    {
        if (_isPaused)
            return;

        // 设置期望速度
        Vector3 prefVelocity = _targetDirection.normalized * _maxSpeed;
        _rvoManager.SetAgentPrefVelocity(_agentId, prefVelocity);
    }

    /// <summary>
    /// 更新旋转
    /// </summary>
    private void UpdateRotation()
    {
        // 如果实际速度太小，不旋转
        if (_actualVelocity.sqrMagnitude < minRotationSpeed * minRotationSpeed)
            return;

        // 计算目标朝向（基于实际移动方向）
        Vector3 targetDirection = _actualVelocity.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // 平滑旋转
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// RVO模拟步进完成回调
    /// </summary>
    private void OnRVOStepCompleted()
    {
        if (!_isInitialized)
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
    private void OnDrawGizmos()
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

        // 实际移动方向（青色箭头，更粗）
        if (_actualVelocity.sqrMagnitude > 0.001f)
        {
            Gizmos.color = Color.cyan;
            Vector3 actualEnd = pos + _actualVelocity.normalized * 2.5f;
            Gizmos.DrawLine(pos, actualEnd);
            Gizmos.DrawSphere(actualEnd, 0.15f);
            
            // 绘制速度大小文本（通过线段密度表示）
            float speedRatio = _actualVelocity.magnitude / Mathf.Max(_maxSpeed, 0.1f);
            for (int i = 1; i <= 3; i++)
            {
                if (speedRatio >= i * 0.25f)
                {
                    Vector3 marker = pos + _actualVelocity.normalized * (0.5f * i);
                    Gizmos.DrawWireSphere(marker, 0.08f);
                }
            }
        }

        // 代理半径（绿色圈）
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(pos, radius);

        // 邻居检测距离（黄色圈）
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(pos, neighborDist);

        // 暂停状态指示（红色圈）
        if (_isPaused)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(pos, radius * 1.2f);
        }
    }
    #endregion
}