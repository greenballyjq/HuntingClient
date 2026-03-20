using System.Collections.Generic;
using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 隐藏地图结算弹窗
/// </summary>
public class UIPopupHiddenSettlement : UIBase
{
    /// <summary>
    /// 肉量统计项
    /// </summary>
    [SerializeField] private UIComponentSettlementStatItem _uiComponentSettlementStatItemMeat;

    /// <summary>
    /// 三千盘金币统计项
    /// </summary>
    [SerializeField] private UIComponentSettlementStatItem _uiComponentSettlementStatItemThreeKPCoin;

    /// <summary>
    /// 积分统计项
    /// </summary>
    [SerializeField] private UIComponentSettlementStatItem _uiComponentSettlementStatItemPoint;

    /// <summary>
    /// 地图统计项容器
    /// </summary>
    [SerializeField] private RectTransform _rectTransformMapStatContainer;

    /// <summary>
    /// 地图统计项预制体
    /// </summary>
    [SerializeField] private GameObject _settlementMapStatItemPrefab;

    /// <summary>
    /// 分享按钮
    /// </summary>
    [SerializeField] protected Button ButtonShare;

    /// <summary>
    /// 领取按钮
    /// </summary>
    [SerializeField] protected Button ButtonClaim;

    /// <summary>
    /// 双倍领取按钮
    /// </summary>
    [SerializeField] protected Button ButtonDoubleClaim;

    /// <summary>
    /// 地图肉量统计
    /// </summary>
    private List<(int mapId, int meatAmount)> _meatPerMapStats = new List<(int mapId, int meatAmount)>();

    /// <summary>
    /// 地图统计项
    /// </summary>
    private List<UIComponentSettlementMapStatItem> _mapStatItems = new List<UIComponentSettlementMapStatItem>();

    protected RoundFlow RoundFlow => RoundFlow.Instance;
    private UIManager _uiManager;
    private HuntingConfigManager _configManager;
    private MeatProgressManager _meatProgressManager;

    private void Awake()
    {
        RegisterServers();
        ButtonShare.onClick.AddListener(OnShareButtonClicked);
        ButtonClaim.onClick.AddListener(OnClaimButtonClicked);
        ButtonDoubleClaim.onClick.AddListener(OnDoubleClaimButtonClicked);
    }

    private void OnDestroy()
    {
        ButtonShare.onClick.RemoveListener(OnShareButtonClicked);
        ButtonClaim.onClick.RemoveListener(OnClaimButtonClicked);
        ButtonDoubleClaim.onClick.RemoveListener(OnDoubleClaimButtonClicked);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _uiComponentSettlementStatItemMeat.Init();
        _uiComponentSettlementStatItemThreeKPCoin.Init();
        _uiComponentSettlementStatItemPoint.Init();

        SetButtonsInteractable(false);
        RefreshSettlementDisplay();
        PlaySettlementAsync().Forget();
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _uiManager = GameServiceLocator.UIManager;
        _configManager = GameServiceLocator.ConfigManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
    }

    /// <summary>
    /// 刷新结算展示
    /// </summary>
    private void RefreshSettlementDisplay()
    {
        int totalMeat = _meatProgressManager.TotalMeatAcrossMaps + _meatProgressManager.CurrentMeatValue;
        int point = _configManager.GetSettlementPointRewardAmount();

        _uiComponentSettlementStatItemMeat.SetCount(totalMeat);
        _uiComponentSettlementStatItemThreeKPCoin.SetCount(0);
        _uiComponentSettlementStatItemPoint.SetCount(point);

        RefreshMapStatItems();
    }

    /// <summary>
    /// 刷新地图统计项
    /// </summary>
    private void RefreshMapStatItems()
    {
        _meatPerMapStats.Clear();
        _meatProgressManager.GetMeatPerMapStatistics(_meatPerMapStats);

        for (int i = 0; i < _meatPerMapStats.Count; i++)
        {
            (int mapId, int meatAmount) = _meatPerMapStats[i];
            Map mapData = _configManager.GetMap(mapId);
            GameObject go = Instantiate(_settlementMapStatItemPrefab, _rectTransformMapStatContainer);
            var item = go.GetComponent<UIComponentSettlementMapStatItem>();
            item.Init(mapData);
            item.SetCount(meatAmount);
            _mapStatItems.Add(item);
        }
    }

    /// <summary>
    /// 设置按钮是否可交互
    /// </summary>
    private void SetButtonsInteractable(bool interactable)
    {
        ButtonShare.interactable = interactable;
        ButtonClaim.interactable = interactable;
        ButtonDoubleClaim.interactable = interactable;
    }

    /// <summary>
    /// 播放结算动画
    /// </summary>
    private async UniTask PlaySettlementAsync()
    {
        int totalMeat = _meatProgressManager.TotalMeatAcrossMaps + _meatProgressManager.CurrentMeatValue;
        int coinTarget = _meatProgressManager.GetThreeKPCoinCountFromMeat(totalMeat);

        await UniTask.Delay(2000);

        await UniTask.WhenAll(
            _uiComponentSettlementStatItemMeat.PlayCountAsync(0, 4f),
            _uiComponentSettlementStatItemThreeKPCoin.PlayCountAsync(coinTarget, 4f)
        );

        SetButtonsInteractable(true);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 分享按钮点击事件回调
    /// </summary>
    protected virtual void OnShareButtonClicked()
    {
    }

    /// <summary>
    /// 领取按钮点击事件回调
    /// </summary>
    protected virtual void OnClaimButtonClicked()
    {
        _uiManager.CloseUI("UISnowMountainVictory");
        Close();

        HuntingAppFlow.Instance.EnterPrepareAsync().Forget();
    }

    /// <summary>
    /// 双倍领取按钮点击事件回调
    /// </summary>
    protected virtual void OnDoubleClaimButtonClicked()
    {
    }
    #endregion
}
