using GameFramework.Core;

/// <summary>
/// 任务系统事件键
/// </summary>
public static class QuestEvents
{
    /// <summary>
    /// 动态任务完成事件
    /// </summary>
    public static readonly EventKey<QuestCompletedEventArgs> QuestCompleted = new EventKey<QuestCompletedEventArgs>();
}

/// <summary>
/// 动态任务完成事件参数
/// </summary>
public sealed class QuestCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 奖励金币数量
    /// </summary>
    public int RewardCoin { get; set; }
}

