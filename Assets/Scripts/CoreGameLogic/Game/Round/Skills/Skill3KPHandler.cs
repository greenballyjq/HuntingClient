using cfg.HuntingConfig.Skill;
using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;

/// <summary>
/// 色块人技能处理器
/// </summary>
public class Skill3KPHandler : BaseSkillHandler
{
    /// <summary>
    /// 技能参数缓存
    /// </summary>
    private Skill3KP _skillParamCache;

    private WeaponManager _weaponManager;

    private const string MODIFIER_SOURCE_ID = "Skill_3KP";

    public Skill3KPHandler() : base()
    {
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
    }

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        if(_skillParamCache == null)
            _skillParamCache = _configManager.GetSkill3KP(context.SkillData.ParamTableID);

        _weaponManager.RegisterFireRateModifier(MODIFIER_SOURCE_ID, _skillParamCache.FireRateMultiplier);
        _weaponManager.RegisterDamageModifier(MODIFIER_SOURCE_ID, _skillParamCache.DamageMultiplier);

        _weaponManager.PlayerWeapon.WeaponVisual.SetSkillEffect(true);

        _soundManager.PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_BlueRed);

        // 模拟播放动画
        await UniTask.Delay(1000);
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd()
    {
        _weaponManager.PlayerWeapon?.WeaponVisual?.SetSkillEffect(false);

        _weaponManager.UnregisterFireRateModifier(MODIFIER_SOURCE_ID);
        _weaponManager.UnregisterDamageModifier(MODIFIER_SOURCE_ID);
    }
}