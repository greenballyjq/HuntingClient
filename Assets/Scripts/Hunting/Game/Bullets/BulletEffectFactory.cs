using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using Hunting.Game.Bullets.Effects;

namespace Hunting.Game.Bullets
{
    /// <summary>
    /// 子弹效果工厂
    /// </summary>
    public static class BulletEffectFactory
    {
        /// <summary>
        /// 创建额外效果
        /// </summary>
        /// <param name="type">子弹类型</param>
        /// <param name="bulletData">子弹数据</param>
        /// <returns>额外效果实例</returns>
        public static IBulletEffect CreateExtraEffect(EBulletType type, Bullet bulletData)
        {
            switch (type)
            {
                case EBulletType.Explosive:
                    return new ExplosiveBulletEffect(bulletData);
                default:
                    return null;
            }
        }
    }
}


