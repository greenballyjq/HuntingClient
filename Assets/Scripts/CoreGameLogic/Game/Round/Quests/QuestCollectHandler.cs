using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;

/// <summary>
/// 收集任务处理器（收集肉）
/// </summary>
public class QuestCollectHandler : BaseQuestHandler
{
    public override void StartQuest()
    {
        base.StartQuest();
        EventManager.AddListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    public override void EndQuest()
    {
        EventManager.RemoveListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    protected override string BuildDescription()
    {
        return $"收集 {QuestData.TargetValue} 块肉";
    }

    private void OnDropRewardArrived(RewardArrivedEventArgs args)
    {
        if (args.DropType != EDropType.Meat)
            return;

        CurrentProgress += args.DropCount;
        NotifyProgressUpdated(CurrentProgress);

        if (CurrentProgress >= QuestData.TargetValue)
            NotifyCompleted(QuestData.RewardValue);
    }
}
