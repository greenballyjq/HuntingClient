using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 普通结算弹窗
/// </summary>
[UIForm(UILayer.Popup)]
public class UIPopupNormalSettlement : UIForm
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
    /// 领取按钮
    /// </summary>
    [SerializeField] protected Button ButtonClaim;

    /// <summary>
    /// 打猎统计
    /// </summary>
    private List<(int specieId, int count)> _huntStatistics = new List<(int specieId, int count)>();

    /// <summary>
    /// 动物统计项
    /// </summary>
    private List<UIComponentSettlementAnimalStatItem> _animalStatItems = new List<UIComponentSettlementAnimalStatItem>();

    protected RoundFlow RoundFlow => HuntingAppFlow.Instance.RoundFlow;
    private HuntingConfigManager _configManager;
    private MeatProgressManager _meatProgressManager;
    private AnimalManager _animalManager;
    private SettlementManager _settlementManager;
    private PlayerDataManager _playerDataManager;
    private AudioManager _audioManager;
    private int _meatCoinReward;

    private void Awake()
    {
        BindServices();
        BindButtons();
    }

    private void OnDestroy()
    {
        UnbindButtons();
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
        _configManager = GameServiceLocator.ConfigManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        _settlementManager = GameServiceLocator.GetRoundManager<SettlementManager>();
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
        _audioManager = GameServiceLocator.AudioManager;
    }

    /// <summary>
    /// 刷新结算展示
    /// </summary>
    private void RefreshSettlementDisplay()
    {
        int meat = _meatProgressManager.CurrentMeatValue;
        var reward = _settlementManager.EvaluateReward();

        _imageMapIcon.sprite = _configManager.MapRefSo.GetMapIcon(RoundFlow.RoundContext.MapData.ID);
        _uiComponentSettlementStatItemMeat.SetCount(meat);
        _uiComponentSettlementStatItemThreeKPCoin.SetCount(0);
        _uiComponentSettlementStatItemPoint.SetCount(reward.Point);
        
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
    /// 绑定按钮
    /// </summary>
    protected virtual void BindButtons()
    {
        ButtonClaim.onClick.AddListener(OnClaimButtonClicked);
    }

    /// <summary>
    /// 解绑按钮
    /// </summary>
    protected virtual void UnbindButtons()
    {
        ButtonClaim.onClick.RemoveListener(OnClaimButtonClicked);
    }

    /// <summary>
    /// 设置按钮是否可交互
    /// </summary>
    protected virtual void SetButtonsInteractable(bool interactable)
    {
        ButtonClaim.interactable = interactable;
    }

    /// <summary>
    /// 播放结算开场音
    /// </summary>
    protected void PlaySettlementAudio()
    {
        _audioManager.PlayMusic(_configManager.UiAudioRefSo.SettlementVictory, 0f);
        _audioManager.Play(_configManager.RoleRefSo.Get(RoundFlow.RoundContext.RoleData.ID)?.Settlement);
    }

    /// <summary>
    /// 播放结算动画
    /// </summary>
    /// <returns></returns>
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
    /// 领取按钮点击事件回调
    /// </summary>
    protected void OnClaimButtonClicked()
    {
        SetButtonsInteractable(false);
        _settlementManager.ClaimMeatCoin();
        _playerDataManager.AddCompletedRound();
        _playerDataManager.Save();
        HuntingAppFlow.Instance.EnterPrepareAsync().Forget();
    }
    #endregion
}
