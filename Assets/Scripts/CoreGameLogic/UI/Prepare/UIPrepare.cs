using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 准备界面
/// </summary>
public class UIPrepare : UIBase
{
    /// <summary>
    /// 随机角色组件
    /// </summary>
    [SerializeField] private UIComponentRollRole _uiComponentRollRole;

    /// <summary>
    /// 角色信息组件
    /// </summary>
    [SerializeField] private UIComponentRoleInfo _uiComponentRoleInfo;

    /// <summary>
    /// 技能信息组件
    /// </summary>
    [SerializeField] private UIComponentSkillInfo _uiComponentSkillInfo;

    /// <summary>
    /// 地图信息组件
    /// </summary>
    [SerializeField] private UIComponentMapInfo _uiComponentMapInfo;

    /// <summary>
    /// 开始单局按钮
    /// </summary>
    [SerializeField] private Button _buttonStartRound;

    /// <summary>
    /// 幸运仪式按钮
    /// </summary>
    [SerializeField] private Button _buttonLucky;

    /// <summary>
    /// 排行榜按钮
    /// </summary>
    [SerializeField] private Button _buttonRanking;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 当前选中的角色ID
    /// </summary>
    private int _currentRoleId;

    /// <summary>
    /// 当前选中幸运仪式增益数据
    /// </summary>
    private LuckyBuff _currentLuckyBuffData;

    private void Awake()
    {
        _buttonStartRound.onClick.AddListener(OnClickStartRound);
        _buttonLucky.onClick.AddListener(OnClickLuckyRitual);
        _buttonRanking.onClick.AddListener(OnClickRanking);
    }

    private void OnDestroy()
    {
        _buttonStartRound.onClick.RemoveListener(OnClickStartRound);
        _buttonLucky.onClick.RemoveListener(OnClickLuckyRitual);
        _buttonRanking.onClick.RemoveListener(OnClickRanking);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _uiComponentRollRole.Init();
        _uiComponentRoleInfo.Init();
        _uiComponentSkillInfo.Init();
        _uiComponentMapInfo.Init();

        // 默认开始单据单局按钮不可用
        _buttonStartRound.interactable = false;

        _eventManager.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.AddListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.AddListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
        _eventManager.AddListener(LuckyEvents.GiftOpened, OnGiftOpened);
    }

    public override void OnClose()
    {
        _uiComponentRollRole.CleanUp();
        _uiComponentRoleInfo.CleanUp();
        _uiComponentSkillInfo.CleanUp();
        _uiComponentMapInfo.CleanUp();

        _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.RemoveListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.RemoveListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
        _eventManager.RemoveListener(LuckyEvents.GiftOpened, OnGiftOpened);

        base.OnClose();
    }

    #region 事件相关
    /// <summary>
    /// 角色选中事件回调
    /// </summary>
    private void OnRoleSelected(RoleSelectedEventArgs args)
    {
        _currentRoleId = args.RoleId;
    }

    /// <summary>
    /// 骰子动画开始事件
    /// </summary>
    private void OnDiceAnimationStarted()
    {
        _buttonStartRound.interactable = false;
        _buttonLucky.interactable = false;
        _buttonRanking.interactable = false;
    }

    /// <summary>
    /// 走格子动画结束事件
    /// </summary>
    private void OnSlotAnimationEnded()
    {
        _buttonLucky.interactable = true;
        _buttonStartRound.interactable = true;
        _buttonRanking.interactable = true;
    }

    /// <summary>
    /// 开始单局按钮回调
    /// </summary>
    private void OnClickStartRound()
    {
        Role roleData = _configManager.GetRole(_currentRoleId);
        int mapId = _uiComponentMapInfo.GetCurrentMapId();
        Map mapData = _configManager.GetMap(mapId);
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);

        // TODO: 测试，未来根据不同地图加载不同场景，以及判断是否有隐藏地图
        SceneManager.LoadSceneAsync("GameplayForestScene").completed += (ao) =>
        {
            Close();
            HuntingAppFlow.Instance.EnterRound(new RoundContext
            {
                RoleData = roleData,
                MapData = mapData,
                SkillData = skillData,
                LuckyBuffData = _currentLuckyBuffData,
                HasLinkage = roleData.LinkedMapId == mapId,
                HasHiddenMap = true,
                HiddenMapData = _configManager.GetMap(EMapType.Hidden)
            }).Forget();
            DynamicGI.UpdateEnvironment();
        };
    }

    /// <summary>
    /// 幸运仪式按钮回调
    /// </summary>
    private async void OnClickLuckyRitual()
    {
        await _uiManager.OpenUIAsync<UIPopupLucky>("UIPopupLucky",UIManager.UILayer.PopUp);
    }

    /// <summary>
    /// 礼包开启事件回调
    /// </summary>
    private void OnGiftOpened(GiftOpenedEventArgs args)
    {
        _currentLuckyBuffData = args.LuckyBuffData;
    }

    /// <summary>
    /// 排行榜按钮回调
    /// </summary>
    private async void OnClickRanking()
    {
        var rankingData = _configManager.GetMockRankingData();
        await _uiManager.OpenUIAsync<UIRanking>("UIRanking", UIManager.UILayer.PopUp, rankingData);
    }
    #endregion
}
