using cfg.HuntingConfig.Skill;
using Hunting.Manager;

namespace Hunting.Game.Skills
{
    /// <summary>
    /// 技能上下文
    /// </summary>
    public class SkillContext
    {
        /// <summary>
        /// 技能数据
        /// </summary>
        public Skill SkillData { get; set; }

        /// <summary>
        /// 单局上下文
        /// </summary>
        public RoundContext RoundContext { get; set; }
    }

    /// <summary>
    /// 技能处理器接口
    /// </summary>
    public interface ISkillHandler
    {
        /// <summary>
        /// 技能开始
        /// </summary>
        /// <param name="context">技能上下文</param>
        void OnSkillStart(SkillContext context);

        /// <summary>
        /// 技能更新
        /// </summary>
        /// <param name="context">技能上下文</param>
        /// <param name="deltaTime">时间增量</param>
        void OnSkillUpdate(SkillContext context, float deltaTime);

        /// <summary>
        /// 技能结束
        /// </summary>
        /// <param name="context">技能上下文</param>
        void OnSkillEnd(SkillContext context);
    }
}


