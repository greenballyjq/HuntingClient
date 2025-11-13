using cfg.HuntingConfig.Enum;

namespace Hunting.Game.Skills
{
    /// <summary>
    /// 技能处理器工厂
    /// </summary>
    public static class SkillHandlerFactory
    {
        /// <summary>
        /// 创建技能处理器
        /// </summary>
        /// <param name="skillType">技能类型</param>
        /// <returns>技能处理器实例</returns>
        public static ISkillHandler CreateSkillHandler(ESkillType skillType)
        {
            switch (skillType)
            {
                case ESkillType.YaKeDongSkill:
                    return new SkillYaKeDongHandler();
                default:
                    return null;
            }
        }
    }
}


