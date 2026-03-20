using UnityEngine;

/// <summary>
/// 瞄准器控制器
/// </summary>
public class AimAssistController : MonoBehaviour
{
    /// <summary>
    /// 跟随速度
    /// </summary>
    [SerializeField] private float _followSpeed = 10f;

    private Collider _targetCollider;

    /// <summary>
    /// 屏幕中心点世界坐标
    /// </summary>
    private Vector3 _screenCenterWorldPos;

    private CameraManager _cameraManager;

    private const float CAMERA_DEPTH = 3f;

    private void Awake()
    {
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();
        _screenCenterWorldPos = _cameraManager.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f), CAMERA_DEPTH);
        Debug.LogWarning(_screenCenterWorldPos);
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    public void SetTarget(Transform target)
    {
        _targetCollider = target?.GetComponent<Collider>();
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void UpdatePosition(float dt)
    {
        Vector3 targetPos;

        if (_targetCollider != null)
            targetPos = _targetCollider.bounds.center;
        else
        {
            targetPos = _screenCenterWorldPos;
            Debug.LogWarning(_screenCenterWorldPos);
        }
            

        transform.position = Vector3.Lerp(transform.position, targetPos, dt * _followSpeed);
    }
}

