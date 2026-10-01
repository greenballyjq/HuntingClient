using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 礼包组件
/// </summary>
public class UIComponentGift : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 礼包类型
    /// </summary>
    [SerializeField] private ELuckyGiftType _giftType;

    /// <summary>
    /// 礼包名称文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textGiftName;

    /// <summary>
    /// 价格文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textPrice;

    /// <summary>
    /// 购买按钮
    /// </summary>
    [SerializeField] private Button _buttonGift;

    /// <summary>
    /// 礼包价值
    /// </summary>
    private int _giftPrice;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager;

    /// <summary>
    /// 玩家数据管理器
    /// </summary>
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
        _buttonGift.onClick.AddListener(OnGiftButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonGift.onClick.RemoveListener(OnGiftButtonClicked);
    }

    public void Init()
    {
        InitializeGift();

        _eventManager.AddListener(LuckyEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
        _eventManager.AddListener(LuckyEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);
        _eventManager.AddListener(PlayerDataEvents.ThreeKPCoinAmountChanged, OnThreeKPCoinChanged);

        UpdateGiftButtonInteractable(_playerDataManager.GetThreeKPCoinAmount());
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(LuckyEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
        _eventManager.RemoveListener(LuckyEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);
        _eventManager.RemoveListener(PlayerDataEvents.ThreeKPCoinAmountChanged, OnThreeKPCoinChanged);
    }

    #region 私有方法
    /// <summary>
    /// 初始化礼包
    /// </summary>
    private void InitializeGift()
    {
        var gift = _configManager.GetLuckyGift(_giftType);

        _giftPrice = gift.ThreeKPCoin;
        _textGiftName.text = gift.Name;
        _textPrice.text = _giftPrice.ToString();
    }

    /// <summary>
    /// 开启礼包
    /// </summary>
    private void OpenGift()
    {
        // 抽到的幸运仪式增益配置
        LuckyBuff buff = _configManager.GetLuckyBuffByGiftAndWeights(_giftType);

        // 扣除三千盘金币
        _playerDataManager.UpdateThreeKPCoinAmount(-_giftPrice);

        // 触发礼包开启事件
        TriggerGiftOpened(new GiftOpenedEventArgs
        {
            GiftType = _giftType,
            LuckyBuffData = buff
        });
    }

    /// <summary>
    /// 更新礼包按钮交互状态
    /// </summary>
    /// <param name="currentCoin">当前三千盘金币数量</param>
    private void UpdateGiftButtonInteractable(int currentCoin)
    {
        _buttonGift.interactable = currentCoin >= _giftPrice;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 礼包按钮点击回调
    /// </summary>
    private void OnGiftButtonClicked()
    {
        OpenGift();
    }

    /// <summary>
    /// 礼包开启动画开始事件回调
    /// </summary>
    private void OnGiftOpenAnimationStarted()
    {
        _buttonGift.interactable = false;
    }

    /// <summary>
    /// 礼包开启动画结束事件回调
    /// </summary>
    private void OnGiftOpenAnimationEnded(GiftOpenAnimationEndedEventArgs args)
    {
        int currentCoin = _playerDataManager.GetThreeKPCoinAmount();
        UpdateGiftButtonInteractable(currentCoin);
    }

    /// <summary>
    /// 3币数量改变事件回调
    /// </summary>
    /// 
    private void OnThreeKPCoinChanged(ThreeKPCoinAmountChangedEventArgs args)
    {
        UpdateGiftButtonInteractable(args.CurrentAmount);
    }

    /// <summary>
    /// 触发礼包开启成功事件
    /// </summary>
    private void TriggerGiftOpened(GiftOpenedEventArgs args)
    {
        _eventManager.Trigger(LuckyEvents.GiftOpened, args);
    }
    #endregion
}
