using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;

/// <summary>
/// 色块人技能处理器
/// </summary>
public class Skill3KPHandler : BaseSkillHandler
{
    /// <summary>
    /// 技能参数
    /// </summary>
    private Skill3KP _skillParam;

    private RoundNumericLayer _numeric;
    private WeaponManager _weaponManager;
    private NumericHandle _fireRateHandle;
    private NumericHandle _damageHandle;

    protected override void OnInit()
    {
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
        _skillParam = ConfigManager.GetSkill3KP(SkillContext.SkillData.ParamTableID);
    }

    protected override UniTask OnSkillStart()
    {
        _fireRateHandle = _numeric.RegisterFireRate(
            new MultiplyFireRateModifier(_skillParam.FireRateMultiplier, FireRateWeaponKind.Player));
        _damageHandle = _numeric.RegisterDamage(
            new MultiplyDamageModifier(_skillParam.DamageMultiplier, DamageSourceKind.Weapon));

        _weaponManager.PlayerWeapon.WeaponVisual.SetSkillEffect(true);

        return UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd()
    {
        _weaponManager.PlayerWeapon?.WeaponVisual?.SetSkillEffect(false);

        _numeric.UnregisterFireRate(_fireRateHandle);
        _numeric.UnregisterDamage(_damageHandle);
    }
}
