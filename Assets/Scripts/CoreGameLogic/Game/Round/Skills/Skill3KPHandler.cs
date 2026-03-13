using cfg.HuntingConfig.Skill;
using CoreGameLogic.Managers.AppManagers;
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

    private WeaponManager _weaponManager;

    private const string MODIFIER_SOURCE_ID = "Skill_3KP";

    protected override void OnInit() 
    {
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
        _skillParam = ConfigManager.GetSkill3KP(SkillContext.SkillData.ParamTableID);
    }

    protected override UniTask OnSkillStart()
    {
        _weaponManager.RegisterFireRateModifier(MODIFIER_SOURCE_ID, _skillParam.FireRateMultiplier);
        _weaponManager.RegisterDamageModifier(MODIFIER_SOURCE_ID, _skillParam.DamageMultiplier);

        _weaponManager.PlayerWeapon.WeaponVisual.SetSkillEffect(true);

        return UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd()
    {
        _weaponManager.PlayerWeapon?.WeaponVisual?.SetSkillEffect(false);

        _weaponManager.UnregisterFireRateModifier(MODIFIER_SOURCE_ID);
        _weaponManager.UnregisterDamageModifier(MODIFIER_SOURCE_ID);
    }
}