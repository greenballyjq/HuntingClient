using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;

/// <summary>
/// 子弹效果工厂
/// </summary>
public static class BulletEffectFactory
{
    /// <summary>
    /// 创建子弹效果
    /// </summary>
    /// <param name="type">子弹类型</param>
    /// <param name="bulletData">子弹数据</param>
    /// <returns>子弹效果实例</returns>
    public static IBulletEffect CreateEffect(EBulletType type, Bullet bulletData)
    {
        switch (type)
        {
            case EBulletType.Explosive:
                return new ExplosiveBulletEffect(bulletData);
            default:
                return new NormalBulletEffect();
        }
    }
}
