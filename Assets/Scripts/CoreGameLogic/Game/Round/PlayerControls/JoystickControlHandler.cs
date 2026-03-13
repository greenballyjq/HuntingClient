using UnityEngine;


/// <summary>
/// 摇杆控制处理器
/// </summary>
public class JoystickControlHandler : BaseControlHandler
{
    private const float MIN_YAW_ANGLE = -80f;
    private const float MAX_YAW_ANGLE = 80f;
    private const float ROTATION_SPEED = 60f;

    private UIComponentJoystick _joystick;

    protected override void OnInit() { }

    protected override void OnControlStart()
    {
        _joystick = UIComponentJoystick.Joystick;
    }

    protected override void OnControlUpdate(float dt)
    {
        if (!_joystick.IsActive)
            return;

        float deltaAngle = _joystick.HorizontalInput * ROTATION_SPEED * dt;

        Vector3 forwardXZ = new Vector3(PlayerWeapon.transform.forward.x, 0f, PlayerWeapon.transform.forward.z);
        if (forwardXZ.sqrMagnitude < 0.0001f)
            return;

        float currentAngle = Vector3.SignedAngle(Vector3.forward, forwardXZ.normalized, Vector3.up);
        float newAngle = Mathf.Clamp(currentAngle + deltaAngle, MIN_YAW_ANGLE, MAX_YAW_ANGLE);
        Vector3 direction = Quaternion.Euler(0f, newAngle, 0f) * Vector3.forward;

        PlayerWeapon.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        PlayerWeapon.TryFire();
    }

    protected override void OnControlEnd() { }
}
