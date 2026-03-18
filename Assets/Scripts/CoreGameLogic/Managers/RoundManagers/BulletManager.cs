using System.Collections.Generic;
using cfg.HuntingConfig;
using GameFramework.Manager;
using GameFramework.Utility;
using Hunting.Events;
using UnityEngine;
/// <summary>
/// 子弹管理器
/// </summary>
public class BulletManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 活跃子弹集合
    /// </summary>
    private readonly HashSet<BulletBehavior> _activeBullets = new HashSet<BulletBehavior>();

    /// <summary>
    /// 待移除子弹列表
    /// </summary>
    private readonly List<BulletBehavior> _pendingRemovalBullets = new List<BulletBehavior>();

    /// <summary>
    /// 子弹配置缓存
    /// </summary>
    private readonly Dictionary<int, Bullet> _bulletDatas = new Dictionary<int, Bullet>();

    /// <summary>
    /// 子弹预制体缓存
    /// </summary>
    private readonly Dictionary<int, GameObject> _bulletPrefabs = new Dictionary<int, GameObject>();

    private EventManager _eventManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private HuntingConfigManager _configManager;
    private WeaponManager _weaponManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        RegisterEvents();
        CacheBulletDatas();
        Log.Info("[BulletManager] 初始化完成");
    }

    public void Dispose()
    {
        RecycleAllBullets();
        ClearBulletDataCache();
        UnregisterEvents();

        Log.Info("[BulletManager] 已释放");
    }

    public void Cleanup()
    {
        RecycleAllBullets();
    }

    public void ReInit(Map mapData){}

    public void DoUpdate(float dt)
    {
        foreach (var bullet in _activeBullets)
            bullet.DoUpdate(dt);

        ProcessPendingRemovals();
    }

    #region 公共方法
    /// <summary>
    /// 根据ID获取子弹配置
    /// </summary>
    public Bullet GetBullet(int bulletId) => _bulletDatas[bulletId];

    /// <summary>
    /// 生成子弹
    /// </summary>
    public BulletBehavior SpawnBullet(int bulletId, Vector3 position, Vector3 direction)
    {
        Bullet bulletData = _bulletDatas[bulletId];
        GameObject prefab = _bulletPrefabs[bulletId];

        float finalDamage = _weaponManager.GetCurrentDamage(bulletId);

        GameObject gameObject = _gameObjectPoolManager.Spawn(prefab);

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
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _configManager = GameServiceLocator.ConfigManager;
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
    }

    /// <summary>
    /// 缓存子弹配置
    /// </summary>
    private void CacheBulletDatas()
    {
        ClearBulletDataCache();

        foreach (var bullet in _configManager.BulletTable.DataList)
        {
            _bulletDatas[bullet.ID] = bullet;
            _bulletPrefabs[bullet.ID] = _configManager.BulletRefSo.GetBulletPrefab(bullet.ID);
        }
    }

    /// <summary>
    /// 清理子弹配置缓存
    /// </summary>
    private void ClearBulletDataCache()
    {
        _bulletDatas.Clear();
        _bulletPrefabs.Clear();
    }

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
    /// 触发子弹生成事件
    /// </summary>
    private void TriggerBulletSpawned(BulletSpawnedEventArgs args)
    {
        _eventManager.Trigger(BulletEvents.BulletSpawned, args);
    }

    /// <summary>
    /// 触发子弹销毁事件回调
    /// </summary>
    private void OnBulletDestroyed(BulletDestroyedEventArgs args)
    {
        _pendingRemovalBullets.Add(args.Bullet);
    }
    #endregion
}
