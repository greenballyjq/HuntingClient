using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using CoreGameLogic.Managers.AppManagers;
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
    /// 准备界面测试组件
    /// </summary>
    [SerializeField] private UIComponentPrepareTest _uiComponentPrepareTest;

    /// <summary>
    /// 调试组件
    /// </summary>
    [SerializeField] private UIComponentPrepareDebug _uiComponentPrepareDebug;

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
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 打猎音效管理器
    /// </summary>
    private HuntingSoundManager _soundManager => GameServiceLocator.GetAppManager<HuntingSoundManager>();

    /// <summary>
    /// CG管理器
    /// </summary>
    private CGManager _cgManager => GameServiceLocator.GetAppManager<CGManager>();

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
        _uiComponentPrepareTest.Init();
        _uiComponentPrepareDebug.Init();

        _eventManager.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.AddListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.AddListener(LuckyEvents.GiftOpened, OnGiftOpened);
    }

    public override void OnClose()
    {
        _uiComponentRollRole.CleanUp();
        _uiComponentRoleInfo.CleanUp();
        _uiComponentSkillInfo.CleanUp();
        _uiComponentMapInfo.CleanUp();
        _uiComponentPrepareTest.CleanUp();
        _uiComponentPrepareDebug.CleanUp();

        _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.RemoveListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.RemoveListener(LuckyEvents.GiftOpened, OnGiftOpened);

        base.OnClose();
    }

    #region 事件相关
    /// <summary>
    /// 开始单局按钮回调
    /// </summary>
    private async void OnClickStartRound()
    {
        // 播放投骰子动画
        await _uiComponentRollRole.PlayRollDiceAsync();

        Role roleData = _configManager.GetRole(_currentRoleId);
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);
        Map mapData = _uiComponentMapInfo.CurrentMapData;

        // 是否有隐藏地图 TODO: 以后转移到配置表
        bool hasHiddenMap = Random.Range(0, 100) < 100;

        // 创建角色CG视频
        if (hasHiddenMap)
            await _cgManager.CreateCGAsync(roleData.VideoResourcePath);

        // 播放角色语音
        if(roleData.RoleType == ERoleType.Bule || roleData.RoleType == ERoleType.Red)
        {
            await UniTask.Delay(2000);
        }
        else
        {
            UniTaskCompletionSource audioCompletionSource = new UniTaskCompletionSource();
            _soundManager.PlayIPOpening(roleData.RoleType).AddCallback(player =>
            {
                audioCompletionSource.TrySetResult();
            });

            await audioCompletionSource.Task;
        }

        // 加载地图
        await SceneManager.LoadSceneAsync("GameplayForestScene").ToUniTask();
        DynamicGI.UpdateEnvironment();

        // 播放角色CG视频
        if (hasHiddenMap)
        {
#if !UNITY_EDITOR
                await _cgManager.PlayCGAsync();
#endif
        }

        _uiManager.CloseUI("UIPrepare");

        // 进入单局
        await HuntingAppFlow.Instance.EnterRound(new RoundContext
        {
            RoleData = roleData,
            MapData = mapData,
            SkillData = skillData,
            LuckyBuffData = _currentLuckyBuffData,
            HasLinkage = roleData.LinkedMapId == mapData.ID,
            HasHiddenMap = hasHiddenMap,
            HiddenMapData = hasHiddenMap ? _configManager.GetMap(EMapType.Hidden) : null
        });
    }

    /// <summary>
    /// 幸运仪式按钮回调
    /// </summary>
    private async void OnClickLuckyRitual()
    {
        await _uiManager.OpenUIAsync<UIPopupLucky>("UIPopupLucky",UIManager.UILayer.PopUp);
    }

    /// <summary>
    /// 排行榜按钮回调
    /// </summary>
    private async void OnClickRanking()
    {
        var rankingData = _configManager.GetMockRankingData();
        await _uiManager.OpenUIAsync<UIRanking>("UIRanking", UIManager.UILayer.PopUp, rankingData);
    }

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
    /// 礼包开启事件回调
    /// </summary>
    private void OnGiftOpened(GiftOpenedEventArgs args)
    {
        _currentLuckyBuffData = args.LuckyBuffData;
    }

    #endregion
}
