using UnityEngine;

public class SmoothAvoidanceImproved : MonoBehaviour
{
    public float speed = 3f;
    public float avoidDistance = 2f;
    public float rotationSpeed = 3f;
    public float sideRayAngle = 30f;

    private Rigidbody rb;
    private Quaternion targetRotation;
    private Vector3 originalForward;
    private bool hasRecordedOriginalDirection = false;
    private bool isAvoiding = false; // 新增：避障状态
    private float lastDecisionTime = 0f; // 决策冷却

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;

        if (!hasRecordedOriginalDirection)
        {
            originalForward = transform.forward;
            hasRecordedOriginalDirection = true;
        }
    }

    void Update()
    {
        if (!hasRecordedOriginalDirection)
        {
            originalForward = transform.forward;
            hasRecordedOriginalDirection = true;
        }

        // 三条射线检测
        bool hitLeft = Physics.Raycast(transform.position, GetRayDirection(-sideRayAngle), avoidDistance);
        bool hitCenter = Physics.Raycast(transform.position, transform.forward, avoidDistance);
        bool hitRight = Physics.Raycast(transform.position, GetRayDirection(sideRayAngle), avoidDistance);

        // 调试射线
        Debug.DrawRay(transform.position, GetRayDirection(-sideRayAngle) * avoidDistance, hitLeft ? Color.red : Color.green);
        Debug.DrawRay(transform.position, transform.forward * avoidDistance, hitCenter ? Color.red : Color.green);
        Debug.DrawRay(transform.position, GetRayDirection(sideRayAngle) * avoidDistance, hitRight ? Color.red : Color.green);

        // 简化决策逻辑：优先级明确
        if (hitCenter && !isAvoiding && Time.time - lastDecisionTime > 0.5f) // 决策冷却
        {
            isAvoiding = true;
            lastDecisionTime = Time.time;

            // 明确的选择逻辑：左优先 > 右优先 > 紧急转向
            if (!hitLeft)
            {
                // 左边畅通，坚决左转
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 45f, 0);
                Debug.Log("决策：向左转");
            }
            else if (!hitRight)
            {
                // 右边畅通，坚决右转
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 45f, 0);
                Debug.Log("决策：向右转");
            }
            else
            {
                // 两侧都堵，坚决大角度转向
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90f, 0);
                Debug.Log("决策：紧急转向");
            }
        }
        else if (!hitCenter && !hitLeft && !hitRight && isAvoiding)
        {
            // 前方完全畅通，结束避障
            isAvoiding = false;
            targetRotation = Quaternion.LookRotation(originalForward);
            Debug.Log("结束避障，回归原始方向");
        }

        // 应用旋转和移动
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    Vector3 GetRayDirection(float angle)
    {
        return Quaternion.Euler(0, angle, 0) * transform.forward;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 碰撞时强制决策
        Vector3 collisionDirection = collision.transform.position - transform.position;
        float crossProduct = Vector3.Cross(transform.forward, collisionDirection).y;
        float avoidDirection = crossProduct > 0 ? -1f : 1f;

        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90f * avoidDirection, 0);
        isAvoiding = true;
        lastDecisionTime = Time.time;
        Debug.Log("碰撞！强制转向");
    }
}