using UnityEngine;

/// <summary>
/// 默认射击控制处理器
/// </summary>
public class DefaultShootingHandler : BasePlayerControlHandler
{
    /// <summary>
    /// 玩家武器
    /// </summary>
    private PlayerWeapon _playerWeapon;

    /// <summary>
    /// 输入管理器
    /// </summary>
    private InputManager Input => GameServiceLocator.GetAppManager<InputManager>();

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    /// <summary>
    /// 瞄准平面高度（射线与该水平面的交点作为瞄准点）
    /// </summary>
    private const float AimPlaneHeight = 0f;

    protected override void OnControlStart()
    {
        Input.SwitchToFireMode();
        _playerWeapon = _weaponManager.PlayerWeapon;
    }

    protected override void OnControlUpdate(float deltaTime)
    {
        if (Input.IsFireHeld)
        {
            Ray ray = _cameraManager.MainCamera.ScreenPointToRay(Input.PointerScreenPosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, AimPlaneHeight, 0f));
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter);
                _playerWeapon.SetAimTarget(worldPos);
            }
            _playerWeapon.TryFire();
        }
            
    }

    protected override void OnControlEnd() { }
}