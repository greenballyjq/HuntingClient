using cfg;

namespace Tool.Extensions
{
    /// <summary>
    /// 子弹数据结构扩展类，为子弹数据结构类提供额外的功能
    /// </summary>
    public static class BulletConfigExtensions
    {
        /// <summary>
        /// 是否是范围伤害
        /// </summary>
        public static bool IsAOE(this cfg.HuntingConfig.Bullet bullet)
            => bullet.DamageRangeType == EBulletDamageRangeType.AOE;

    }
}

