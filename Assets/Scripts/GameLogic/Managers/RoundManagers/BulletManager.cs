using System.Collections.Generic;
using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using Hunting.Events;
using UnityEngine;
    /// <summary>
    /// 子弹管理器
    /// </summary>
    public class BulletManager : IRoundManager
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
        /// 追踪所有活跃的子弹
        /// </summary>
        private readonly HashSet<BulletBehavior> _activeBullets = new HashSet<BulletBehavior>();

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
            // 回收所有活跃的子弹
            foreach (var bullet in _activeBullets)
                if (bullet != null && bullet.gameObject != null)
                    _gameObjectPoolManager.Despawn(bullet.gameObject);
                    
            _activeBullets.Clear();
            
            UnregisterEvents();
            Debug.Log("[BulletManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 生成子弹
        /// </summary>
        public async UniTask<BulletBehavior> SpawnBullet(int bulletId, Vector3 position, Vector3 direction)
        {
            Bullet bulletData = _configManager.GetBullet(bulletId);

            // 从WeaponManager获取修正后的伤害
            float finalDamage = _weaponManager.GetCurrentDamage(bulletId);

            // 从对象池获取子弹预制体
            GameObject gameObject = await _gameObjectPoolManager.SpawnAsync(bulletData.PrefabResourcePath);
            if (gameObject == null)
            {
                Debug.LogError($"[BulletManager] 对象池取子弹失败: {bulletData.PrefabResourcePath}");
                return null;
            }

            // 设置初始位置和朝向
            gameObject.transform.position = position;
            if (direction != Vector3.zero)
                gameObject.transform.rotation = Quaternion.LookRotation(direction);

            // 获取并初始化子弹组件
            var bullet = gameObject.GetComponent<BulletBehavior>();
            if (bullet == null)
            {
                Debug.LogError("[BulletManager] 子弹预制体缺少 BulletBehavior 组件");
                _gameObjectPoolManager.Despawn(gameObject);
                return null;
            }

            bullet.Init(bulletData, finalDamage);
            
            // 添加到活跃子弹集合
            _activeBullets.Add(bullet);
            
            // 对外通知子弹已生成
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
            // 从活跃集合中移除
            _activeBullets.Remove(args.Bullet);
            _gameObjectPoolManager.Despawn(args.Bullet.gameObject);
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

