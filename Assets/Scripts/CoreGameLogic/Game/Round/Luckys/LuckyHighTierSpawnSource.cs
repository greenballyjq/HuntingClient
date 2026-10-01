using cfg.HuntingConfig.Bean;

/// <summary>
/// 高阶派发：Large / Bullet / ThreeKPCoin 权重连乘
/// </summary>
public sealed class LuckyHighTierSpawnSource : ILuckyBuffSource
{
    private readonly LuckyBuffHighTierSpawn _param;
    private RoundNumericLayer _numeric;
    private NumericHandle _handle;

    public LuckyHighTierSpawnSource(LuckyBuffHighTierSpawn param)
    {
        _param = param;
    }

    public void Activate()
    {
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
        _handle = _numeric.RegisterSpawnWeight(
            new HighTierSpawnWeightModifier(_param.HighTierSpawnWeightMultiplier));
    }

    public void Deactivate()
    {
        _numeric?.UnregisterSpawnWeight(_handle);
        _numeric = null;
    }
}
