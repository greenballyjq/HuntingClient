using UnityEngine;
using RVO;

/// <summary>
/// 带有避障功能的移动组件
/// </summary>
public class RVOMovement : MonoBehaviour
{
    [Header("代理参数")]
    [Tooltip("邻居检测距离")]
    public float neighborDist = 5f;
    [Tooltip("最多考虑的邻居数量")]
    public int maxNeighbors = 5;
    [Tooltip("时间视界（预测与代理碰撞）")]
    public float timeHorizon = 10f;
    [Tooltip("障碍物时间视界（预测与障碍碰撞）")]
    public float timeHorizonObst = 10f;
    [Tooltip("代理半径")]
    public float radius = 0.8f;
    [Tooltip("最大速度")]
    public float maxSpeed = 2f;

    [Header("旋转参数")]
    [Tooltip("旋转平滑速度")]
    public float rotationSmoothSpeed = 5f;

    private int agentId = -1;
    private Vector3 targetDirection = Vector3.zero;
    private Vector3 lastPosition;
    private Vector3 smoothingVelocity;

    void Start()
    {
        RVOMovementManager.Instance.Register(this);

        Simulator sim = RVOMovementManager.Instance.GetSimulator();
        Vector3 pos = transform.position;

        agentId = sim.addAgent(
            new RVO2Vector2(pos.x, pos.z),
            neighborDist,
            maxNeighbors,
            timeHorizon,
            timeHorizonObst,
            radius,
            maxSpeed,
            new RVO2Vector2(0, 0)
        );

        sim.SetNumWorkers(0);

        lastPosition = transform.position;
        smoothingVelocity = transform.forward;
    }

    /// <summary>
    /// 第1步：同步位置到RVO并设置期望速度（Manager调用）
    /// </summary>
    public void UpdateRVO()
    {
        if (agentId < 0) return;

        Simulator sim = RVOMovementManager.Instance.GetSimulator();

        // 同步当前位置
        RVO2Vector2 pos = new RVO2Vector2(transform.position.x, transform.position.z);
        sim.setAgentPosition(agentId, pos);

        // 设置期望速度
        RVO2Vector2 prefVel = new RVO2Vector2(targetDirection.x, targetDirection.z);
        if (RVOMath.absSq(prefVel) > 0.01f)
        {
            prefVel = RVOMath.normalize(prefVel) * maxSpeed;
        }
        sim.setAgentPrefVelocity(agentId, prefVel);
    }

    /// <summary>
    /// 第3步：获取RVO计算后的位置并同步（Manager调用）
    /// </summary>
    public void SyncPosition()
    {
        if (agentId < 0) return;

        Simulator sim = RVOMovementManager.Instance.GetSimulator();

        // 获取RVO计算后的新位置
        RVO2Vector2 rvoPos = sim.getAgentPosition(agentId);
        Vector3 newPos = new Vector3(rvoPos.x(), transform.position.y, rvoPos.y());

        // 计算移动方向（用位置差）
        Vector3 moveDirection = newPos - lastPosition;

        // 平滑移动方向
        if (moveDirection.magnitude > 0.01f)
        {
            smoothingVelocity = Vector3.Lerp(smoothingVelocity, moveDirection, Time.deltaTime * rotationSmoothSpeed);
        }

        // 更新位置
        transform.position = newPos;

        // 根据平滑后的移动方向旋转
        if (smoothingVelocity.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(smoothingVelocity);
        }

        lastPosition = newPos;
    }

    /// <summary>
    /// 设置移动方向
    /// </summary>
    public void SetMoveDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            targetDirection = direction.normalized;
        }
        else
        {
            targetDirection = Vector3.zero;
        }
    }

    void OnDestroy()
    {
        if (RVOMovementManager.Instance != null)
        {
            RVOMovementManager.Instance.Unregister(this);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        // 目标方向辅助线（绿色）
        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + targetDirection * 2f);
            Gizmos.DrawSphere(pos + targetDirection * 2f, 0.1f);
        }

        // 当前移动方向（白色）
        if (Application.isPlaying && smoothingVelocity.magnitude > 0.01f)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos, pos + smoothingVelocity.normalized * 2f);
        }

        // 代理半径（绿球）
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(pos, radius);

        // 邻居检测距离（黄球）
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(pos, neighborDist);
    }
}