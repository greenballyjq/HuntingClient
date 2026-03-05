using cfg.HuntingConfig.Enum;
using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;
using Hunting.Events;
using UnityEngine;

/// <summary>
/// 玩家武器类
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
    /// <summary>
    /// 旋转速度
    /// </summary>
    [SerializeField] private float _rotationSpeed;

    /// <summary>
    /// 开火点
    /// </summary>
    [SerializeField] private Transform _firePoint;

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
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    /// <summary>
    /// 子弹管理器
    /// </summary>
    private BulletManager _bulletManager => GameServiceLocator.GetRoundManager<BulletManager>();

    /// <summary>
    /// 打猎音效管理器
    /// </summary>
    private HuntingSoundManager _huntingSoundManager => GameServiceLocator.GetAppManager<HuntingSoundManager>();

    /// <summary>
    /// 武器视觉组件
    /// </summary>
    public WeaponVisual WeaponVisual { get; private set; }

    private void Awake()
    {
        WeaponVisual = GetComponent<WeaponVisual>();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    #region 公共方法
    /// <summary>
    /// 初始化武器
    /// </summary>
    public void Init()
    {
        RegisterEvents();
        // 设置默认子弹
        ChangeBullet(_currentBulletId);
        UpdateFireInterval();
    }

    /// <summary>
    /// 更新特殊子弹倒计时
    /// </summary>
    public void UpdateSpecialBulletTimer()
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
        }
    }

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
        Vector3 direction = worldPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    /// <summary>
    /// 旋转武器
    /// </summary>
    /// <param name="deltaAngle">增量角度（度）</param>
    public void RotateWeapon(float deltaAngle)
    {
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0f, transform.forward.z);
        if (forwardXZ.sqrMagnitude < 0.0001f)
            return;

        float currentAngle = Vector3.SignedAngle(Vector3.forward, forwardXZ.normalized, Vector3.up);
        float newAngle = Mathf.Clamp(currentAngle + deltaAngle, -70f, 70f);

        Vector3 direction = Quaternion.Euler(0f, newAngle, 0f) * Vector3.forward;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }


    /// <summary>
    /// 尝试射击
    /// </summary>
    public void TryFire()
    {
        // 检查射击间隔
        if (Time.time - _lastFireTime < _fireInterval)
            return;

        // 执行射击
        SpawnBulletAsync().Forget();
        _huntingSoundManager.PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.GunShoot_Default);
        _lastFireTime = Time.time;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 更新射击间隔
    /// </summary>
    private void UpdateFireInterval()
    {
        float fireRate = _weaponManager.GetCurrentFireRate(_currentBulletId);
        _fireInterval = 1f / fireRate;
    }

    /// <summary>
    /// 生成子弹
    /// </summary>
    private async UniTask SpawnBulletAsync()
    {
        await _bulletManager.SpawnBullet(_currentBulletId, _firePoint.position, _firePoint.forward);
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

        var oldBulletData = _configManager.GetBullet(oldBulletId);

        // 判断是否为特殊子弹
        bool isSpecialBullet = newBulletData.BulletType != EBulletType.Normal;
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
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }
    
    /// <summary>
    /// 掉落奖励生效事件回调
    /// </summary>
    private void OnDropRewardArrived(RewardArrivedEventArgs args)
    {
        if (args.DropType == EDropType.Bullet)
            ChangeBullet(_configManager.GetRandomSpecialBullet().ID);
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
}
