using UnityEngine;
using RVO;

[ExecuteInEditMode]
public class RVO2AvoidancePlus : MonoBehaviour
{
    [Header("RVO2参数")]
    public float radius = 1.5f;
    public float maxSpeed = 2.0f;
    public float neighborDist = 15.0f;
    public int maxNeighbors = 10;
    public float timeHorizon = 10.0f;
    public float avoidanceInfluence = 0.5f;

    public int agentId { get; private set; } = -1;
    private static bool hasInitialized = false;
    private static int frameCount = -1;
    private UnityEngine.Vector3 desiredDirection;  // 存储期望方向

    void Start()
    {
        RVO.Vector2 startPos = new RVO.Vector2(transform.position.x, transform.position.z);
        agentId = Simulator.Instance.addAgent(startPos, neighborDist, maxNeighbors, timeHorizon, timeHorizon, radius, maxSpeed, new RVO.Vector2(0, 0));

        if (!hasInitialized)
        {
            Simulator.Instance.setTimeStep(0.25f);
            hasInitialized = true;
        }

        Debug.Log($"{name} AgentId: {agentId}");
    }

    void Update()
    {
        if (agentId < 0) return;

        // 1. 同步位置
        RVO.Vector2 pos = new RVO.Vector2(transform.position.x, transform.position.z);
        Simulator.Instance.setAgentPosition(agentId, pos);

        // 2. 设置偏好速度
        RVO.Vector2 prefVel = new RVO.Vector2(desiredDirection.x, desiredDirection.z);
        if (RVOMath.absSq(prefVel) > 0.01f)
        {
            prefVel = RVOMath.normalize(prefVel) * maxSpeed;
        }
        Simulator.Instance.setAgentPrefVelocity(agentId, prefVel);
    }

    void LateUpdate()
    {
        if (agentId < 0) return;

        // 执行RVO计算
        if (frameCount != Time.frameCount)
        {
            Simulator.Instance.doStep();
            frameCount = Time.frameCount;
        }
    }

    // 设置期望方向（由移动组件调用）
    public void SetDesiredDirection(UnityEngine.Vector3 direction)
    {
        desiredDirection = direction;
    }

    // 获取混合后的方向（由移动组件的LateUpdate调用）
    public UnityEngine.Vector3 GetBlendedDirection(UnityEngine.Vector3 currentDirection)
    {
        if (agentId < 0) return currentDirection;

        RVO.Vector2 rvoVel = Simulator.Instance.getAgentVelocity(agentId);
        UnityEngine.Vector3 rvoDirection = new UnityEngine.Vector3(rvoVel.x(), 0, rvoVel.y()).normalized;

        UnityEngine.Vector3 desiredDir = currentDirection.normalized;
        UnityEngine.Vector3 blendedDir = UnityEngine.Vector3.Lerp(desiredDir, rvoDirection, avoidanceInfluence);

        return blendedDir.normalized * UnityEngine.Vector3.Distance(UnityEngine.Vector3.zero, currentDirection);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = new Color(0, 0, 1, 0.2f);
        Gizmos.DrawWireSphere(transform.position, neighborDist);

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

        if (desiredDirection.magnitude > 0.01f)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, desiredDirection.normalized * 2f);
        }
    }
}