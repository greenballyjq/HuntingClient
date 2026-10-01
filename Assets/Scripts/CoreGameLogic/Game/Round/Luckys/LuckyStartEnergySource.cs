using cfg.HuntingConfig.Bean;

/// <summary>
/// 开局丰收能量：瞬时发放对应条数能量
/// </summary>
public sealed class LuckyStartEnergySource : ILuckyBuffSource
{
    private readonly LuckyBuffParamStartEnergy _param;
    private EnergyProgressManager _energyProgressManager;

    public LuckyStartEnergySource(LuckyBuffParamStartEnergy param)
    {
        _param = param;
    }

    public void Activate()
    {
        _energyProgressManager = GameServiceLocator.GetRoundManager<EnergyProgressManager>();
        float amount = _energyProgressManager.ValuePerBar * _param.EnergyBarCount;
        _energyProgressManager.AddEnergyValue(amount);
    }

    public void Deactivate()
    {
    }
}
