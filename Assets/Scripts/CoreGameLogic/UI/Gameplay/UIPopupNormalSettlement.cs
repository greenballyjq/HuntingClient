using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using CoreGameLogic.Net;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 普通结算弹窗
/// </summary>
public class UIPopupNormalSettlement : UIBase
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
    /// 地图图标
    /// </summary>
    [SerializeField] private Image _imageMapIcon;

    /// <summary>
    /// 动物统计项容器
    /// </summary>
    [SerializeField] private RectTransform _rectTransformAnimalStatContainer;

    /// <summary>
    /// 动物统计项预制体
    /// </summary>
    [SerializeField] private GameObject _settlementAnimalStatItemPrefab;

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
    /// 打猎统计
    /// </summary>
    private List<(int specieId, int count)> _huntStatistics = new List<(int specieId, int count)>();

    /// <summary>
    /// 动物统计项
    /// </summary>
    private List<UIComponentSettlementAnimalStatItem> _animalStatItems = new List<UIComponentSettlementAnimalStatItem>();

    protected RoundFlow RoundFlow => RoundFlow.Instance;
    private HuntingConfigManager _configManager;
    private MeatProgressManager _meatProgressManager;
    private AnimalManager _animalManager;

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
        _configManager = GameServiceLocator.ConfigManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
    }

    /// <summary>
    /// 刷新结算展示
    /// </summary>
    private void RefreshSettlementDisplay()
    {
        int meat = _meatProgressManager.CurrentMeatValue;
        int point = _configManager.GetSettlementPointRewardAmount();

        _imageMapIcon.sprite = _configManager.MapRefSo.GetMapIcon(RoundFlow.RoundContext.MapData.ID);
        _uiComponentSettlementStatItemMeat.SetCount(meat);
        _uiComponentSettlementStatItemThreeKPCoin.SetCount(0);
        _uiComponentSettlementStatItemPoint.SetCount(point);
        
        RefreshAnimalStatItems();
    }

    /// <summary>
    /// 刷新动物统计项
    /// </summary>
    private void RefreshAnimalStatItems()
    {
        _huntStatistics.Clear();
        _animalManager.GetHuntStatistics(_huntStatistics, new[]
        {
            ESpecieType.Small,
            ESpecieType.Medium,
            ESpecieType.Large
        });

        for (int i = 0; i < _huntStatistics.Count; i++)
        {
            (int specieId, int count) = _huntStatistics[i];
            GameObject go = Instantiate(_settlementAnimalStatItemPrefab, _rectTransformAnimalStatContainer);
            var item = go.GetComponent<UIComponentSettlementAnimalStatItem>();
            item.Init(specieId);
            item.SetCount(count);
            item.SetMeatValue(_animalManager.GetMeatAmount(specieId));
            _animalStatItems.Add(item);
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
    /// <returns></returns>
    private async UniTask PlaySettlementAsync()
    {
        int totalMeat = _meatProgressManager.TotalMeatAcrossMaps + _meatProgressManager.CurrentMeatValue;
        int coinTarget = _meatProgressManager.GetThreeKPCoinCountFromMeat(totalMeat);

        // 等待固定时长控制节奏
        await UniTask.Delay(2000);

        // 播放结算动画
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
