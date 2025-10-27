using UnityEngine;
using RVO;

[RequireComponent(typeof(SpriteRenderer))]
public class RVO2DAgent : MonoBehaviour
{
    [Header("“∆∂Ø≈‰÷√")]
    public UnityEngine.Vector2 targetDirection = UnityEngine.Vector2.up;
    public float speed = 3.0f;

    [Header("RVO2≤Œ ˝")]
    public float radius = 0.5f;
    public float maxSpeed = 10.0f;
    public float neighborDist = 10.0f;
    public int maxNeighbors = 5;
    public float timeHorizon = 2.0f;

    private int agentId = -1;
    private UnityEngine.Vector2 lastPosition;
    private UnityEngine.Vector2 smoothingVelocity;

    void Start()
    {
        RVO.Vector2 startPos = new RVO.Vector2(transform.position.x, transform.position.y);

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

        lastPosition = transform.position;
        smoothingVelocity = UnityEngine.Vector2.up;

        Debug.Log($"{name} AgentId: {agentId}");
    }

    public void UpdateRVO()
    {
        if (agentId < 0) return;

        RVO.Vector2 pos = new RVO.Vector2(transform.position.x, transform.position.y);
        Simulator.Instance.setAgentPosition(agentId, pos);

        RVO.Vector2 prefVel = new RVO.Vector2(targetDirection.x, targetDirection.y);
        if (RVOMath.absSq(prefVel) > 0.01f)
        {
            prefVel = RVOMath.normalize(prefVel) * speed;
        }

        Simulator.Instance.setAgentPrefVelocity(agentId, prefVel);
    }

    public void SyncPosition()
    {
        if (agentId < 0) return;

        RVO.Vector2 rvoPos = Simulator.Instance.getAgentPosition(agentId);
        RVO.Vector2 rvoVel = Simulator.Instance.getAgentVelocity(agentId);

        UnityEngine.Vector2 newPos = new UnityEngine.Vector2(rvoPos.x(), rvoPos.y());

        UnityEngine.Vector2 rvoDirection = new UnityEngine.Vector2(rvoVel.x(), rvoVel.y()).normalized;

        UnityEngine.Vector2 desiredDir = targetDirection.normalized;
        UnityEngine.Vector2 finalDirection = desiredDir;

        if (RVOMath.absSq(rvoVel) > 0.1f)
        {
            finalDirection = UnityEngine.Vector2.Lerp(desiredDir, rvoDirection, 0.7f).normalized;
        }

        UnityEngine.Vector2 movement = finalDirection * speed * Time.deltaTime;
        transform.position += new UnityEngine.Vector3(movement.x, movement.y, 0);

        if (movement.magnitude > 0.01f)
        {
            smoothingVelocity = UnityEngine.Vector2.Lerp(smoothingVelocity, movement, Time.deltaTime * 8f);
            float angle = Mathf.Atan2(smoothingVelocity.y, smoothingVelocity.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 8f);
        }

        lastPosition = transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.white;
        UnityEngine.Vector3 dir = new UnityEngine.Vector3(smoothingVelocity.x, smoothingVelocity.y, 0).normalized;
        Gizmos.DrawRay(transform.position, dir * 2f);

        Gizmos.color = Color.green;
        UnityEngine.Vector3 targetDir = new UnityEngine.Vector3(targetDirection.x, targetDirection.y, 0);
        Gizmos.DrawRay(transform.position, targetDir * 2f);
    }
}