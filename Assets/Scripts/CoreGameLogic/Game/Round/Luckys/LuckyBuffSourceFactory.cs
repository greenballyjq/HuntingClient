using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using GameFramework.Utility;

/// <summary>
/// 幸运仪式增益来源工厂
/// </summary>
public static class LuckyBuffSourceFactory
{
    public static ILuckyBuffSource Create(LuckyBuff buffData)
    {
        if (buffData?.EffectParam == null)
            return null;

        switch (buffData.LuckyBuffType)
        {
            case ELuckyBuffType.MoreMeat:
                return buffData.EffectParam is LuckyBuffParamMoreMeat moreMeat
                    ? new LuckyMoreMeatSource(moreMeat)
                    : null;

            case ELuckyBuffType.StartEnergy:
                return buffData.EffectParam is LuckyBuffParamStartEnergy startEnergy
                    ? new LuckyStartEnergySource(startEnergy)
                    : null;

            case ELuckyBuffType.DamageBoost:
                return buffData.EffectParam is LuckyBuffParamDamageBoost damageBoost
                    ? new LuckyDamageBoostSource(damageBoost)
                    : null;

            case ELuckyBuffType.HighTierSpawn:
                return buffData.EffectParam is LuckyBuffHighTierSpawn highTier
                    ? new LuckyHighTierSpawnSource(highTier)
                    : null;

            default:
                Log.Error($"[LuckyBuffSourceFactory] 未知幸运仪式类型: {buffData.LuckyBuffType}");
                return null;
        }
    }
}
