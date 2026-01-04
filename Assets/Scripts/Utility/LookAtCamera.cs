using UnityEngine;

/// <summary>
/// 始终朝向摄像机 - 让物体（如面片）始终面向摄像机
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    private Camera _targetCamera;

    private void Start()
    {
        _targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_targetCamera == null)
        {
            return;
        }

        Vector3 direction = _targetCamera.transform.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // 只绕XZ轴旋转，不绕Y轴（锁定Y轴旋转）
            Vector3 euler = targetRotation.eulerAngles;
            euler.y = transform.eulerAngles.y; // 保持当前Y轴角度不变
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}

