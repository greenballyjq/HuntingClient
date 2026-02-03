/// <summary>
/// 色块人技能处理器
/// </summary>
public class Skill3KPHandler : ISkillHandler
{
    /// <summary>
    /// 技能修正来源ID
    /// </summary>
    private const string ModifierSourceId = "Skill_3KP";

    /// <summary>
    /// 剩余时间
    /// </summary>
    private float _remainingTime;

    public SkillPhase SkillPhase { get; set; }

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager = GameServiceLocator.ConfigManager;

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    public void OnSkillStart(SkillContext context)
    {
        SkillPhase = SkillPhase.Starting;

        var skillParam = _configManager.GetSkill3KP(context.SkillData.ParamTableID);
        _remainingTime = skillParam.Duration;   

        _weaponManager.RegisterFireRateModifier(ModifierSourceId, skillParam.FireRateMultiplier);
        _weaponManager.RegisterDamageModifier(ModifierSourceId, skillParam.DamageMultiplier);

        SkillPhase = SkillPhase.Running;
    }

    public void DoUpdate(float dt)
    {
        _remainingTime -= dt;

        if (_remainingTime <= 0)
            SkillPhase = SkillPhase.Finished;
    }

    public void OnSkillEnd()
    {
        _weaponManager.UnregisterFireRateModifier(ModifierSourceId);
        _weaponManager.UnregisterDamageModifier(ModifierSourceId);
    }
}