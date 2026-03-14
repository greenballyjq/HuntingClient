using cfg.HuntingConfig.Enum;
using GameFramework.Core.UI;
using GameFramework.Manager;
using TMPro;
using UnityEngine;

/// <summary>
/// 三千盘金币组件
/// </summary>
public class UIComponentThreeKPCoin : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 三千盘金币显示模式
    /// </summary>
    public enum EThreeKPCoinUpdateMode
    {
        /// <summary>
        /// 立即更新
        /// </summary>
        Immediate,

        /// <summary>
        /// 掉落动画后更新
        /// </summary>
        OnDropArrived
    }

    /// <summary>
    /// 三千盘金币数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textThreeKPCoinAmount;

    /// <summary>
    /// 更新模式
    /// </summary>
    [SerializeField] private EThreeKPCoinUpdateMode _updateMode = EThreeKPCoinUpdateMode.Immediate;

    /// <summary>
    /// 三千盘金币组件矩形变换
    /// </summary>
    private RectTransform _rectTransformThreeKPCoin;
    public RectTransform RectTransformThreeKPCoin => _rectTransformThreeKPCoin;

    private EventManager _eventManager;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        _rectTransformThreeKPCoin = _textThreeKPCoinAmount.GetComponent<RectTransform>();
        _eventManager = GameServiceLocator.EventManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
    }

    public void Init()
    {
        RefreshAmount(_playerDataManager.GetThreeKPCoinAmount());

        if (_updateMode == EThreeKPCoinUpdateMode.Immediate)
            _eventManager.AddListener(PlayerDataEvents.ThreeKPCoinAmountChanged, OnThreeKPCoinAmountChanged);
        else
            _eventManager.AddListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    public void CleanUp()
    {
        if (_updateMode == EThreeKPCoinUpdateMode.Immediate)
            _eventManager.RemoveListener(PlayerDataEvents.ThreeKPCoinAmountChanged, OnThreeKPCoinAmountChanged);
        else
            _eventManager.RemoveListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    private void OnThreeKPCoinAmountChanged(ThreeKPCoinAmountChangedEventArgs args)
    {
        RefreshAmount(args.CurrentAmount);
    }

    private void OnDropRewardArrived(RewardArrivedEventArgs args)
    {
        if (args.DropType == EDropType.ThreeKPCoin)
            RefreshAmount(_playerDataManager.GetThreeKPCoinAmount());
    }

    private void RefreshAmount(int amount)
    {
        _textThreeKPCoinAmount.text = amount.ToString();
    }
}
