using cfg.HuntingConfig.Skill;

/// <summary>
/// 技能上下文
/// </summary>
public class SkillContext
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public RoundContext RoundContext { get; set; }

    /// <summary>
    /// 技能数据
    /// </summary>
    public Skill SkillData { get; set; }
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
    /// <param name="dt">时间增量</param>
    void OnSkillUpdate(float dt);

    /// <summary>
    /// 技能结束
    /// </summary>
    void OnSkillEnd();
}
