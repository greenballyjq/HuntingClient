using cfg.HuntingConfig;
using GameFramework.Core;

/// <summary>
/// 任务系统事件键
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
    public static readonly EventKey<QuestTimeoutEventArgs> QuestTimeout = new EventKey<QuestTimeoutEventArgs>();
}

/// <summary>
/// 任务派发事件参数
/// </summary>
public sealed class QuestDispatchedEventArgs : EventArgs
{
    /// <summary>
    /// 任务配置
    /// </summary>
    public Quest QuestData { get; set; }
}

/// <summary>
/// 任务进度更新事件参数
/// </summary>
public sealed class QuestProgressUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// 任务配置
    /// </summary>
    public Quest QuestData { get; set; }

    /// <summary>
    /// 当前进度值
    /// </summary>
    public int CurrentProgress { get; set; }

    /// <summary>
    /// 目标值
    /// </summary>
    public int TargetValue { get; set; }

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
    /// 任务配置
    /// </summary>
    public Quest QuestData { get; set; }

    /// <summary>
    /// 奖励金币数量
    /// </summary>
    public int RewardCoin { get; set; }
}

/// <summary>
/// 任务超时事件参数
/// </summary>
public sealed class QuestTimeoutEventArgs : EventArgs
{
    /// <summary>
    /// 任务配置
    /// </summary>
    public Quest QuestData { get; set; }
}

