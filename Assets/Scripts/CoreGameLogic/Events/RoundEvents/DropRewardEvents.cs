using cfg.HuntingConfig.Enum;
using GameFramework.Core;


/// <summary>
/// 掉落奖励事件
/// </summary>
public static class DropRewardEvents
{
    /// <summary>
    /// 掉落奖励到达事件
    /// </summary>
    public static readonly EventKey<RewardArrivedEventArgs> DropRewardArrived = new EventKey<RewardArrivedEventArgs>();
}

/// <summary>
/// 掉落奖励到达事件参数
/// </summary>
public sealed class RewardArrivedEventArgs : EventArgs
{
    public EDropType DropType { get; set; }
    public int DropCount { get; set; }
}