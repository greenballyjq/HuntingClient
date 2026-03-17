using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 任务相关事件
/// </summary>
public static class QuestEvents
{
    /// <summary>
    /// 任务派发事件
    /// </summary>
    public static readonly EventKey<QuestDispatchedEventArgs> QuestDispatched = new EventKey<QuestDispatchedEventArgs>();

    /// <summary>
    /// 任务进度更新事件
    /// </summary>
    public static readonly EventKey<QuestProgressUpdatedEventArgs> QuestProgressUpdated = new EventKey<QuestProgressUpdatedEventArgs>();

    /// <summary>
    /// 任务完成事件
    /// </summary>
    public static readonly EventKey<QuestCompletedEventArgs> QuestCompleted = new EventKey<QuestCompletedEventArgs>();

    /// <summary>
    /// 任务超时事件
    /// </summary>
    public static readonly EventKey QuestTimeout = new EventKey();
}

/// <summary>
/// 任务派发事件参数
/// </summary>
public sealed class QuestDispatchedEventArgs : EventArgs
{
    /// <summary>
    /// 任务描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 目标值
    /// </summary>
    public int TargetValue { get; set; }

    /// <summary>
    /// 奖励值
    /// </summary>
    public int RewardValue { get; set; }

    /// <summary>
    /// 持续时间（秒）
    /// </summary>
    public float Duration { get; set; }
}

/// <summary>
/// 任务进度更新事件参数
/// </summary>
public sealed class QuestProgressUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// 当前进度
    /// </summary>
    public int CurrentProgress { get; set; }

    /// <summary>
    /// 剩余时间（秒）
    /// </summary>
    public float RemainingTime { get; set; }
}

/// <summary>
/// 任务完成事件参数
/// </summary>
public sealed class QuestCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 奖励值
    /// </summary>
    public int RewardValue { get; set; }
}