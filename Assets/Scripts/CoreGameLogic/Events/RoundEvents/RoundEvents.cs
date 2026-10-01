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

    /// <summary>
    /// 时间更新事件
    /// </summary>
    public static readonly EventKey<TimeUpdatedEventArgs> TimeUpdated = new EventKey<TimeUpdatedEventArgs>();
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

/// <summary>
/// 时间更新事件参数
/// </summary>
public sealed class TimeUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// 当前秒数
    /// </summary>
    public float Seconds { get; set; }

    /// <summary>
    /// 是否为倒计时模式
    /// </summary>
    public bool IsCountdown { get; set; }
}


