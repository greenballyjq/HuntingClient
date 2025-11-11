using GameFramework.Core;
using Hunting;
using Hunting.Manager;

/// <summary>
/// 单局流程事件键
/// </summary>
public static class RoundEvents
{
    /// <summary>
    /// 单局开始事件
    /// </summary>
    public static readonly EventKey<RoundStartedEventArgs> RoundStarted = new EventKey<RoundStartedEventArgs>();

    /// <summary>
    /// 单局暂停事件
    /// </summary>
    public static readonly EventKey RoundPaused = new EventKey();

    /// <summary>
    /// 单局恢复事件
    /// </summary>
    public static readonly EventKey RoundResumed = new EventKey();

    /// <summary>
    /// 单局结束事件
    /// </summary>
    public static readonly EventKey<RoundEndedEventArgs> RoundEnded = new EventKey<RoundEndedEventArgs>();
}

/// <summary>
/// 单局开始事件参数
/// </summary>
public sealed class RoundStartedEventArgs : EventArgs
{
    /// <summary>
    /// 本局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}

/// <summary>
/// 单局结束事件参数
/// </summary>
public sealed class RoundEndedEventArgs : EventArgs
{
    /// <summary>
    /// 本局上下文
    /// </summary>
    public RoundContext Context { get; set; }

    /// <summary>
    /// 是否正常完成
    /// </summary>
    public bool IsCompleted { get; set; }
}

