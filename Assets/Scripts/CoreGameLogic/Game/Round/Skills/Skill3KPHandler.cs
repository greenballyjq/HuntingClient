using Cysharp.Threading.Tasks;

/// <summary>
/// 色块人技能处理器
/// </summary>
public class Skill3KPHandler : BaseSkillHandler
{
    /// <summary>
    /// 技能修正来源ID
    /// </summary>
    private const string ModifierSourceId = "Skill_3KP";

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkill3KP(context.SkillData.ParamTableID);

        _weaponManager.RegisterFireRateModifier(ModifierSourceId, skillParam.FireRateMultiplier);
        _weaponManager.RegisterDamageModifier(ModifierSourceId, skillParam.DamageMultiplier);

        _weaponManager.PlayerWeapon.WeaponVisual.SetSkillEffect(true);

        await UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd()
    {
        _weaponManager.UnregisterFireRateModifier(ModifierSourceId);
        _weaponManager.UnregisterDamageModifier(ModifierSourceId);

        _weaponManager.PlayerWeapon.WeaponVisual.SetSkillEffect(false);
    }
}