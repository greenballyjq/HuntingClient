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
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager = GameServiceLocator.ConfigManager;

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    /// <summary>
    /// 技能开始
    /// </summary>
    public void OnSkillStart(SkillContext context)
    {
        var parameter = _configManager.GetSkill3KP(context.SkillData.ParamTableID);

        _weaponManager.RegisterFireRateModifier(ModifierSourceId, parameter.FireRateMultiplier);
        _weaponManager.RegisterDamageModifier(ModifierSourceId, parameter.DamageMultiplier);
    }

    public void OnSkillUpdate(float dt){}

    public void OnSkillEnd()
    {
        _weaponManager.UnregisterFireRateModifier(ModifierSourceId);
        _weaponManager.UnregisterDamageModifier(ModifierSourceId);
    }
}