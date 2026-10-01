using cfg.HuntingConfig.Bean;

/// <summary>
/// 伤害提升：武器与技能伤害连乘
/// </summary>
public sealed class LuckyDamageBoostSource : ILuckyBuffSource
{
    private readonly LuckyBuffParamDamageBoost _param;
    private RoundNumericLayer _numeric;
    private NumericHandle _handle;

    public LuckyDamageBoostSource(LuckyBuffParamDamageBoost param)
    {
        _param = param;
    }

    public void Activate()
    {
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
        _handle = _numeric.RegisterDamage(
            new MultiplyDamageModifier(
                _param.DamageIncreaseMultiplier,
                DamageSourceKind.Weapon | DamageSourceKind.Skill));
    }

    public void Deactivate()
    {
        _numeric?.UnregisterDamage(_handle);
        _numeric = null;
    }
}
