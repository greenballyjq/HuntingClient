using GameFramework.Core.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 三千盘金币组件
/// </summary>
public class UIComponentThreeKPCoin : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 三千盘金币数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textThreeKPCoinAmount;

    /// <summary>
    /// 三千盘金币组件矩形变换
    /// </summary>
    private RectTransform _rectTransformThreeKPCoin;
    public RectTransform RectTransformThreeKPCoin => _rectTransformThreeKPCoin;

    private EventManager _eventManager;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        RegisterServers();
        _rectTransformThreeKPCoin = _textThreeKPCoinAmount.GetComponent<RectTransform>();
    }

    public void Init()
    {
        RefreshAmount(_playerDataManager.GetThreeKPCoinAmount());
        _eventManager.AddListener(PlayerDataEvents.ThreeKPCoinAmountChanged, OnThreeKPCoinAmountChanged);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(PlayerDataEvents.ThreeKPCoinAmountChanged, OnThreeKPCoinAmountChanged);
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _eventManager = GameServiceLocator.EventManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
    }

    /// <summary>
    /// 刷新金币数量显示
    /// </summary>
    private void RefreshAmount(int amount)
    {
        _textThreeKPCoinAmount.text = amount.ToString();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 三千盘金币数量变化事件回调
    /// </summary>
    private void OnThreeKPCoinAmountChanged(ThreeKPCoinAmountChangedEventArgs args)
    {
        RefreshAmount(args.CurrentAmount);
    }
    #endregion
}
