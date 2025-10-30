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

    [Tooltip("时间视界（预测与代理碰撞的时间）")]
    public float timeHorizon = 3f;

    [Tooltip("障碍物时间视界（预测与障碍物碰撞的时间）")]
    public float timeHorizonObst = 3f;

    [Tooltip("代理半径")]
    public float radius = 3.0f;

    [Header("旋转参数")]
    [Tooltip("旋转平滑速度")]
    public float rotationSmoothSpeed = 3.0f;

    /// <summary>
    /// 数值判断阈值
    /// </summary>
    private const float EPS = 0.001f;

    /// <summary>
    /// RVO代理ID
    /// </summary>
    private int _agentId = -1;

    /// <summary>
    /// 目标移动方向（期望方向）
    /// </summary>
    private Vector3 _targetDirection;

    /// <summary>
    /// 上一帧位置（用于计算朝向）
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

        // 构建代理配置
        RVOAgentConfig config = new RVOAgentConfig
        {
            neighborDist = neighborDist,
            maxNeighbors = maxNeighbors,
            timeHorizon = timeHorizon,
            timeHorizonObst = timeHorizonObst,
            radius = radius,
            maxSpeed = _maxSpeed,
        };

        // 注册代理
        _agentId = _rvoManager.AddAgent(transform.position, config, owner: this);
        if (_agentId < 0)
        {
            Debug.LogError($"[RVOMovement] {gameObject.name} 注册代理失败");
            return;
        }

        // 订阅步进回调
        _rvoManager.OnRVOStepCompleted += OnRVOStepCompleted;

        // 记录初始状态并对齐初始朝向（方向为零则使用当前前向）
        _lastPosition = transform.position;
        _smoothRotationVector = (_targetDirection.sqrMagnitude > EPS ? _targetDirection : transform.forward).normalized;
        if (_smoothRotationVector.sqrMagnitude > EPS)
            transform.rotation = Quaternion.LookRotation(_smoothRotationVector);

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


        // 移除代理（先移除再重置本地ID与状态）
        int oldId = _agentId;
        if (_agentId >= 0)
            _rvoManager.RemoveAgent(_agentId);

        // 取消订阅步进回调
        _rvoManager.OnRVOStepCompleted -= OnRVOStepCompleted;

        // 重置状态
        _maxSpeed = 0;
        _targetDirection = Vector3.zero;
        _lastPosition = Vector3.zero;
        _smoothRotationVector = Vector3.zero;
        _isInitialized = false;
        _agentId = -1;

        Debug.Log($"[RVOMovement] {gameObject.name} 释放代理 ID: {oldId}");
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        UpdatePrefVelocity();
    }

    private void OnDestroy()
    {
        Release();
    }

    #region 公共方法
    /// <summary>
    /// 设置移动方向。
    /// </summary>
    public void SetMoveDirection(Vector3 direction)
    {
        _targetDirection = direction.normalized;
    }

    /// <summary>
    /// 设置最大速度。
    /// </summary>
    public void SetMaxSpeed(float speed)
    {
        _maxSpeed = speed;
        if (_agentId >= 0)
            _rvoManager.SetAgentMaxSpeed(_agentId, speed);
    }
    #endregion

    #region 私有方法    
    /// <summary>
    /// 同步当前位置与期望速度。
    /// </summary>
    private void UpdatePrefVelocity()
    {
        if (!_isInitialized)
            return;

        if (_agentId < 0)
            return;

        _rvoManager.SetAgentPosition(_agentId, transform.position);

        Vector3 prefVelocity = _targetDirection.normalized * _maxSpeed;
        _rvoManager.SetAgentPrefVelocity(_agentId, prefVelocity);
    }

    /// <summary>
    /// RVO模拟步进完成回调
    /// </summary>
    private void OnRVOStepCompleted()
    {
        if (!_isInitialized)
            return;

        // 应用位置
        Vector3 newPos = _rvoManager.GetAgentPosition(_agentId);
        newPos.y = transform.position.y;
        transform.position = newPos;

        // 平滑旋转方向（静止时沿用上一帧方向）
        Vector3 dir = newPos - _lastPosition;
        if (dir.sqrMagnitude <= EPS)
            dir = _smoothRotationVector;
        float t = Mathf.Clamp01(rotationSmoothSpeed * Time.deltaTime);
        _smoothRotationVector = Vector3.Lerp(_smoothRotationVector, dir, t);
        if (_smoothRotationVector.sqrMagnitude > EPS)
            transform.rotation = Quaternion.LookRotation(_smoothRotationVector);

        _lastPosition = newPos;
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