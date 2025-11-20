using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using GameFramework.Core;

/// <summary>
/// 道具系统事件键
/// </summary>
public static class PropEvents
{
    /// <summary>
    /// 道具效果开始事件
    /// </summary>
    public static readonly EventKey<PropStartedEventArgs> PropStarted = new EventKey<PropStartedEventArgs>();

    /// <summary>
    /// 道具效果更新事件
    /// </summary>
    public static readonly EventKey<PropUpdatedEventArgs> PropUpdated = new EventKey<PropUpdatedEventArgs>();

    /// <summary>
    /// 道具效果结束事件
    /// </summary>
    public static readonly EventKey<PropEndedEventArgs> PropEnded = new EventKey<PropEndedEventArgs>();

    /// <summary>
    /// 道具使用被拒绝事件
    /// </summary>
    public static readonly EventKey<PropUseRejectedEventArgs> PropUseRejected = new EventKey<PropUseRejectedEventArgs>();
}

/// <summary>
/// 道具效果开始事件参数
/// </summary>
public sealed class PropStartedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 道具效果更新事件参数
/// </summary>
public sealed class PropUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }

    /// <summary>
    /// 剩余时间（秒）
    /// </summary>
    public float RemainingTime { get; set; }
}

/// <summary>
/// 道具效果结束事件参数
/// </summary>
public sealed class PropEndedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 道具使用被拒绝事件参数
/// </summary>
public sealed class PropUseRejectedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }

    /// <summary>
    /// 拒绝原因
    /// </summary>
    public string Reason { get; set; }
}

