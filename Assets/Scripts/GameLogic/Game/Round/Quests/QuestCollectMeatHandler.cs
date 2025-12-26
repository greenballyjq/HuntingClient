using cfg.HuntingConfig.Enum;
using GameFramework.Core;

namespace Hunting.Game.Quests
{
    /// <summary>
    /// 收集肉类任务处理器
    /// </summary>
    public class QuestCollectMeatHandler : IQuestHandler
    {
        /// <summary>
        /// 当前收集的肉类数量
        /// </summary>
        private int _currentProgress;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager => GameServiceLocator.EventManager;

        /// <summary>
        /// 任务开始
        /// </summary>
        public void OnQuestStart(QuestContext context)
        {
            // 重置进度
            _currentProgress = 0;

            // 订阅动物掉落奖励事件
            _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 任务更新
        /// </summary>
        public void OnQuestUpdate(QuestContext context, float deltaTime)
        {

        }

        /// <summary>
        /// 任务结束
        /// </summary>
        public void OnQuestEnd(QuestContext context)
        {
            // 取消订阅动物掉落奖励事件
            _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 获取当前进度值
        /// </summary>
        public int GetCurrentProgress(QuestContext context)
        {
            return _currentProgress;
        }

        #region 事件回调
        /// <summary>
        /// 动物掉落奖励事件回调
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            // 从掉落奖励中获取肉类数量
            if (args.DropRewards.TryGetValue(EDropType.Meat, out var meatAmount) && meatAmount > 0)
                _currentProgress += meatAmount;
        }
        #endregion
    }
}

