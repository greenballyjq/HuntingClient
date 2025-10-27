using UnityEngine;

public class SimpleMovementPlus : MonoBehaviour
{
    [Header("移动配置")]
    public Vector3 targetDirection;
    public float speed = 2.0f;
    public float rotationSpeed = 5.0f;

    private RVO2AvoidancePlus avoidance;
    private Vector3 blendedDirection;

    void Start()
    {
        avoidance = GetComponent<RVO2AvoidancePlus>();
    }

    void Update()
    {
        // 设置期望方向并获取避障修正
        if (avoidance != null)
        {
            avoidance.SetDesiredDirection(targetDirection);
            blendedDirection = avoidance.GetBlendedDirection(targetDirection);
        }
        else
        {
            blendedDirection = targetDirection;
        }

        // 应用移动和旋转
        transform.position += blendedDirection.normalized * speed * Time.deltaTime;

        if (blendedDirection.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(blendedDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}