/// <summary>
/// 控制类型枚举
/// </summary>
public enum EControlType
{
    /// <summary>
    /// 默认射击
    /// </summary>
    DefaultShooting,

    /// <summary>
    /// 指哪打哪
    /// </summary>
    AimAssist
}

/// <summary>
/// 玩家控制处理器工厂
/// </summary>
public static class PlayerControlHandlerFactory
{
    /// <summary>
    /// 创建控制处理器
    /// </summary>
    /// <param name="type">控制类型</param>
    /// <returns>控制处理器实例</returns>
    public static IPlayerControlHandler CreatePlayerControlHandler(EControlType type)
    {
        switch (type)
        {
            case EControlType.DefaultShooting:
                return new DefaultShootingHandler();
            case EControlType.AimAssist:
                return new AimAssistHandler();
            default:
                return null;
        }
    }
}