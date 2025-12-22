using GameFramework.Core;
using Hunting.Manager;
using Hunting.Round;

/// <summary>
/// 单局流程广播事件
/// </summary>
public static class RoundFlowEvents
{
    /// <summary>
    /// 单局开始事件
    /// </summary>
    public static readonly EventKey<RoundFlowStartedEventArgs> RoundFlowStarted = new EventKey<RoundFlowStartedEventArgs>();

    /// <summary>
    /// 单局暂停事件
    /// </summary>
    public static readonly EventKey RoundFlowPaused = new EventKey();

    /// <summary>
    /// 单局恢复事件
    /// </summary>
    public static readonly EventKey RoundFlowResumed = new EventKey();

    /// <summary>
    /// 单局结束事件
    /// </summary>
    public static readonly EventKey<RoundFlowEndedEventArgs> RoundFlowEnded = new EventKey<RoundFlowEndedEventArgs>();
}

/// <summary>
/// 单局开始事件参数
/// </summary>
public sealed class RoundFlowStartedEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}

/// <summary>
/// 单局结束事件参数
/// </summary>
public sealed class RoundFlowEndedEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}
