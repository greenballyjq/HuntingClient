using GameFramework.Core;

/// <summary>
/// 单局流程事件键
/// </summary>
public static class RoundEvents
{
    /// <summary>
    /// 单局开始事件
    /// </summary>
    public static readonly EventKey RoundStarted = new EventKey();

    /// <summary>
    /// 单局结束事件
    /// </summary>
    public static readonly EventKey RoundEnded = new EventKey();
}

