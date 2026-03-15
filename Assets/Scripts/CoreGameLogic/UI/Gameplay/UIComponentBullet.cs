using cfg.HuntingConfig;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;
using GameFramework.Manager;
using GameFramework.Core.UI;
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

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager;

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager;

    private void Awake()
    {
        _rectTransformBulletImage = _imageBullet.GetComponent<RectTransform>();

        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
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
    /// 从管理器同步当前状态并刷新显示
    /// </summary>
    private void SyncFromManager()
    {
        var playerWeapon = _weaponManager?.PlayerWeapon;
        if (playerWeapon != null)
        {
            var bulletData = _configManager.GetBullet(playerWeapon.CurrentBulletID);
            if (bulletData != null)
            {
                RefreshBulletIcon(bulletData);
            }
        }

        RefreshCountdown(false, 0, 0);
    }

    /// <summary>
    /// 刷新子弹图标
    /// </summary>
    /// <param name="bulletData">子弹配置</param>
    private void RefreshBulletIcon(Bullet bulletData)
    {
        var sprite = _configManager.BulletRefSo.GetBulletIcon(bulletData.ID);
        _imageBullet.sprite = sprite;
    }

    /// <summary>
    /// 刷新倒计时显示
    /// </summary>
    /// <param name="isSpecial">是否为特殊子弹</param>
    /// <param name="remainingTime">剩余时间</param>
    /// <param name="totalTime">总时间</param>
    private void RefreshCountdown(bool isSpecial, float remainingTime = 0f, float totalTime = 0f)
    {
        if (!isSpecial)
            _textCountdown.text = "∞";
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
