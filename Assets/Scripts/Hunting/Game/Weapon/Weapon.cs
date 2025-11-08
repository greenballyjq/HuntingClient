using cfg;
using Hunting.Game.Animal;
using Hunting.Game.Bullet;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Weapon
{
    /// <summary>
    /// 武器实现
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("武器视觉引用")]
        [SerializeField] private WeaponVisual weaponVisual;
        
        [Header("射击设置")]
        [SerializeField] private int currentBulletId = 1; // 当前子弹类型ID

        [Header("音效")] [SerializeField] private AudioClip fireAudioClip;
        private AudioSource _audioSource;
        
        #region 射击相关字段
        /// <summary>
        /// 上次射击的时间戳
        /// </summary>
        private float _lastFireTime;
        /// <summary>
        /// 射击间隔
        /// </summary>
        private float _fireInterval;
        /// <summary>
        /// 特殊子弹剩余持续时间（秒）
        /// </summary>
        private float _specialBulletRemainingTime;

        /// <summary>
        /// 是否处于特殊子弹状态
        /// </summary>
        private bool _isUsingSpecialBullet;
        #endregion

        #region 服务引用
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;
        #endregion

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private async void Start()
        {
            await GameServiceLocator.WaitForInitialization();
            
            // 订阅动物掉落奖励事件
            Event.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
            
            ChangeBullet(currentBulletId);
            UpdateFireIntervalFromConfig();
        }

        /// <summary>
        /// 处理动物掉落奖励
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            if (!args.DropRewards.TryGetValue(EDropType.Bullet, out var bulletAmount) || bulletAmount <= 0)
                return;

            // 掉落子弹奖励，获取随机特殊子弹
            var specialBullet = HuntingGameConfigManager.Instance.GetRandomSpecialBullet();
            if (specialBullet != null)
            {
                // 切换到特殊子弹
                ChangeBullet(specialBullet.ID);
                Debug.LogWarning($"[Weapon] 获得特殊子弹: {specialBullet.Name}，持续时间: {specialBullet.Duration}秒");
            }
        }

        private void OnDestroy()
        {
            // 取消订阅事件
            Event.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        private void Update()
        {
            UpdateShooting();
            UpdateSpecialBulletTimer();
        }

        /// <summary>
        /// 根据子弹配置更新射击间隔
        /// </summary>
        private void UpdateFireIntervalFromConfig()
        {
            var bulletConfig = HuntingGameConfigManager.Instance.GetBullet(currentBulletId);
            if (bulletConfig != null)
            {
                // 射击间隔 = 1 / 射速（发/秒）
                _fireInterval = 1f / bulletConfig.FireRate;
            }
        }

        private void UpdateShooting()
        {
            if (!PlayerControl.Instance.ShootingPerformed) return;
            TryShoot();
        }
        
        /// <summary>
        /// 尝试发射子弹（考虑射击冷却）
        /// </summary>
        private void TryShoot()
        {
            // 检查射击间隔
            if (Time.time - _lastFireTime >= _fireInterval)
            {
                SpawnBullet();
                _lastFireTime = Time.time;

                _audioSource.clip = fireAudioClip;
                _audioSource.Play();
            }
        }
        
        /// <summary>
        /// 生成并初始化子弹
        /// </summary>
        private void SpawnBullet()
        {
            // 从配置管理器获取子弹数据
            var bulletConfig = HuntingGameConfigManager.Instance.GetBullet(currentBulletId);
            if (bulletConfig == null) return;

            // 根据子弹类型获取预制体名称
            string bulletName = GetBulletPrefabName(bulletConfig.BulletType);
            // 从Resources加载子弹预制体
            GameObject bulletPrefab = Resources.Load<GameObject>($"Bullets/{bulletName}");

            if (bulletPrefab == null)
            {
                Debug.LogError($"子弹预制体不存在: {bulletName}");
                return;
            }

            // 实例化子弹对象
            Transform muzzlePoint = weaponVisual.MuzzlePoint; 
            GameObject bulletObj = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
            BulletBehavior bullet = bulletObj.GetComponent<BulletBehavior>();

            // 初始化子弹行为
            if (bullet != null)
            {
                bullet.Initialize(bulletConfig, muzzlePoint.position, muzzlePoint.forward);
            }
        }
        
        /// <summary>
        /// 根据子弹类型枚举获取预制体资源名称
        /// </summary>
        private string GetBulletPrefabName(EBulletType bulletType)
        {
            return bulletType switch
            {
                EBulletType.Normal => "Bullet_Normal",
                EBulletType.Explosive => "Bullet_Explosive",
                EBulletType.HighDamage => "Bullet_HighDamage",
                EBulletType.HighSpeed => "Bullet_HighSpeed",
                _ => "Bullet_Normal"
            };
        }
        
        /// <summary>
        /// 检查并更新特殊子弹持续时间
        /// </summary>
        private void UpdateSpecialBulletTimer()
        {
            if (!_isUsingSpecialBullet) return;

            _specialBulletRemainingTime -= Time.deltaTime;

            if (_specialBulletRemainingTime <= 0f)
            {
                // 恢复为普通子弹
                ChangeBullet(1);
                _isUsingSpecialBullet = false;
                Debug.LogWarning("[PlayerControl] 特殊子弹时间到，已切回普通子弹");
            }
        }
        
        /// <summary>
        /// 切换当前使用的子弹类型
        /// </summary>
        /// <param name="newBulletId">新的子弹ID</param>
        private void ChangeBullet(int newBulletId)
        {
            currentBulletId = newBulletId;
            UpdateFireIntervalFromConfig(); // 更新射击间隔

            var bulletConfig = HuntingGameConfigManager.Instance.GetBullet(newBulletId);

            // 如果是特殊子弹（持续时间 > 0），则启动计时
            if (bulletConfig != null && bulletConfig.Duration > 0)
            {
                _specialBulletRemainingTime = bulletConfig.Duration;
                _isUsingSpecialBullet = true;
            }
            else
            {
                _isUsingSpecialBullet = false;
            }
        }
        
    }
}