using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using Hunting.Events;
using Hunting.Game.Animal;
using Hunting.Game.Bullets;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 子弹管理器
    /// </summary>
    public class BulletManager : BaseGameManager
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 对象池管理器
        /// </summary>
        private GameObjectPoolManager Pool => GameServiceLocator.Pool;

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager Weapon => GameServiceLocator.GetGameManager<WeaponManager>();

        public override void Init()
        {
            RegisterEvents();
            Debug.Log("[BulletManager] 初始化完成");
        }

        public override void Update()
        {
            // 当前无需逐帧逻辑
        }

        public override void Release()
        {
            UnregisterEvents();
            Debug.Log("[BulletManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 生成子弹
        /// </summary>
        public async UniTask<BulletBehavior> SpawnBullet(int bulletId, Vector3 position, Vector3 direction, GameObject owner)
        {
            Bullet bulletData = Config.GetBullet(bulletId);
            if (bulletData == null)
            {
                Debug.LogError($"[BulletManager] 子弹配置不存在: {bulletId}");
                return null;
            }

            // 从WeaponManager获取修正后的伤害
            float finalDamage = Weapon.GetCurrentDamage(bulletId);

            // 从对象池获取子弹预制体
            var go = await Pool.PullAsync(bulletData.PrefabResourcePath);
            if (go == null)
            {
                Debug.LogError($"[BulletManager] 对象池取子弹失败: {bulletData.PrefabResourcePath}");
                return null;
            }

            // 设置初始位置和朝向
            go.transform.position = position;
            if (direction != Vector3.zero)
                go.transform.rotation = Quaternion.LookRotation(direction);

            // 获取并初始化子弹组件
            var bullet = go.GetComponent<BulletBehavior>();
            if (bullet == null)
            {
                Debug.LogError("[BulletManager] 子弹预制体缺少 BulletBehavior 组件");
                Pool.Push(go);
                return null;
            }

            bullet.Init(bulletData, position, direction, owner, finalDamage);

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

        #region 私有方法
        /// <summary>
        /// 回收指定子弹到对象池
        /// </summary>
        private void RecycleBullet(BulletBehavior bullet)
        {
            Pool.Push(bullet.gameObject);
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(BulletEvents.BulletDestroyed, OnBulletDestroyed);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(BulletEvents.BulletDestroyed, OnBulletDestroyed);
        }

        /// <summary>
        /// 子弹销毁事件回调
        /// </summary>
        private void OnBulletDestroyed(BulletDestroyedEventArgs args)
        {
            RecycleBullet(args.Bullet);
            Debug.Log("[BulletManager] 子弹已回收");
        }

        /// <summary>
        /// 触发子弹生成事件
        /// </summary>
        private void TriggerBulletSpawned(BulletSpawnedEventArgs args)
        {
            Event.Trigger(BulletEvents.BulletSpawned, args);
        }
        #endregion
    }
}

