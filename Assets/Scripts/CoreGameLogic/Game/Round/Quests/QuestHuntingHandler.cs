using cfg.HuntingConfig;

/// <summary>
/// 狩猎任务处理器（击杀动物）
/// </summary>
public class QuestHuntingHandler : BaseQuestHandler
{
    public override void StartQuest()
    {
        base.StartQuest();
        EventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    public override void EndQuest()
    {
        EventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    protected override string BuildDescription()
    {
        return $"击杀 {QuestData.TargetValue} 只动物";
    }

    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        CurrentProgress += 1;
        NotifyProgressUpdated(CurrentProgress);

        if (CurrentProgress >= QuestData.TargetValue)
            NotifyCompleted(QuestData.RewardValue);
    }
}
