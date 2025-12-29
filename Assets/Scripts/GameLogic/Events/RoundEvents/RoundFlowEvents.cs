using cfg.HuntingConfig.Enum;
using GameFramework.Core;

/// <summary>
/// 单局流程事件
/// </summary>
public static class RoundFlowEvents
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

    /// <summary>
    /// 切图开始事件
    /// </summary>
    public static readonly EventKey<RoundMapChangeEventArgs> RoundMapChangeStarted = new EventKey<RoundMapChangeEventArgs>();

    /// <summary>
    /// 切图完成事件
    /// </summary>
    public static readonly EventKey<RoundMapChangeEventArgs> RoundMapChangeFinished = new EventKey<RoundMapChangeEventArgs>();
}

/// <summary>
/// 单局开始事件参数
/// </summary>
public sealed class RoundStartedEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}

/// <summary>
/// 单局结束事件参数
/// </summary>
public sealed class RoundEndedEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}

/// <summary>
/// 切图事件参数
/// </summary>
public sealed class RoundMapChangeEventArgs : EventArgs
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext Context { get; set; }

    /// <summary>
    /// 目标地图类型
    /// </summary>
    public EMapType MapType { get; set; }
}
