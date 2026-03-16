using cfg.HuntingConfig;
using cfg.HuntingConfig.Prop;
using cfg.HuntingConfig.Skill;

/// <summary>
/// 消耗任务处理器（技能或道具被使用）
/// </summary>
public class QuestConsumeHandler : BaseQuestHandler
{
    public override void StartQuest()
    {
        base.StartQuest();
        EventManager.AddListener(SkillEvents.SkillStarted, OnSkillStarted);
        EventManager.AddListener(PropEvents.PropStarted, OnPropStarted);
    }

    public override void EndQuest()
    {
        EventManager.RemoveListener(SkillEvents.SkillStarted, OnSkillStarted);
        EventManager.RemoveListener(PropEvents.PropStarted, OnPropStarted);
    }

    protected override string BuildDescription()
    {
        return $"使用 {QuestData.TargetValue} 次技能或道具";
    }

    private void OnSkillStarted(SkillStartedEventArgs args)
    {
        AddProgress();
    }

    private void OnPropStarted(PropStartedEventArgs args)
    {
        AddProgress();
    }

    private void AddProgress()
    {
        CurrentProgress += 1;
        NotifyProgressUpdated(CurrentProgress);

        if (CurrentProgress >= QuestData.TargetValue)
            NotifyCompleted(QuestData.RewardValue);
    }
}
