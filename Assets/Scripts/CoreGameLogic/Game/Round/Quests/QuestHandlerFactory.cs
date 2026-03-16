using cfg.HuntingConfig.Enum;

/// <summary>
/// 任务处理器工厂
/// </summary>
public static class QuestHandlerFactory
{
    /// <summary>
    /// 创建任务处理器
    /// </summary>
    /// <param name="questType">任务类型</param>
    /// <returns>任务处理器实例</returns>
    public static IQuestHandler CreateQuestHandler(EQuestType questType)
    {
        switch (questType)
        {
            case EQuestType.Hunting:
                return new QuestHuntingHandler();
            case EQuestType.Collect:
                return new QuestCollectHandler();
            case EQuestType.Consume:
                return new QuestConsumeHandler();
            default:
                return null;
        }
    }
}
