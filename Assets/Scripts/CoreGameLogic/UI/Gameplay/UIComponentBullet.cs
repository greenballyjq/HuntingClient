using Cysharp.Threading.Tasks;
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
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    public RectTransform BulletTransform => _imageBullet.GetComponent<RectTransform>();

    public void Init()
    {
        UpdateCountdown(false, 0, 0);

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

    private void OnDestroy()
    {
        CleanUp();
    }

    #region 私有方法
    /// <summary>
    /// 更新子弹图标
    /// </summary>
    /// <param name="bulletData">子弹配置</param>
    private async UniTask UpdateBulletIconAsync(Bullet bulletData)
    {
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(bulletData.IconResourcePath);
        _imageBullet.sprite = sprite;
    }

    /// <summary>
    /// 更新倒计时显示
    /// </summary>
    /// <param name="isSpecial">是否为特殊子弹</param>
    /// <param name="remainingTime">剩余时间</param>
    /// <param name="totalTime">总时间</param>
    private void UpdateCountdown(bool isSpecial, float remainingTime = 0f, float totalTime = 0f)
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

        // 更新子弹图标
        UpdateBulletIconAsync(args.NewBulletData).Forget();

        // 更新倒计时显示
        UpdateCountdown(args.IsSpecialBullet, args.RemainingTime, _duration);
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

        // 更新倒计时显示
        UpdateCountdown(true, args.RemainingTime, args.BulletData.Duration);
    }

    /// <summary>
    /// 特殊子弹效果结束事件回调
    /// </summary>
    private void OnSpecialBulletEffectEnded(SpecialBulletEffectEndedEventArgs args)
    {
        _isSpecialBullet = false;
        _remainingTime = 0f;
        _duration = 0f;
    }
    #endregion
}