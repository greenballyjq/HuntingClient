/// <summary>
/// 控制类型
/// </summary>
public enum EControlType
{
    /// <summary>
    /// 默认射击控制
    /// </summary>
    DefaultShooting,

    /// <summary>
    /// 瞄准辅助控制
    /// </summary>
    AimAssist,

    /// <summary>
    /// 摇杆控制
    /// </summary>
    Joystick
}

/// <summary>
/// 控制处理器工厂
/// </summary>
public static class ControlHandlerFactory
{
    /// <summary>
    /// 创建控制处理器
    /// </summary>
    /// <param name="type">控制类型</param>
    /// <returns>控制处理器实例</returns>
    public static BaseControlHandler CreatePlayerControlHandler(EControlType type)
    {
        switch (type)
        {
            case EControlType.DefaultShooting:
                return new DefaultShootingControlHandler();
            case EControlType.AimAssist:
                return new AimAssistControlHandler();
            case EControlType.Joystick:
                return new JoystickControlHandler();
            default:
                return null;
        }
    }
}