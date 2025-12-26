using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using Hunting.App;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Weapons
{
    /// <summary>
    /// 武器类
    /// </summary>
    public class MainWeapon : MonoBehaviour
    {
        /// <summary>
        /// 武器旋转速度
        /// </summary>
        [SerializeField] private float _rotationSpeed = 20f;

        /// <summary>
        /// 开火点
        /// </summary>
        [SerializeField] private Transform _muzzlePoint;

        /// <summary>
        /// 当前子弹ID
        /// </summary>
        private int _currentBulletId = 1;

        /// <summary>
        /// 射击间隔（秒）
        /// </summary>
        private float _fireInterval;

        /// <summary>
        /// 上次射击的时间戳
        /// </summary>
        private float _lastFireTime;

        /// <summary>
        /// 特殊子弹剩余持续时间（秒）
        /// </summary>
        private float _specialBulletRemainingTime;

        /// <summary>
        /// 是否处于特殊子弹状态
        /// </summary>
        private bool _isSpecialBullet;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager = GameServiceLocator.EventManager;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager _configManager = GameServiceLocator.ConfigManager;

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

        /// <summary>
        /// 子弹管理器
        /// </summary>
        private BulletManager _bulletManager => GameServiceLocator.GetRoundManager<BulletManager>();

        private void Start()
        {
            RegisterEvents();
            InitializeWeapon();
        }

        private void Update()
        {
            UpdateSpecialBulletTimer();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #region 公共方法
        /// <summary>
        /// 设置当前子弹类型
        /// </summary>
        public void SetCurrentBullet(int bulletId)
        {
            ChangeBullet(bulletId);
        }

        /// <summary>
        /// 获取当前子弹ID
        /// </summary>
        public int GetCurrentBulletId()
        {
            return _currentBulletId;
        }

        /// <summary>
        /// 响应射速变化
        /// </summary>
        public void OnFireRateChanged()
        {
            UpdateFireInterval();
        }

        /// <summary>
        /// 设置瞄准目标
        /// </summary>
        /// <param name="worldPosition">目标世界坐标</param>
        public void SetAimTarget(Vector3 worldPosition)
        {
            // 计算方向（保持Y轴水平）
            Vector3 direction = worldPosition - transform.position;
            direction.y = 0f;

            // 旋转武器
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        /// <summary>
        /// 射击
        /// </summary>
        public void Fire()
        {
            // 检查射击间隔
            if (Time.time - _lastFireTime < _fireInterval)
                return;

            // 执行射击
            SpawnBulletAsync().Forget();
            PlayFireSound();
            _lastFireTime = Time.time;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化武器状态
        /// </summary>
        private void InitializeWeapon()
        {
            // 设置默认子弹
            ChangeBullet(_currentBulletId);
            UpdateFireInterval();
        }

        /// <summary>
        /// 更新射击间隔
        /// </summary>
        private void UpdateFireInterval()
        {
            float fireRate = _weaponManager.GetCurrentFireRate(_currentBulletId);
            _fireInterval = 1f / fireRate;
        }

        /// <summary>
        /// 更新特殊子弹倒计时
        /// </summary>
        private void UpdateSpecialBulletTimer()
        {
            if (!_isSpecialBullet)
                return;

            _specialBulletRemainingTime -= Time.deltaTime;

            // 获取子弹配置用于触发倒计时事件
            var bulletData = _configManager.GetBullet(_currentBulletId);
            if (bulletData != null)
            {
                // 触发特殊子弹倒计时事件
                TriggerSpecialBulletCountdown(new SpecialBulletCountdownEventArgs
                {
                    BulletData = bulletData,
                    RemainingTime = _specialBulletRemainingTime
                });
            }

            // 检查是否时间到
            if (_specialBulletRemainingTime <= 0f)
            {
                // 触发特殊子弹效果结束事件
                var endedBulletData = _configManager.GetBullet(_currentBulletId);
                TriggerSpecialBulletEffectEnded(new SpecialBulletEffectEndedEventArgs
                {
                    BulletData = endedBulletData
                });

                // 恢复为普通子弹（ID为1）
                ChangeBullet(1);
                Debug.Log("[Weapon] 特殊子弹时间到，已切回普通子弹");
            }
        }

        /// <summary>
        /// 生成子弹
        /// </summary>
        private async UniTask SpawnBulletAsync()
        {
            await _bulletManager.SpawnBullet(_currentBulletId, _muzzlePoint.position, _muzzlePoint.forward);
        }

        /// <summary>
        /// 切换当前使用的子弹类型
        /// </summary>
        private void ChangeBullet(int newBulletId)
        {
            int oldBulletId = _currentBulletId;
            _currentBulletId = newBulletId;

            // 更新射击间隔
            UpdateFireInterval();

            // 获取子弹配置
            var newBulletData = _configManager.GetBullet(newBulletId);
            if (newBulletData== null)
            {
                Debug.LogWarning($"[Weapon] 子弹配置不存在: {newBulletId}");
                return;
            }

            var oldBulletData = _configManager.GetBullet(oldBulletId);

            // 判断是否为特殊子弹（持续时间 > 0）
            bool isSpecialBullet = newBulletData.Duration > 0f;
            if (isSpecialBullet)
            {
                _specialBulletRemainingTime = newBulletData.Duration;
                _isSpecialBullet = true;
            }
            else
            {
                _specialBulletRemainingTime = 0f;
                _isSpecialBullet = false;
            }

            // 触发子弹切换事件
            TriggerBulletChanged(new BulletChangedEventArgs
            {
                OldBulletData = oldBulletData,
                NewBulletData = newBulletData,
                IsSpecialBullet = isSpecialBullet,
                RemainingTime = _specialBulletRemainingTime
            });

            Debug.Log($"[Weapon] 切换子弹: {oldBulletId} -> {newBulletId}");
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 动物掉落奖励事件回调
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            // 检查是否掉落子弹奖励
            if (!args.DropRewards.TryGetValue(EDropType.Bullet, out int bulletAmount) || bulletAmount <= 0)
                return;

            // 获取随机特殊子弹
            var specialBullet = _configManager.GetRandomSpecialBullet();

            // 切换到特殊子弹
            ChangeBullet(specialBullet.ID);
            Debug.Log($"[Weapon] 获得特殊子弹: {specialBullet.Name}，持续时间: {specialBullet.Duration}秒");
        }

        /// <summary>
        /// 触发子弹切换事件
        /// </summary>
        private void TriggerBulletChanged(BulletChangedEventArgs args)
        {
            _eventManager.Trigger(BulletEvents.BulletChanged, args);
        }

        /// <summary>
        /// 触发特殊子弹倒计时事件
        /// </summary>
        private void TriggerSpecialBulletCountdown(SpecialBulletCountdownEventArgs args)
        {
            _eventManager.Trigger(BulletEvents.SpecialBulletCountdown, args);
        }

        /// <summary>
        /// 触发特殊子弹效果结束事件
        /// </summary>
        private void TriggerSpecialBulletEffectEnded(SpecialBulletEffectEndedEventArgs args)
        {
            _eventManager.Trigger(BulletEvents.SpecialBulletEffectEnded, args);
        }
        #endregion

        #region TODO: 待音效系统待实现
        [Header("音效")]
        [SerializeField] private AudioClip fireAudioClip;
        private AudioSource _audioSource;

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void PlayFireSound()
        {
            if (fireAudioClip != null && _audioSource != null)
            {
                _audioSource.clip = fireAudioClip;
                _audioSource.Play();
            }
        }
        #endregion

    }
}
