using cfg.HuntingConfig;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;
using GameFramework.Manager;
using GameFramework.UI;
using TMPro;

/// <summary>
/// 子弹UI组件
/// </summary>
public class UIComponentBullet : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 子弹图像
    /// </summary>
    [SerializeField] private Image _imageBullet;

    /// <summary>
    /// 倒计时文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCountdown;

    /// <summary>
    /// 子弹图像矩形变换组件
    /// </summary>
    private RectTransform _rectTransformBulletImage;
    public RectTransform RectTransformBulletImage => _rectTransformBulletImage;

    /// <summary>
    /// 当前子弹ID
    /// </summary>
    private int _currentBulletId;

    /// <summary>
    /// 是否为特殊子弹
    /// </summary>
    private bool _isSpecialBullet;

    /// <summary>
    /// 特殊子弹剩余时间
    /// </summary>
    private float _remainingTime;

    /// <summary>
    /// 特殊子弹持续时间
    /// </summary>
    private float _duration;

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;
    private WeaponManager _weaponManager;

    private void Awake()
    {
        _rectTransformBulletImage = _imageBullet.GetComponent<RectTransform>();
        BindServices();
    }

    public void Init()
    {
        SyncFromManager();
        _eventManager.AddListener(BulletEvents.BulletChanged, OnBulletChanged);
        _eventManager.AddListener(BulletEvents.SpecialBulletCountdown, OnSpecialBulletCountdown);
        _eventManager.AddListener(BulletEvents.SpecialBulletEffectEnded, OnSpecialBulletEffectEnded);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(BulletEvents.BulletChanged, OnBulletChanged);
        _eventManager.RemoveListener(BulletEvents.SpecialBulletCountdown, OnSpecialBulletCountdown);
        _eventManager.RemoveListener(BulletEvents.SpecialBulletEffectEnded, OnSpecialBulletEffectEnded);
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
    }

    /// <summary>
    /// 从管理器同步当前状态并刷新显示
    /// </summary>
    private void SyncFromManager()
    {
        var playerWeapon = _weaponManager.PlayerWeapon;
        var bulletData = _configManager.GetBullet(playerWeapon.CurrentBulletID);

        RefreshBulletIcon(bulletData);
        RefreshCountdown(false, 0, 0);
    }

    /// <summary>
    /// 刷新子弹图标
    /// </summary>
    private void RefreshBulletIcon(Bullet bulletData)
    {
        var sprite = _configManager.BulletRefSo.GetBulletIcon(bulletData.ID);
        _imageBullet.sprite = sprite;
    }

    /// <summary>
    /// 刷新倒计时显示
    /// </summary>
    private void RefreshCountdown(bool isSpecial, float remainingTime = 0f, float totalTime = 0f)
    {
        if (!isSpecial)
            _textCountdown.text = "";
        else
            _textCountdown.text = Mathf.CeilToInt(remainingTime).ToString();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 子弹切换事件回调
    /// </summary>
    private void OnBulletChanged(BulletChangedEventArgs args)
    {
        _currentBulletId = args.NewBulletData.ID;
        _isSpecialBullet = args.IsSpecialBullet;
        _remainingTime = args.RemainingTime;
        _duration = args.NewBulletData.Duration;

        RefreshBulletIcon(args.NewBulletData);
        RefreshCountdown(args.IsSpecialBullet, args.RemainingTime, _duration);
    }

    /// <summary>
    /// 特殊子弹倒计时事件回调
    /// </summary>
    private void OnSpecialBulletCountdown(SpecialBulletCountdownEventArgs args)
    {
        if (args.BulletData.ID != _currentBulletId)
            return;

        _remainingTime = args.RemainingTime;
        _duration = args.BulletData.Duration;

        RefreshCountdown(true, args.RemainingTime, args.BulletData.Duration);
    }

    /// <summary>
    /// 特殊子弹效果结束事件回调
    /// </summary>
    private void OnSpecialBulletEffectEnded(SpecialBulletEffectEndedEventArgs args)
    {
        _isSpecialBullet = false;
        _remainingTime = 0f;
        _duration = 0f;

        RefreshCountdown(false, 0, 0);
    }
    #endregion
}
