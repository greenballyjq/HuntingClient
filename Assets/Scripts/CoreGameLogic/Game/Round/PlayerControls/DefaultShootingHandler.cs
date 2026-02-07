using UnityEngine;

/// <summary>
/// 默认射击控制处理器
/// </summary>
public class DefaultShootingHandler : IPlayerControlHandler
{
    /// <summary>
    /// 主武器
    /// </summary>
    private PlayerWeapon _weapon;

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
    /// 瞄准深度
    /// </summary>
    private const float AimDepth = 6f;

    public void OnControlStart()
    {
        Input.SwitchToFireMode();
        _weapon = _weaponManager.PlayerWeapon;
    }

    public void OnControlUpdate(float deltaTime)
    {
        if (Input.IsFireHeld)
        {
            Vector3 worldPos = _cameraManager.ScreenToWorldPoint(Input.PointerScreenPosition, AimDepth);
            _weapon.SetAimTarget(worldPos);
            _weapon.Fire();
        }
            
    }

    public void OnControlEnd()
    {

    }
}