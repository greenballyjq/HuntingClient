using UnityEngine;
using RVO;

/// <summary>
/// 非侵入式避障组件
/// 挂载到任何会移动的对象上，自动提供避障支持
/// </summary>
[RequireComponent(typeof(Collider))]  // 需要Collider来确定半径
public class RVOAvoidance : MonoBehaviour
{
    [Header("避障参数")]
    [Tooltip("代理半径，影响避障距离")]
    public float radius = 0.5f;

    [Tooltip("最大速度限制，0表示不限制")]
    public float maxSpeed = 0f;

    [Tooltip("邻居检测距离")]
    public float neighborDistance = 15f;

    [Tooltip("是否启用避障")]
    public bool enableAvoidance = true;

    [Header("移动检测")]
    [Tooltip("移动检测方式")]
    public MovementDetectionMode detectionMode = MovementDetectionMode.Auto;

    [Header("旋转设置")]
    [Tooltip("是否自动旋转朝向移动方向")]
    public bool autoRotate = true;

    [Tooltip("旋转平滑速度，值越大旋转越快")]
    public float rotationSpeed = 10f;

    [Tooltip("只旋转Y轴（适合地面单位）")]
    public bool onlyRotateY = true;

    public enum MovementDetectionMode
    {
        Auto,           // 自动检测（优先Rigidbody）
        Rigidbody,      // 从Rigidbody读取速度
        Transform       // 从位置变化计算速度
    }

    // RVO代理ID
    private int agentId = -1;

    // 移动检测
    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 detectedVelocity;

    // RVO模拟器引用
    private Simulator sim;

    void Start()
    {
        // 获取模拟器
        sim = RVOManager.Instance.GetSimulator();

        // 尝试获取Rigidbody
        rb = GetComponent<Rigidbody>();

        // 自动检测半径
        if (radius <= 0)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                radius = col.bounds.extents.magnitude;
            }
            else
            {
                radius = 0.5f;
            }
        }

        // 添加到RVO模拟器
        Vector3 pos = transform.position;
        agentId = sim.addAgent(
            position: new RVO2Vector2(pos.x, pos.z),
            neighborDist: neighborDistance,
            maxNeighbors: 10,
            timeHorizon: 2f,
            timeHorizonObst: 2f,
            radius: radius,
            maxSpeed: maxSpeed > 0 ? maxSpeed : 999f,
            velocity: new RVO2Vector2(0, 0)
        );

        lastPosition = transform.position;
    }

    void Update()
    {
        if (!enableAvoidance || agentId < 0) return;

        // === 步骤1：检测当前移动意图 ===
        Vector3 desiredVelocity = DetectMovementIntention();

        // === 步骤2：同步当前位置到RVO ===
        // （因为其他脚本可能已经移动了对象）
        Vector3 currentPos = transform.position;
        sim.setAgentPosition(agentId, new RVO2Vector2(currentPos.x, currentPos.z));

        // === 步骤3：设置期望速度 ===
        sim.setAgentPrefVelocity(agentId, new RVO2Vector2(desiredVelocity.x, desiredVelocity.z));

        // === 步骤4：执行RVO模拟（管理器保证每帧只执行一次）===
        RVOManager.Instance.ExecuteSimulation(Time.deltaTime);

        // === 步骤5：获取避障后的速度 ===
        RVO2Vector2 avoidanceVel = sim.getAgentVelocity(agentId);
        Vector3 avoidanceVelocity = new Vector3(avoidanceVel.x(), 0, avoidanceVel.y());

        // === 步骤6：应用避障（修正移动）===
        ApplyAvoidance(desiredVelocity, avoidanceVelocity);
        ApplyRotation(avoidanceVelocity);

        // 记录位置用于下一帧
        lastPosition = transform.position;
    }

    /// <summary>
    /// 检测物体的移动意图
    /// </summary>
    private Vector3 DetectMovementIntention()
    {
        MovementDetectionMode mode = detectionMode;

        // 自动模式：优先使用Rigidbody
        if (mode == MovementDetectionMode.Auto)
        {
            mode = (rb != null) ? MovementDetectionMode.Rigidbody : MovementDetectionMode.Transform;
        }

        if (mode == MovementDetectionMode.Rigidbody && rb != null)
        {
            // 从Rigidbody读取速度
            return rb.velocity;
        }
        else
        {
            // 从位置变化计算速度
            Vector3 displacement = transform.position - lastPosition;
            return displacement / Time.deltaTime;
        }
    }

    /// <summary>
    /// 应用避障速度
    /// </summary>
    private void ApplyAvoidance(Vector3 originalVelocity, Vector3 avoidanceVelocity)
    {
        if (rb != null)
        {
            // 如果有Rigidbody，直接设置速度
            rb.velocity = new Vector3(avoidanceVelocity.x, rb.velocity.y, avoidanceVelocity.z);
        }
        else
        {
            // 如果没有Rigidbody，直接修正位置
            // 计算本帧应该移动的距离
            Vector3 originalMovement = originalVelocity * Time.deltaTime;
            Vector3 avoidanceMovement = avoidanceVelocity * Time.deltaTime;

            // 撤销原本的移动，应用避障后的移动
            Vector3 correction = avoidanceMovement - originalMovement;
            transform.position += correction;
        }
    }

    /// <summary>
    /// 根据避障速度自动旋转物体
    /// </summary>
    private void ApplyRotation(Vector3 velocity)
    {
        if (!autoRotate) return;

        // 速度太小不旋转（避免抖动）
        if (velocity.magnitude < 0.1f) return;

        if (onlyRotateY)
        {
            // 只旋转Y轴
            Vector3 lookDirection = new Vector3(velocity.x, 0, velocity.z);
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            // 完全朝向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void OnDestroy()
    {
        // 这里不能删除代理，因为RVO库没有提供删除功能
        // 实际使用中需要对象池或者在场景切换时重置整个模拟器
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        if (!enableAvoidance || agentId < 0) return;

        // 绘制避障半径
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // 绘制邻居检测范围
        Gizmos.color = new Color(1, 1, 0, 0.1f);
        Gizmos.DrawWireSphere(transform.position, neighborDistance);
    }
}