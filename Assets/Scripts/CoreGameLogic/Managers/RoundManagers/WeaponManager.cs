using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 武器管理器
/// </summary>
public class WeaponManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 主武器
    /// </summary>
    private MainWeapon _mainWeapon;

    /// <summary>
    /// 射速修正倍率字典
    /// key: 来源ID
    /// value: 倍率值
    /// </summary>
    private Dictionary<string, float> _fireRateModifiers = new Dictionary<string, float>();

    /// <summary>
    /// 伤害修正倍率字典
    /// key: 来源ID
    /// value: 倍率值
    /// </summary>
    private Dictionary<string, float> _damageModifiers = new Dictionary<string, float>();

    /// <summary>
    /// 当前射速修正倍率
    /// </summary>
    private float _fireRateMultiplier = 1f;

    /// <summary>
    /// 当前伤害修正倍率
    /// </summary>
    private float _damageMultiplier = 1f;

    public void Init(RoundContext context)
    {
        CollectMainWeapon();
        _mainWeapon.Init();
        Debug.Log("[WeaponManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        if (_mainWeapon != null)
            _mainWeapon.UpdateSpecialBulletTimer();
    }

    public void Dispose()
    {
        Debug.Log("[WeaponManager] 已释放");
    }

    public void Cleanup()
    {
        _mainWeapon.SetCurrentBullet(1);
        _mainWeapon = null;
    }

    public void ReInit(RoundContext context)
    {
        CollectMainWeapon();
        _mainWeapon.Init();
    }

    #region 公共方法
    /// <summary>
    /// 获取主武器实例
    /// </summary>
    public MainWeapon GetMainWeapon()
    {
        return _mainWeapon;
    }

    /// <summary>
    /// 注册射速修正倍率
    /// </summary>
    /// <param name="sourceId">修正来源ID（如技能类型、幸运仪式类型等）</param>
    /// <param name="multiplier">倍率值</param>
    public void RegisterFireRateModifier(string sourceId, float multiplier)
    {
        _fireRateModifiers[sourceId] = multiplier;
        UpdateFireRateMultiplier();
        Debug.Log($"[WeaponManager] 注册射速修正，来源:{sourceId}，倍率:{multiplier:F2}，当前总倍率:{_fireRateMultiplier:F2}");
    }

    /// <summary>
    /// 注销射速修正倍率
    /// </summary>
    /// <param name="sourceId">修正来源ID</param>
    public void UnregisterFireRateModifier(string sourceId)
    {
        if (_fireRateModifiers.Remove(sourceId))
        {
            UpdateFireRateMultiplier();
            Debug.Log($"[WeaponManager] 注销射速修正，来源:{sourceId}，当前总倍率:{_fireRateMultiplier:F2}");
        }
    }

    /// <summary>
    /// 注册伤害修正倍率
    /// </summary>
    /// <param name="sourceId">修正来源ID（如技能类型、幸运仪式类型等）</param>
    /// <param name="multiplier">倍率值</param>
    public void RegisterDamageModifier(string sourceId, float multiplier)
    {
        _damageModifiers[sourceId] = multiplier;
        UpdateDamageMultiplier();
        Debug.Log($"[WeaponManager] 注册伤害修正，来源:{sourceId}，倍率:{multiplier:F2}，当前总倍率:{_damageMultiplier:F2}");
    }

    /// <summary>
    /// 注销伤害修正倍率
    /// </summary>
    /// <param name="sourceId">修正来源ID</param>
    public void UnregisterDamageModifier(string sourceId)
    {
        if (_damageModifiers.Remove(sourceId))
        {
            UpdateDamageMultiplier();
            Debug.Log($"[WeaponManager] 注销伤害修正，来源:{sourceId}，当前总倍率:{_damageMultiplier:F2}");
        }
    }

    /// <summary>
    /// 获取修正后的射速
    /// </summary>
    public float GetCurrentFireRate(int bulletId)
    {
        var configManager = GameServiceLocator.ConfigManager;
        var bulletData = configManager.GetBullet(bulletId);
        if (bulletData == null)
        {
            Debug.LogWarning($"[WeaponManager] 子弹配置不存在: {bulletId}");
            return 0f;
        }

        return bulletData.FireRate * _fireRateMultiplier;
    }

    /// <summary>
    /// 获取修正后的伤害
    /// </summary>
    public float GetCurrentDamage(int bulletId)
    {
        var configManager = GameServiceLocator.ConfigManager;
        var bulletData = configManager.GetBullet(bulletId);
        if (bulletData == null)
        {
            Debug.LogWarning($"[WeaponManager] 子弹配置不存在: {bulletId}");
            return 0f;
        }

        return bulletData.BaseDamage * _damageMultiplier;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 收集场景中的主武器
    /// </summary>
    private void CollectMainWeapon()
    {
        GameObject weaponObj = GameObject.Find("MainWeapon");
        _mainWeapon = weaponObj.GetComponent<MainWeapon>();
    }

    /// <summary>
    /// 更新射速修正倍率
    /// </summary>
    private void UpdateFireRateMultiplier()
    {
        float total = 1f;
        foreach (var modifier in _fireRateModifiers.Values)
        {
            total *= modifier;
        }
        _fireRateMultiplier = total;

        // 通知主武器更新射速
        if (_mainWeapon != null)
            _mainWeapon.OnFireRateChanged();
    }

    /// <summary>
    /// 更新伤害修正倍率
    /// </summary>
    private void UpdateDamageMultiplier()
    {
        float total = 1f;
        foreach (var modifier in _damageModifiers.Values)
        {
            total *= modifier;
        }
        _damageMultiplier = total;
    }
    #endregion
}

