using GameFramework.Game.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 幸运仪式界面
/// </summary>
public class UILucky : UIBase
{
    /// <summary>
    /// 3币数量文本
    /// </summary>
    [SerializeField] private Text _textThreeCoinAmount;

    /// <summary>
    /// 关闭按钮
    /// </summary>
    [SerializeField] private Button _buttonClose;

    /// <summary>
    /// 礼包组件列表
    /// </summary>
    [SerializeField] private UIComponentGift[] _uiComponentGifts;

    /// <summary>
    /// 广告礼包组件列表
    /// </summary>
    [SerializeField] private UIComponentGiftAd[] _uiComponentGiftAds;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 玩家数据管理器
    /// </summary>
    private PlayerDataManager PlayerData => GameServiceLocator.GetAppManager<PlayerDataManager>();

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    private void Awake()
    {
        _buttonClose.onClick.AddListener(OnClickClose);
    }

    private void OnDestroy()
    {
        _buttonClose.onClick.RemoveListener(OnClickClose);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        foreach (var gift in _uiComponentGifts)
            gift.Init();

        foreach (var giftAd in _uiComponentGiftAds)
            giftAd.Init();

        UpdateThreeCoinDisplay(PlayerData.GetThreeKPCoin());

        _eventManager.AddListener(PlayerDataEvents.ThreeKPCoinChanged, OnThreeKPCoinChanged);
        _eventManager.AddListener(LuckyEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
        _eventManager.AddListener(LuckyEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);
    }

    public override void OnClose()
    {
        foreach (var gift in _uiComponentGifts)
            gift.CleanUp();

        foreach (var giftAd in _uiComponentGiftAds)
            giftAd.CleanUp();

        _eventManager.RemoveListener(PlayerDataEvents.ThreeKPCoinChanged, OnThreeKPCoinChanged);
        _eventManager.RemoveListener(LuckyEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
        _eventManager.RemoveListener(LuckyEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);

        base.OnClose();
    }

    #region 私有方法
    /// <summary>
    /// 更新3币显示
    /// </summary>
    /// <param name="coinAmount">3币数量</param>
    private void UpdateThreeCoinDisplay(int coinAmount)
    {
        _textThreeCoinAmount.text = coinAmount.ToString();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 关闭按钮点击回调
    /// </summary>
    private void OnClickClose()
    {
        Close();
    }

    /// <summary>
    /// 3币数量改变事件回调
    /// </summary>
    private void OnThreeKPCoinChanged(ThreeKPCoinChangedEventArgs args)
    {
        UpdateThreeCoinDisplay(args.CurrentAmount);
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

        await _uiManager.OpenUIAsync<UILuckyBuffShow>("UILuckyBuffShow", UIManager.UILayer.PopUp, args.LuckyBuffData);
    }
    #endregion
}
