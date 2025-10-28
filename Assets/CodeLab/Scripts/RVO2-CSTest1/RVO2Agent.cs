using UnityEngine;
using RVO;

public class RVO2Agent : MonoBehaviour
{
    [Header("移动配置")]
    public Vector3 targetDirection = Vector3.forward;
    public float speed = 2.0f;

    [Header("RVO2参数")]
    public float radius = 1.5f;
    public float maxSpeed = 2.0f;
    public float neighborDist = 15.0f;
    public int maxNeighbors = 10;
    public float timeHorizon = 10.0f;

    private int agentId = -1;
    private Vector3 lastPosition;
    private Vector3 smoothingVelocity;

    void Start()
    {
        RVO.RVO2Vector2 startPos = new RVO.RVO2Vector2(transform.position.x, transform.position.z);

        agentId = Simulator.Instance.addAgent(
            startPos,
            neighborDist,
            maxNeighbors,
            timeHorizon,
            timeHorizon,
            radius,
            maxSpeed,
            new RVO.RVO2Vector2(0, 0)
        );

        lastPosition = transform.position;
        smoothingVelocity = transform.forward;

        Debug.Log($"{name} AgentId: {agentId}");
    }

    public void UpdateRVO()
    {
        if (agentId < 0) return;

        // 同步位置
        RVO.RVO2Vector2 pos = new RVO.RVO2Vector2(transform.position.x, transform.position.z);
        Simulator.Instance.setAgentPosition(agentId, pos);

        // 设置偏好速度
        RVO.RVO2Vector2 prefVel = new RVO.RVO2Vector2(targetDirection.x, targetDirection.z);
        if (RVOMath.absSq(prefVel) > 0.01f)
        {
            prefVel = RVOMath.normalize(prefVel) * speed;
        }
        Simulator.Instance.setAgentPrefVelocity(agentId, prefVel);
    }

    public void SyncPosition()
    {
        if (agentId < 0) return;

        // 获取新位置
        RVO.RVO2Vector2 rvoPos = Simulator.Instance.getAgentPosition(agentId);
        Vector3 newPos = new Vector3(rvoPos.x(), transform.position.y, rvoPos.y());

        // 计算移动方向（用位置差，不要用velocity）
        Vector3 moveDirection = newPos - lastPosition;

        // 平滑移动方向
        if (moveDirection.magnitude > 0.01f)
        {
            smoothingVelocity = Vector3.Lerp(smoothingVelocity, moveDirection, Time.deltaTime * 5f);
        }

        // 更新位置
        transform.position = newPos;

        // 根据移动方向旋转（这是关键！）
        if (smoothingVelocity.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(smoothingVelocity);
        }

        lastPosition = newPos;
    }

    void OnDrawGizmos()
    {
        // 黄色球：碰撞半径
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // 白色线：当前移动方向
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, smoothingVelocity * 2f);

        // 绿色箭头：目标方向
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetDirection * 2f);
    }
}