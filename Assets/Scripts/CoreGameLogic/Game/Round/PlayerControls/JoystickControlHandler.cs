/// <summary>
/// 摇杆控制处理器
/// </summary>
public class JoystickControlHandler : BasePlayerControlHandler
{
    /// <summary>
    /// 旋转速度（度/秒）
    /// </summary>
    private const float ROTATION_SPEED = 80f;

    /// <summary>
    /// 摇杆组件
    /// </summary>
    private UIComponentJoystick _joystick;

    /// <summary>
    /// 玩家武器
    /// </summary>
    private PlayerWeapon _playerWeapon;

    protected override void OnControlStart()
    {
        _playerWeapon = GameServiceLocator.GetRoundManager<WeaponManager>().PlayerWeapon;
        _joystick = UIComponentJoystick.Joystick;
    }

    protected override void OnControlUpdate(float deltaTime)
    {
        if (!_joystick.IsActive)
            return;

        float deltaAngle = _joystick.HorizontalInput * ROTATION_SPEED * deltaTime;
        _playerWeapon.RotateWeapon(deltaAngle);
        _playerWeapon.TryFire();
    }

    protected override void OnControlEnd(){}
}
