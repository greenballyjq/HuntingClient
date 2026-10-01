using System;
using cfg.HuntingConfig;

/// <summary>
/// 任务处理器基类
/// </summary>
public abstract class BaseQuestHandler : IQuestHandler
{
    /// <summary>
    /// 任务数据
    /// </summary>
    protected Quest QuestData;

    /// <summary>
    /// 当前进度
    /// </summary>
    protected int CurrentProgress;

    public Action<string, int, int> OnQuestDispatched { get; set; }
    public Action<int> OnQuestProgressUpdated { get; set; }
    public Action<int> OnQuestCompleted { get; set; }

    protected EventManager EventManager;

    public virtual void Init(Quest questData)
    {
        if (EventManager == null)
            EventManager = GameServiceLocator.EventManager;

        QuestData = questData;
        CurrentProgress = 0;
    }

    public virtual void StartQuest()
    {
        string description = BuildDescription();
        OnQuestDispatched?.Invoke(description, QuestData.TargetValue, QuestData.RewardValue);
    }

    public virtual void EndQuest() {}

    /// <summary>
    /// 构建任务描述
    /// </summary>
    protected abstract string BuildDescription();

    /// <summary>
    /// 通知进度更新
    /// </summary>
    protected void NotifyProgressUpdated(int progress)
    {
        OnQuestProgressUpdated?.Invoke(progress);
    }

    /// <summary>
    /// 通知任务完成
    /// </summary>
    protected void NotifyCompleted(int rewardValue)
    {
        OnQuestCompleted?.Invoke(rewardValue);
    }
}
