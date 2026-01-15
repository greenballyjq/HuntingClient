using UnityEngine;

/// <summary>
/// RVO避障移动组件
/// </summary>
public class RVOMovement : MonoBehaviour, IAnimalMovement
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
private float _timeHorizon = 10.0f;

[Tooltip("与障碍物碰撞预测时间")]
[SerializeField]
/// <summary>
/// 与障碍物碰撞预测时间
/// </summary>
private float _timeHorizonObst = 10.0f;

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
private float _defaultMaxSpeed = 8.0f;

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
private float _rotationSmoothSpeed = 3.0f;

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
private Vector3 _targetDirection;

/// <summary>
/// 实际移动速度
/// </summary>
private Vector3 _actualVelocity;

/// <summary>
/// 保存的实际移动速度
/// </summary>
private Vector3 _lastActualVelocity;

/// <summary>
/// 是否已注册到RVO系统
/// </summary>
private bool isRegistered => _agentId != -1;

/// <summary>
/// 当前是否已启用RVO
/// </summary>
private bool _isEnabled = false;

/// <summary>
/// RVO管理器引用
/// </summary>
private RVOManager _rvoManager => RVOManager.Instance;

private void Awake()
{
    RegisterAgent();
    Enable();
}

private void Update()
{
    if (!isRegistered || !_isEnabled)
        return;

    // 更新期望速度
    UpdatePrefVelocity();

    // 更新旋转
    if (_enableAutoRotation)
        UpdateRotation();
}

private void OnDestroy()
{
    if (!isRegistered) 
        return;

    Disable();

    _rvoManager.RemoveAgent(_agentId);
    _agentId = -1;

    Debug.Log($"[RVOMovement] {gameObject.name} 已销毁并释放代理");
}

#region 公共方法
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
/// 启用RVO代理
/// </summary>
public void Enable()
{
    if (!isRegistered || _isEnabled)
        return;

    _actualVelocity = _lastActualVelocity;
    _rvoManager.SetAgentPrefVelocity(_agentId, _actualVelocity);
    _rvoManager.OnRVOStepCompleted += OnRVOStepCompleted;
    _isEnabled = true;
}

/// <summary>
/// 禁用RVO代理
/// </summary>
public void Disable(bool resetPosition = true)
{
    if (!isRegistered || !_isEnabled)
        return;

    _rvoManager.OnRVOStepCompleted -= OnRVOStepCompleted;

    _rvoManager.SetAgentPrefVelocity(_agentId, Vector3.zero);
    if (resetPosition)
        _rvoManager.SetAgentPosition(_agentId, new Vector3(99999, 99999, 99999));

    _currentMaxSpeed = 0f;
    _targetDirection = Vector3.zero;
    _lastActualVelocity = _actualVelocity;
    _actualVelocity = Vector3.zero;

    _isEnabled = false;
}

/// <summary>
/// 设置最大移动速度
/// </summary>
/// <param name="speed">最大速度值</param>
public void SetMaxSpeed(float speed)
{
    _currentMaxSpeed = speed;

    if (isRegistered)
        _rvoManager.SetAgentMaxSpeed(_agentId, speed);
}

/// <summary>
/// 同步当前位置到RVO代理
/// </summary>
public void SyncPosition()
{
    if (!isRegistered || !_isEnabled) return;

    // 同步当前位置到RVO系统
    _rvoManager.SetAgentPosition(_agentId, transform.position);
}

/// <summary>
/// 获取当前移动方向
/// </summary>
/// <returns></returns>
public Vector3 GetCurrentDirection()
{
    return _actualVelocity.normalized;
}
#endregion

#region IAnimalMovement 实现

public void SetDirection(Vector3 direction)
{
    SetMoveDirection(direction);
}

public void SetSpeed(float speed)
{
    SetMaxSpeed(speed);
}

void IAnimalMovement.Enable()
{
    Enable();
}

void IAnimalMovement.Disable()
{
    Disable(false);
}

public void Initialize(float speed, Vector3 direction)
{
    Enable();
    SyncPosition();
    SetMoveDirection(direction);
    SetMaxSpeed(speed);
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
        maxSpeed = _defaultMaxSpeed,
        radius = _radius,
    };

    // 注册代理到RVO系统
    _agentId = _rvoManager.AddAgent(transform.position, config, owner: this);
    if (_agentId < 0)
    {
        Debug.LogError($"[RVOMovement] {gameObject.name} 注册代理失败");
        return;
    }

    // 初始化运行时状态
    _currentMaxSpeed = _defaultMaxSpeed;
    _targetDirection = Vector3.zero;
    _actualVelocity = Vector3.zero;
    _rvoManager.SetAgentPosition(_agentId, new Vector3(99999, 99999, 99999));
}

/// <summary>
/// 更新期望速度到RVO系统
/// </summary>
private void UpdatePrefVelocity()
{
    // 计算期望速度
    Vector3 prefVelocity = _targetDirection * _currentMaxSpeed;

    // 设置到RVO系统
    _rvoManager.SetAgentPrefVelocity(_agentId, prefVelocity);
}

/// <summary>
/// 更新旋转
/// </summary>
private void UpdateRotation()
{
    // 计算目标朝向
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
/// RVO模拟步进完成回调
/// </summary>
private void OnRVOStepCompleted()
{
    // if (!isRegistered)
    if (!isRegistered || !_isEnabled)
        return;

    // 获取实际速度
    _actualVelocity = _rvoManager.GetAgentVelocity(_agentId);

    // 应用位置
    Vector3 newPos = _rvoManager.GetAgentPosition(_agentId);
    newPos.y = transform.position.y;
    transform.position = newPos;
}
#endregion

#region 调试可视化
/// <summary>
/// 绘制调试信息
/// </summary>
private void OnDrawGizmos()
{
    if (!Application.isPlaying || !isRegistered || !_isEnabled) 
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
