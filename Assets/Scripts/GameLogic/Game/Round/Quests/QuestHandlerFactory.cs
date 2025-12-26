using cfg.HuntingConfig.Enum;

namespace Hunting.Game.Quests
{
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
                case EQuestType.KillLargeAnimal:
                    return new QuestKillLargeAnimalHandler();
                case EQuestType.CollectMeat:
                    return new QuestCollectMeatHandler();
                case EQuestType.UsePaidItem:
                    //return new QuestUsePaidItemHandler();
                case EQuestType.Settle:
                    //return new QuestSettleHandler();
                default:
                    return null;
            }
        }
    }
}

