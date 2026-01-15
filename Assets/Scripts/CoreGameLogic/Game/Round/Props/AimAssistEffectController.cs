using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 指哪打哪特效控制器
/// </summary>
public class AimAssistEffectController : MonoBehaviour
{
    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField] private float _moveSpeed = 15f;

    /// <summary>
    /// 当前目标
    /// </summary>
    private Transform _target;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    /// <summary>
    /// 设置目标
    /// </summary>
    /// <param name="target">目标</param>
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void UpdatePosition(float dt)
    {
        Vector3 targetPos;

        // 有目标时在目标位置显示，无目标时在屏幕中心显示
        if (_target != null && _target.gameObject != null)
            targetPos = _target.GetComponent<Collider>().bounds.center;
        else
            targetPos = _cameraManager.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f),3f);

        // 平滑移动
        transform.position = Vector3.Lerp(transform.position, targetPos, dt * _moveSpeed);
    }
}

