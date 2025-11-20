using cfg.HuntingConfig;
using Hunting.Manager;

namespace Hunting.Game.Quests
{
    /// <summary>
    /// 任务上下文
    /// </summary>
    public class QuestContext
    {
        /// <summary>
        /// 任务配置
        /// </summary>
        public Quest QuestData { get; set; }
    }

    /// <summary>
    /// 任务处理器接口
    /// </summary>
    public interface IQuestHandler
    {
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="context">任务上下文</param>
        void OnQuestStart(QuestContext context);

        /// <summary>
        /// 任务更新
        /// </summary>
        /// <param name="context">任务上下文</param>
        /// <param name="deltaTime">时间增量</param>
        void OnQuestUpdate(QuestContext context, float deltaTime);

        /// <summary>
        /// 任务结束
        /// </summary>
        /// <param name="context">任务上下文</param>
        void OnQuestEnd(QuestContext context);

        /// <summary>
        /// 获取当前进度值
        /// </summary>
        /// <param name="context">任务上下文</param>
        /// <returns>当前进度值</returns>
        int GetCurrentProgress(QuestContext context);
    }
}

