using UnityEngine;
using System;

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

    [Tooltip("是否启用自动旋转")]
    public bool enableAutoRotation = true;

    [Header("避障检测阈值")]
    [Tooltip("判定为避障的速度偏差阈值")]
    public float avoidanceThreshold = 0.5f;
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

    /// <summary>
    /// 是否暂停
    /// </summary>
    private bool _isPaused = false;

    /// <summary>
    /// 上一帧是否在移动（用于触发事件）
    /// </summary>
    private bool _wasMovingLastFrame = false;

    /// <summary>
    /// 上一帧是否在避障（用于触发事件）
    /// </summary>
    private bool _wasAvoidingLastFrame = false;
    #endregion

    #region 事件定义
    /// <summary>
    /// 开始移动事件
    /// </summary>
    public event Action OnStartMoving;

    /// <summary>
    /// 停止移动事件
    /// </summary>
    public event Action OnStopMoving;

    /// <summary>
    /// 避障触发事件（当开始避障时触发一次）
    /// </summary>
    public event Action OnAvoidanceTriggered;
    #endregion

    #region Unity 生命周期

    void Update()
    {
        if (!_isInitialized || _agentId < 0 || _isPaused)
            return;

        // 每帧设置期望速度到 RVO
        UpdatePrefVelocity();

        // 检测移动状态变化并触发事件
        CheckMovementStateChange();
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
    public void Init()
    {
        if (_isInitialized)
            return;

        // 速度与方向初始化检查
        if (_maxSpeed <= 0 || !IsMoving())
            Debug.LogWarning($"[RVOMovement] 速度或方向初始化错误，初始化速度：{_maxSpeed}，初始化方向：{_targetDirection}");

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
        _isPaused = false;
        _wasMovingLastFrame = false;
        _wasAvoidingLastFrame = false;
        _isInitialized = true;

        Debug.Log($"[RVOMovement] {gameObject.name} 初始化完成，代理 ID: {_agentId}");
    }

    /// <summary>
    /// 释放组件
    /// </summary>
    public void Release()
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

        // 清空事件
        OnStartMoving = null;
        OnStopMoving = null;
        OnAvoidanceTriggered = null;

        // 重置状态
        _agentId = -1;
        _isInitialized = false;
        _maxSpeed = 0;
        _isPaused = false;
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

        // 计算期望速度
        Vector3 prefVelocity = _targetDirection.magnitude > 0.001f
            ? _targetDirection.normalized * _maxSpeed
            : Vector3.zero;

        // 设置到 RVO
        RVOManager.Instance.SetAgentPrefVelocity(_agentId, prefVelocity);
    }

    /// <summary>
    /// RVO 模拟步进完成回调
    /// </summary>
    private void OnRVOStepCompleted()
    {
        if (!_isInitialized || _agentId < 0 || _isPaused)
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

        // 应用平滑后的旋转（如果启用）
        if (enableAutoRotation && _smoothRotationVector.magnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(_smoothRotationVector);

        // 记录位置供下一帧使用
        _lastPosition = newPos;
    }

    /// <summary>
    /// 检测移动状态变化并触发事件
    /// </summary>
    private void CheckMovementStateChange()
    {
        bool isMovingNow = IsMoving();

        // 检测移动开始/停止
        if (isMovingNow && !_wasMovingLastFrame)
        {
            OnStartMoving?.Invoke();
        }
        else if (!isMovingNow && _wasMovingLastFrame)
        {
            OnStopMoving?.Invoke();
        }

        _wasMovingLastFrame = isMovingNow;

        // 检测避障触发
        if (isMovingNow)
        {
            bool isAvoidingNow = IsAvoiding();
            if (isAvoidingNow && !_wasAvoidingLastFrame)
            {
                OnAvoidanceTriggered?.Invoke();
            }
            _wasAvoidingLastFrame = isAvoidingNow;
        }
        else
        {
            _wasAvoidingLastFrame = false;
        }
    }
    #endregion

    #region 公共接口 - 状态查询
    /// <summary>
    /// 检查是否已初始化
    /// </summary>
    public bool IsInitialized()
    {
        return _isInitialized;
    }

    /// <summary>
    /// 检查是否正在移动
    /// </summary>
    public bool IsMoving()
    {
        return _targetDirection.sqrMagnitude > 0.001f;
    }

    /// <summary>
    /// 检查是否暂停
    /// </summary>
    public bool IsPaused()
    {
        return _isPaused;
    }

    /// <summary>
    /// 判断是否正在避障（实际速度与期望速度偏差较大）
    /// </summary>
    public bool IsAvoiding()
    {
        if (_agentId < 0 || !IsMoving())
            return false;

        Vector3 actualVelocity = GetVelocity();
        Vector3 preferredVelocity = _targetDirection.normalized * _maxSpeed;

        float deviation = Vector3.Distance(actualVelocity, preferredVelocity);
        return deviation > avoidanceThreshold;
    }

    /// <summary>
    /// 获取避障偏差程度（0-1，0表示无偏差，1表示完全偏离）
    /// </summary>
    public float GetAvoidanceDeviation()
    {
        if (_agentId < 0 || !IsMoving())
            return 0;

        Vector3 actualVelocity = GetVelocity();
        Vector3 preferredVelocity = _targetDirection.normalized * _maxSpeed;

        if (preferredVelocity.magnitude < 0.001f)
            return 0;

        float deviation = Vector3.Distance(actualVelocity, preferredVelocity);
        return Mathf.Clamp01(deviation / _maxSpeed);
    }

    /// <summary>
    /// 获取当前移动方向（期望方向）
    /// </summary>
    public Vector3 GetMoveDirection()
    {
        return _targetDirection;
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

    /// <summary>
    /// 获取当前代理ID
    /// </summary>
    public int GetAgentId()
    {
        return _agentId;
    }
    #endregion

    #region 公共接口 - 控制方法
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
    /// 立即停止移动
    /// </summary>
    public void Stop()
    {
        SetMoveDirection(Vector3.zero);
    }

    /// <summary>
    /// 暂停避障更新（保持当前速度，但不参与RVO计算）
    /// </summary>
    public void Pause()
    {
        _isPaused = true;
    }

    /// <summary>
    /// 恢复避障更新
    /// </summary>
    public void Resume()
    {
        _isPaused = false;
    }

    /// <summary>
    /// 瞬移到指定位置（不经过RVO计算）
    /// </summary>
    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;
        _lastPosition = newPosition;

        if (_agentId >= 0 && RVOManager.Instance != null)
            RVOManager.Instance.SetAgentPosition(_agentId, newPosition);
    }
    #endregion

    #region 公共接口 - 参数设置
    /// <summary>
    /// 设置最大速度
    /// </summary>
    /// <param name="speed">最大速度值</param>
    public void SetMaxSpeed(float speed)
    {
        if (speed < 0)
        {
            Debug.LogWarning($"[RVOMovement] 速度不能为负数: {speed}");
            return;
        }

        _maxSpeed = speed;

        if (_agentId >= 0 && RVOManager.Instance != null)
            RVOManager.Instance.SetAgentMaxSpeed(_agentId, speed);
    }

    /// <summary>
    /// 动态设置代理半径
    /// </summary>
    public void SetRadius(float newRadius)
    {
        if (newRadius < 0)
        {
            Debug.LogWarning($"[RVOMovement] 半径不能为负数: {newRadius}");
            return;
        }

        radius = newRadius;

        if (_agentId >= 0 && RVOManager.Instance != null)
            RVOManager.Instance.SetAgentRadius(_agentId, newRadius);
    }

    /// <summary>
    /// 设置是否自动旋转
    /// </summary>
    public void SetAutoRotation(bool enabled)
    {
        enableAutoRotation = enabled;
    }

    /// <summary>
    /// 设置旋转平滑速度
    /// </summary>
    public void SetRotationSmoothSpeed(float speed)
    {
        rotationSmoothSpeed = Mathf.Max(0, speed);
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

        // 如果正在避障，显示红色警告圈
        if (_isInitialized && IsAvoiding())
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(pos, radius * 1.2f);
        }

        // 如果暂停，显示黄色叉
        if (_isPaused)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos + Vector3.left * 0.5f, pos + Vector3.right * 0.5f);
            Gizmos.DrawLine(pos + Vector3.forward * 0.5f, pos + Vector3.back * 0.5f);
        }
    }
    #endregion
}