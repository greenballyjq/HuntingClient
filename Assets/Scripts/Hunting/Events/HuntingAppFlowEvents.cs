using GameFramework.Core;
using Hunting.Round;

/// <summary>
/// 打猎应用流程相关事件
/// </summary>
public static class HuntingAppFlowEvents
{
    /// <summary>
    /// 进入准备事件
    /// </summary>
    public static readonly EventKey PrepareEntered = new EventKey();

    /// <summary>
    /// 离开准备事件
    /// </summary>
    public static readonly EventKey<PrepareExitedEventArgs> PrepareExited = new EventKey<PrepareExitedEventArgs>();
}

/// <summary>
/// 离开准备事件参数
/// </summary>
public sealed class PrepareExitedEventArgs : EventArgs
{
    /// <summary>
    /// 下局单局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}
