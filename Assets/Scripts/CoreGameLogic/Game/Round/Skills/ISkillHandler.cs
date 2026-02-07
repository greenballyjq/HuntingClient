using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;

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
/// 技能阶段
/// </summary>
public enum SkillPhase
{
    /// <summary>
    /// 无阶段
    /// </summary>
    None,

    ///<summary>
    /// 技能开始阶段
    /// </summary>
    Starting,   

    /// <summary>
    /// 技能运行阶段
    /// </summary>
    Running,    
    
    /// <summary>
    /// 技能结束阶段
    /// </summary>
    Finished
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
    UniTask StartSkill(SkillContext context);

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    void DoUpdate(float dt);

    /// <summary>
    /// 技能结束
    /// </summary>
    void EndSkill();
}
