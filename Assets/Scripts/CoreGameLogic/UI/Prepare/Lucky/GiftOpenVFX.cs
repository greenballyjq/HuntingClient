using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 礼包开箱视觉组件
/// </summary>
public class GiftOpenVFX : MonoBehaviour
{
    /// <summary>
    /// 礼包类型
    /// </summary>
    [SerializeField] private ELuckyGiftType _giftType;

    /// <summary>
    /// 礼包图像
    /// </summary>
    [SerializeField] private Image _imageGift;

    /// <summary>
    /// 礼包打开后的图片
    /// </summary>
    [SerializeField] private Sprite _giftOpenSprite;

    /// <summary>
    /// 礼包矩形变换组件
    /// </summary>
    [SerializeField] private RectTransform _giftRectTransform;

    /// <summary>
    /// 抖动时长
    /// </summary>
    [SerializeField] private float _shakeDuration = 0.5f;

    /// <summary>
    /// 抖动强度
    /// </summary>
    [SerializeField] private float _shakeStrength = 50f;
    /// <summary>
    /// 初始礼包图像
    /// </summary>
    private Sprite _originalGiftSprite;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    private void Awake()
    {
        _originalGiftSprite = _imageGift.sprite;

        _eventManager.AddListener(LuckyEvents.GiftOpened, OnGiftOpened);
        _eventManager.AddListener(LuckyEvents.LuckyBuffConfirmClicked, OnLuckyBuffConfirmClicked);
    }

    private void OnDestroy()
    {
        _eventManager.RemoveListener(LuckyEvents.GiftOpened, OnGiftOpened);
        _eventManager.RemoveListener(LuckyEvents.LuckyBuffConfirmClicked, OnLuckyBuffConfirmClicked);
    }

    #region 私有方法
    /// <summary>
    /// 播放礼包开启动画
    /// </summary>
    /// <param name="buffData">幸运仪式增益配置</param>
    private async UniTask PlayGiftAnimationAsync(LuckyBuff buffData)
    {
        // 触发礼包开启动画开始事件
        _eventManager.Trigger(LuckyEvents.GiftOpenAnimationStarted);

        // 播放抖动动画
        _giftRectTransform.DOShakePosition(_shakeDuration, _shakeStrength);
        await UniTask.Delay((int)(_shakeDuration * 1000));

        // 切换到打开后的图片
        _imageGift.sprite = _giftOpenSprite;

        // 触发礼包开启动画结束事件
        _eventManager.Trigger(LuckyEvents.GiftOpenAnimationEnded, new GiftOpenAnimationEndedEventArgs
        {
            LuckyBuffData = buffData
        });
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 礼包开启事件回调
    /// </summary>
    /// <param name="args">事件参数</param>
    private void OnGiftOpened(GiftOpenedEventArgs args)
    {
        // 判断是否匹配自己的礼包类型
        if (args.GiftType == _giftType)
            PlayGiftAnimationAsync(args.LuckyBuffData).Forget();
    }

    /// <summary>
    /// 幸运增益确认点击事件回调
    /// </summary>
    private void OnLuckyBuffConfirmClicked()
    {
        // 重置礼包图像到初始状态
        _imageGift.sprite = _originalGiftSprite;
    }
    #endregion
}
