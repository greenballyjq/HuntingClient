using cfg.HuntingConfig;
using GameFramework.Utility;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 武器管理器
/// </summary>
public class WeaponManager : IMapWorld, IRoundUpdatable
{
    /// <summary>
    /// 玩家武器
    /// </summary>
    private PlayerWeapon _playerWeapon;
    public PlayerWeapon PlayerWeapon => _playerWeapon;

    public UniTask InitAsync(RoundContext context)
    {
        FindPlayerWeapon();
        _playerWeapon.Init();
        Log.Info("[WeaponManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void DoUpdate(float deltaTime)
    {
        if (_playerWeapon != null)
            _playerWeapon.UpdateSpecialBulletTimer();
    }

    public void Dispose()
    {
        Log.Info("[WeaponManager] 已释放");
    }

    public void Unbind()
    {
        if (_playerWeapon != null)
            _playerWeapon.SetBullet(1);
        _playerWeapon = null;
    }

    public void Bind(Map mapData)
    {
        FindPlayerWeapon();
        _playerWeapon.Init();
    }

    /// <summary>
    /// 寻找玩家武器
    /// </summary>
    private void FindPlayerWeapon()
    {
        GameObject weaponObj = GameObject.Find("PlayerWeapon");
        _playerWeapon = weaponObj.GetComponent<PlayerWeapon>();
    }
}
