using GameFramework.Core;

/// <summary>
/// 单局流程相关事件
/// </summary>
public static class RoundEvents
{
    /// <summary>
    /// 进入单局事件
    /// </summary>
    public static readonly EventKey<RoundEnteredEventArgs> RoundEntered = new EventKey<RoundEnteredEventArgs>();

    /// <summary>
    /// 单局开始事件
    /// </summary>
    public static readonly EventKey<RoundStartedEventArgs> RoundStarted = new EventKey<RoundStartedEventArgs>();
}

/// <summary>
/// 进入单局事件参数
/// </summary>
public sealed class RoundEnteredEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext RoundContext { get; set; }
}

/// <summary>
/// 单局开始事件参数
/// </summary>
public sealed class RoundStartedEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext RoundContext { get; set; }
}


