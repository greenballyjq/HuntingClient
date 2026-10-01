using System.Collections.Generic;
using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 隐藏地图结算弹窗
/// </summary>
[UIForm(UILayer.Popup)]
public class UIPopupHiddenSettlement : UIForm
{
    private const int CountUpDelayMs = 2000;
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
    /// 领取按钮
    /// </summary>
    [SerializeField] protected Button ButtonClaim;

    /// <summary>
    /// 地图肉量统计
    /// </summary>
    private List<(int mapId, int meatAmount)> _meatPerMapStats = new List<(int mapId, int meatAmount)>();

    /// <summary>
    /// 地图统计项
    /// </summary>
    private List<UIComponentSettlementMapStatItem> _mapStatItems = new List<UIComponentSettlementMapStatItem>();

    protected RoundFlow RoundFlow => HuntingAppFlow.Instance.RoundFlow;
    private UIManager _uiManager;
    private HuntingConfigManager _configManager;
    private MeatProgressManager _meatProgressManager;
    private SettlementManager _settlementManager;
    private PlayerDataManager _playerDataManager;
    private AudioManager _audioManager;
    private int _meatCoinReward;

    private void Awake()
    {
        BindServices();
        ButtonClaim.onClick.AddListener(OnClaimButtonClicked);
    }

    private void OnDestroy()
    {
        ButtonClaim.onClick.RemoveListener(OnClaimButtonClicked);
    }

    protected override void OnOpen()
    {

        _uiComponentSettlementStatItemMeat.Init();
        _uiComponentSettlementStatItemThreeKPCoin.Init();
        _uiComponentSettlementStatItemPoint.Init();

        SetButtonsInteractable(false);
        RefreshSettlementDisplay();
        PlaySettlementAudio();
        PlaySettlementAsync().Forget();
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _configManager = GameServiceLocator.ConfigManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
        _settlementManager = GameServiceLocator.GetRoundManager<SettlementManager>();
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
        _audioManager = GameServiceLocator.AudioManager;
    }

    /// <summary>
    /// 刷新结算展示
    /// </summary>
    private void RefreshSettlementDisplay()
    {
        int totalMeat = _meatProgressManager.TotalMeatAcrossMaps + _meatProgressManager.CurrentMeatValue;
        var reward = _settlementManager.EvaluateReward();

        _uiComponentSettlementStatItemMeat.SetCount(totalMeat);
        _uiComponentSettlementStatItemThreeKPCoin.SetCount(0);
        _uiComponentSettlementStatItemPoint.SetCount(reward.Point);

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
        ButtonClaim.interactable = interactable;
    }

    private void PlaySettlementAudio()
    {
        _audioManager.PlayMusic(_configManager.UiAudioRefSo.SettlementVictory, 0f);
        _audioManager.Play(_configManager.RoleRefSo.Get(RoundFlow.RoundContext.RoleData.ID)?.Settlement);
    }

    /// <summary>
    /// 播放结算动画
    /// </summary>
    private async UniTask PlaySettlementAsync()
    {
        var reward = _settlementManager.EvaluateReward();
        _meatCoinReward = reward.CoinFromMeat;

        await UniTask.Delay(CountUpDelayMs);

        await UniTask.WhenAll(
            _uiComponentSettlementStatItemMeat.PlayCountAsync(0, 4f),
            _uiComponentSettlementStatItemThreeKPCoin.PlayCountAsync(_meatCoinReward, 4f)
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
        SetButtonsInteractable(false);
        _settlementManager.ClaimMeatCoin();
        _playerDataManager.AddCompletedRound();
        _playerDataManager.Save();
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
