using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 单局流程相关事件
/// </summary>
public static class RoundEvents
{
    /// <summary>
    /// 单局开始事件
    /// </summary>
    public static readonly EventKey<RoundStartedEventArgs> RoundStarted = new EventKey<RoundStartedEventArgs>();
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
