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

        // fixme PlayerWeapon is null 
        // CleanUp 时被调用，同时 WeaponManager也被调用CleanUp方法，PlayerWeapon 被置为空
        // 目前先加一个判空安全, 保证后续测试不会卡死
        _weaponManager.PlayerWeapon?.WeaponVisual.SetSkillEffect(false);
    }
}