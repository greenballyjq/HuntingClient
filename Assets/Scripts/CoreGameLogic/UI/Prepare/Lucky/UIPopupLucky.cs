using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 幸运仪式界面
/// </summary>
public class UIPopupLucky : UIBase
{
    /// <summary>
    /// 三千盘金币组件
    /// </summary>
    [SerializeField] private UIComponentThreeKPCoin _uiComponentThreeKPCoin;

    /// <summary>
    /// 礼包组件列表
    /// </summary>
    [SerializeField] private UIComponentGift[] _uiComponentGifts;

    /// <summary>
    /// 广告礼包组件列表
    /// </summary>
    [SerializeField] private UIComponentGiftAd[] _uiComponentGiftAds;

    /// <summary>
    /// 关闭按钮
    /// </summary>
    [SerializeField] private Button _buttonClose;

    private EventManager _eventManager;
    private UIManager _uiManager;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;

        _buttonClose.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonClose.onClick.RemoveListener(OnCloseButtonClicked);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _eventManager.AddListener(LuckyEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
        _eventManager.AddListener(LuckyEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);

        _uiComponentThreeKPCoin.Init();

        foreach (var gift in _uiComponentGifts)
            gift.Init();

        foreach (var giftAd in _uiComponentGiftAds)
            giftAd.Init();
    }

    public override void OnClose()
    {
        _uiComponentThreeKPCoin.CleanUp();

        foreach (var gift in _uiComponentGifts)
            gift.CleanUp();

        foreach (var giftAd in _uiComponentGiftAds)
            giftAd.CleanUp();

        _eventManager.RemoveListener(LuckyEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
        _eventManager.RemoveListener(LuckyEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);

        base.OnClose();
    }

    #region 事件相关
    /// <summary>
    /// 关闭按钮点击回调
    /// </summary>
    private void OnCloseButtonClicked()
    {
        Close();
    }

    /// <summary>
    /// 礼包开启动画开始事件回调
    /// </summary>
    private void OnGiftOpenAnimationStarted()
    {
        _buttonClose.interactable = false;
    }

    /// <summary>
    /// 礼包开启动画结束事件回调
    /// </summary>
    private async void OnGiftOpenAnimationEnded(GiftOpenAnimationEndedEventArgs args)
    {
        _buttonClose.interactable = true;

        await _uiManager.OpenUIAsync<UIPopupLuckyBuff>("UIPopupLuckyBuff", UIManager.UILayer.PopUp, args.LuckyBuffData);
    }
    #endregion
}
