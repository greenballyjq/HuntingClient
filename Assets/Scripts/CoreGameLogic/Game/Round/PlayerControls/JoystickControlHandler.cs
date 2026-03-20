using UnityEngine;


/// <summary>
/// 摇杆控制处理器
/// </summary>
public class JoystickControlHandler : BaseControlHandler
{
    /// <summary>
    /// 摇杆组件
    /// </summary>
    private UIComponentJoystick _joystick;

    /// <summary>
    /// 屏幕滑动满量程像素数
    /// </summary>
    private float _screenSwipeFullRange;

    /// <summary>
    /// 上一帧是否为屏幕滑动激活状态
    /// </summary>
    private bool _wasScreenSwipeActive;

    /// <summary>
    /// 平台系统信息
    /// </summary>
    private SystemInfo _systemInfo;

    private PlatformManager _platformManager;

    private const float MIN_YAW_ANGLE = -70f;
    private const float MAX_YAW_ANGLE = 70f;
    private const float ROTATION_SMOOTH_SPEED = 16f;
    private const float SCREEN_SWIPE_RATIO = 0.25f;

    protected override void OnInit() { }

    protected override void OnControlStart()
    {
        InputManager.SwitchToFireMode();
        _joystick = UIComponentJoystick.Joystick;
        _platformManager = GameServiceLocator.PlatformManager;
        _systemInfo = _platformManager.CurrentPlatform.GetSystemInfo();
        RefreshScreenSwipeFullRange();
    }

    protected override void OnControlUpdate(float dt)
    {
        UpdateScreenSwipeInput();

        float targetAngle = Mathf.Lerp(MIN_YAW_ANGLE, MAX_YAW_ANGLE, (_joystick.HorizontalInput + 1f) * 0.5f);

        Vector3 forwardXZ = new Vector3(PlayerWeapon.transform.forward.x, 0f, PlayerWeapon.transform.forward.z);
        float currentAngle = forwardXZ.sqrMagnitude >= 0.0001f
            ? Vector3.SignedAngle(Vector3.forward, forwardXZ.normalized, Vector3.up)
            : 0f;

        float smoothFactor = 1f - Mathf.Exp(-ROTATION_SMOOTH_SPEED * dt);
        float newAngle = Mathf.Lerp(currentAngle, targetAngle, smoothFactor);
        Vector3 direction = Quaternion.Euler(0f, newAngle, 0f) * Vector3.forward;

        PlayerWeapon.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        if (_joystick.IsActive)
            PlayerWeapon.TryFire();
    }

    protected override void OnControlEnd() { }

    #region 私有方法
    /// <summary>
    /// 刷新屏幕滑动满量程
    /// </summary>
    private void RefreshScreenSwipeFullRange()
    {
        float screenWidth = _systemInfo != null ? _systemInfo.ScreenWidth : Screen.width;
        _screenSwipeFullRange = screenWidth * SCREEN_SWIPE_RATIO;
    }

    /// <summary>
    /// 更新屏幕滑动输入
    /// </summary>
    private void UpdateScreenSwipeInput()
    {
        bool isScreenSwipeActive = InputManager.IsFireHeld;

        if (isScreenSwipeActive)
        {
            float deltaX = InputManager.PointerScreenPosition.x - InputManager.PointerPressStartPosition.x;
            float horizontalInput = Mathf.Clamp(deltaX / _screenSwipeFullRange, -1f, 1f);
            _joystick.ApplyExternalInput(horizontalInput, true);
        }
        else if (_wasScreenSwipeActive)
        {
            _joystick.ApplyExternalInput(0f, false);
        }

        _wasScreenSwipeActive = isScreenSwipeActive;
    }
    #endregion
}
