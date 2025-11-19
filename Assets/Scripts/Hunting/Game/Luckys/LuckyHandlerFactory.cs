using cfg.HuntingConfig.Enum;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 幸运仪式处理器工厂
    /// </summary>
    public static class LuckyHandlerFactory
    {
        /// <summary>
        /// 创建幸运仪式处理器
        /// </summary>
        /// <param name="luckyType">幸运仪式类型</param>
        /// <returns>幸运仪式处理器实例</returns>
        public static ILuckyHandler CreateLuckyHandler(ELuckyType luckyType)
        {
            switch (luckyType)
            {
                case ELuckyType.MoreMeat:
                    //return new LuckyMoreMeatHandler();
                case ELuckyType.SpecialBullet:
                    return new LuckySpecialBulletHandler();
                case ELuckyType.StartEnergy:
                    return new LuckyStartEnergyHandler();
                case ELuckyType.DamageBoost:
                    return new LuckyDamageBoostHandler();
                case ELuckyType.HighTierSpawn:
                    //return new LuckyHighTierSpawnHandler();
                case ELuckyType.None:
                default:
                    return null;
            }
        }
    }
}

