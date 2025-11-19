using cfg.HuntingConfig.Skill;
using GameFramework.Core;
using Hunting.Manager;

/// <summary>
/// 技能系统事件键
/// </summary>
public static class SkillEvents
{
    /// <summary>
    /// 技能开始事件
    /// </summary>
    public static readonly EventKey<SkillStartedEventArgs> SkillStarted = new EventKey<SkillStartedEventArgs>();

    /// <summary>
    /// 技能结束事件
    /// </summary>
    public static readonly EventKey<SkillEndedEventArgs> SkillEnded = new EventKey<SkillEndedEventArgs>();
}

/// <summary>
/// 技能开始事件参数
/// </summary>
public sealed class SkillStartedEventArgs : EventArgs
{
    /// <summary>
    /// 技能配置
    /// </summary>
    public Skill SkillData { get; set; }

    /// <summary>
    /// 当前局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}

/// <summary>
/// 技能结束事件参数
/// </summary>
public sealed class SkillEndedEventArgs : EventArgs
{
    /// <summary>
    /// 技能配置
    /// </summary>
    public Skill SkillData { get; set; }

    /// <summary>
    /// 当前局上下文
    /// </summary>
    public RoundContext Context { get; set; }
}


