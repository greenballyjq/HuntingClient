using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [Header("移动配置")]
    public UnityEngine.Vector3 targetDirection = UnityEngine.Vector3.forward;
    public float speed = 2.0f;
    public float rotationSpeed = 5.0f;

    private UnityEngine.Vector3 lastFramePosition;

    void Start()
    {
        lastFramePosition = transform.position;
    }

    void Update()
    {
        // 直接移动
        transform.position += targetDirection.normalized * speed * Time.deltaTime;

        // 旋转朝向移动方向
        if (targetDirection.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        lastFramePosition = transform.position;
    }
}