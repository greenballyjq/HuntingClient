/// <summary>
/// 开局特殊子弹幸运仪式增益处理器
/// </summary>
public class LuckyBuffStartSpecialBulletHandler : ILuckyBuffHandler
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager = GameServiceLocator.ConfigManager;

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    /// <summary>
    /// 激活幸运仪式增益效果
    /// </summary>
    public void OnActivate(LuckyBuffContext context)
    {
        // 获取随机特殊子弹
        var specialBullet = _configManager.GetRandomSpecialBullet();

        // 获取主武器并切换子弹
        var mainWeapon = _weaponManager.PlayerWeapon;
        mainWeapon.SetBullet(specialBullet.ID);
    }

    /// <summary>
    /// 注销幸运仪式增益效果
    /// </summary>
    public void OnDeactivate(LuckyBuffContext context)
    {

    }
}