using cfg.HuntingConfig.Enum;

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
    public static BaseSkillHandler CreateSkillHandler(ESkillType skillType)
    {
        switch (skillType)
        {
            case ESkillType._3KPSkill:
                return new Skill3KPHandler();
            case ESkillType.ZiWeiSkill:
                return new SkillZiWeiHandler();
            case ESkillType.DaMeiLiSkill:
                return new SkillDaMeiLiHandler();
            case ESkillType.JinZhuangYuanSkill:
                return new SkillJinZhuangYuanHandler();
            case ESkillType.YaKeDongSkill:
                return new SkillYaKeDongHandler();
            default:
                return null;
        }
    }
}
