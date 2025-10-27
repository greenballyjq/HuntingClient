using UnityEngine;
using RVO;


[ExecuteInEditMode]  // 添加这一行
public class RVO2Avoidance : MonoBehaviour
{
    [Header("RVO2参数")]
    public float radius = 1.5f;
    public float maxSpeed = 2.0f;
    public float neighborDist = 15.0f;
    public int maxNeighbors = 10;
    public float timeHorizon = 10.0f;
    public float avoidanceInfluence = 0.5f;  // 避障影响权重

    public int agentId { get; private set; } = -1;

    private static bool hasInitialized = false;
    private static int frameCount = -1;

    private SimpleMovement movement;
    private UnityEngine.Vector3 lastPosition;

    void Start()
    {
        movement = GetComponent<SimpleMovement>();

        RVO.Vector2 startPos = new RVO.Vector2(transform.position.x, transform.position.z);

        agentId = Simulator.Instance.addAgent(
            startPos,
            neighborDist,
            maxNeighbors,
            timeHorizon,
            timeHorizon,
            radius,
            maxSpeed,
            new RVO.Vector2(0, 0)
        );

        if (!hasInitialized)
        {
            Simulator.Instance.setTimeStep(0.25f);
            hasInitialized = true;
        }

        lastPosition = transform.position;
        Debug.Log($"{name} AgentId: {agentId}");
    }

    void Update()
    {
        if (agentId < 0) return;

        // 1. 同步位置到RVO2
        RVO.Vector2 pos = new RVO.Vector2(transform.position.x, transform.position.z);
        Simulator.Instance.setAgentPosition(agentId, pos);

        // 2. 设置偏好速度（从SimpleMovement获取）
        UnityEngine.Vector3 desiredDir = movement.targetDirection;
        RVO.Vector2 prefVel = new RVO.Vector2(desiredDir.x, desiredDir.z);
        if (RVOMath.absSq(prefVel) > 0.01f)
        {
            prefVel = RVOMath.normalize(prefVel) * maxSpeed;
        }
        Simulator.Instance.setAgentPrefVelocity(agentId, prefVel);
    }

    void LateUpdate()
    {
        if (agentId < 0) return;

        // 执行RVO计算（每帧只调用一次）
        if (frameCount != Time.frameCount)
        {
            Simulator.Instance.doStep();
            frameCount = Time.frameCount;
        }

        // 获取RVO计算后的避障方向
        RVO.Vector2 rvoVel = Simulator.Instance.getAgentVelocity(agentId);
        UnityEngine.Vector3 rvoDirection = new UnityEngine.Vector3(rvoVel.x(), 0, rvoVel.y()).normalized;

        // 获取SimpleMovement的期望方向
        UnityEngine.Vector3 desiredDir = movement.targetDirection.normalized;

        // 混合方向：根据权重调整
        UnityEngine.Vector3 blendedDirection = UnityEngine.Vector3.Lerp(desiredDir, rvoDirection, avoidanceInfluence);

        // 修改SimpleMovement的目标方向
        movement.targetDirection = blendedDirection * movement.speed;

        lastPosition = transform.position;
    }

    void OnDrawGizmos()
    {
        // 黄色球：自身体积
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // 蓝色大圆：预测范围
        Gizmos.color = new Color(0, 0, 1, 0.2f);
        Gizmos.DrawWireSphere(transform.position, neighborDist);

        // 白色线：RVO计算的避障方向
        if (agentId >= 0)
        {
            RVO.Vector2 vel = Simulator.Instance.getAgentVelocity(agentId);
            if (RVOMath.absSq(vel) > 0.01f)
            {
                Gizmos.color = Color.white;
                UnityEngine.Vector3 vel3D = new UnityEngine.Vector3(vel.x(), 0, vel.y());
                Gizmos.DrawRay(transform.position, vel3D.normalized * 2f);
            }
        }

        // 绿色线：目标方向（SimpleMovement的targetDirection）
        if (movement != null && movement.targetDirection.magnitude > 0.01f)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, movement.targetDirection.normalized * 2f);
        }
    }

    void OnDestroy()
    {
        // 清理
    }
}