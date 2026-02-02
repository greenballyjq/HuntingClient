using System.Collections.Generic;
using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Events;
using UnityEngine;
/// <summary>
/// 子弹管理器
/// </summary>
public class BulletManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager = GameServiceLocator.EventManager;

    /// <summary>
    /// 对象池管理器
    /// </summary>
    private GameObjectPoolManager _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;

    /// <summary>
    /// 活跃子弹集合
    /// </summary>
    private readonly HashSet<BulletBehavior> _activeBullets = new HashSet<BulletBehavior>();

    /// <summary>
    /// 待移除子弹列表
    /// </summary>
    private readonly List<BulletBehavior> _pendingRemovalBullets = new List<BulletBehavior>();

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager = GameServiceLocator.ConfigManager;

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    public void Init(RoundContext context)
    {
        RegisterEvents();
        Debug.Log("[BulletManager] 初始化完成");
    }

    public void Dispose()
    {
        RecycleAllBullets();
        UnregisterEvents();
        Debug.Log("[BulletManager] 已释放");
    }

    public void Cleanup()
    {
        RecycleAllBullets();
    }

    public void ReInit(RoundContext context){}

    public void DoUpdate(float dt)
    {
        foreach (var bullet in _activeBullets)
            bullet.DoUpdate(dt);

        ProcessPendingRemovals();
    }

    #region 公共方法
    /// <summary>
    /// 生成子弹
    /// </summary>
    public async UniTask<BulletBehavior> SpawnBullet(int bulletId, Vector3 position, Vector3 direction)
    {
        Bullet bulletData = _configManager.GetBullet(bulletId);

        float finalDamage = _weaponManager.GetCurrentDamage(bulletId);

        GameObject gameObject = await _gameObjectPoolManager.SpawnAsync(bulletData.PrefabResourcePath);

        // 初始化子弹
        gameObject.transform.position = position;
        if (direction != Vector3.zero)
            gameObject.transform.rotation = Quaternion.LookRotation(direction);
        var bullet = gameObject.GetComponent<BulletBehavior>();
        bullet.Init(bulletData, finalDamage);
        _activeBullets.Add(bullet);
            
        TriggerBulletSpawned(new BulletSpawnedEventArgs
        {
            SpawnPosition = position,
            Direction = direction,
            BulletData = bulletData,
            Bullet = bullet
        });

        return bullet;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 处理待移除的子弹
    /// </summary>
    private void ProcessPendingRemovals()
    {
        foreach (var bullet in _pendingRemovalBullets)
        {
            _activeBullets.Remove(bullet);
            _gameObjectPoolManager.Despawn(bullet.gameObject);
        }

        _pendingRemovalBullets.Clear();
    }

    /// <summary>
    /// 回收所有子弹
    /// </summary>
    private void RecycleAllBullets()
    {
        foreach (var bullet in _activeBullets)
            _gameObjectPoolManager.Despawn(bullet.gameObject);

        _activeBullets.Clear();
        _pendingRemovalBullets.Clear();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(BulletEvents.BulletDestroyed, OnBulletDestroyed);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(BulletEvents.BulletDestroyed, OnBulletDestroyed);
    }

    /// <summary>
    /// 子弹销毁事件回调
    /// </summary>
    private void OnBulletDestroyed(BulletDestroyedEventArgs args)
    {
        _pendingRemovalBullets.Add(args.Bullet);
    }

    /// <summary>
    /// 触发子弹生成事件
    /// </summary>
    private void TriggerBulletSpawned(BulletSpawnedEventArgs args)
    {
        _eventManager.Trigger(BulletEvents.BulletSpawned, args);
    }
    #endregion
}

