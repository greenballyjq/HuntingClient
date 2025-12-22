using cfg.HuntingConfig.Enum;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 幸运仪式增益处理器工厂
    /// </summary>
    public static class LuckyBuffHandlerFactory
    {
        /// <summary>
        /// 创建幸运仪式增益处理器
        /// </summary>
        /// <param name="buffType">幸运仪式增益类型</param>
        /// <returns>幸运仪式增益处理器实例</returns>
        public static ILuckyBuffHandler CreateLuckyBuffHandler(ELuckyBuffType buffType)
        {
            switch (buffType)
            {
                case ELuckyBuffType.MoreMeat:
                    //return new LuckyBuffMoreMeatHandler();
                case ELuckyBuffType.StartSpecialBullet:
                    //return new LuckyBuffStartSpecialBulletHandler();
                case ELuckyBuffType.StartEnergy:
                    return new LuckyBuffStartEnergyHandler();
                case ELuckyBuffType.DamageBoost:
                    return new LuckyBuffDamageBoostHandler();
                case ELuckyBuffType.HighTierSpawn:
                    //return new LuckyBuffHighTierSpawnHandler();
                default:
                    return null;
            }
        }
    }
}

