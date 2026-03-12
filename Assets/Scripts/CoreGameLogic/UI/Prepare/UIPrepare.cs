using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Audio;
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

    private EventManager _eventManager;
    private UIManager _uiManager;
    private HuntingConfigManager _configManager;
    private HuntingSoundManager _soundManager;
    private CGManager _cgManager;

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
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _configManager = GameServiceLocator.ConfigManager;
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
        _cgManager = GameServiceLocator.GetAppManager<CGManager>();

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

        RegisterEvents();

        _uiComponentRollRole.Init();
        _uiComponentRoleInfo.Init();
        _uiComponentSkillInfo.Init();
        _uiComponentMapInfo.Init();
        _uiComponentPrepareDebug.Init();
        _uiComponentPrepareDebug.gameObject.SetActive(false);
    }

    public override void OnClose()
    {
        _uiComponentRollRole.CleanUp();
        _uiComponentRoleInfo.CleanUp();
        _uiComponentSkillInfo.CleanUp();
        _uiComponentMapInfo.CleanUp();
        _uiComponentPrepareDebug.CleanUp();

        UnregisterEvents();

        base.OnClose();
    }

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    public void RegisterEvents()
    {
        _eventManager.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.AddListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.AddListener(LuckyEvents.GiftOpened, OnGiftOpened);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    public void UnregisterEvents()
    {
        _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.RemoveListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.RemoveListener(LuckyEvents.GiftOpened, OnGiftOpened);
    }

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
            await UniTask.Delay(2000);
        else
            await _soundManager.PlayIPOpening(roleData.RoleType).ToUniTask();

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

    #region 开发者测试方法
    /// <summary>指定角色直接开始，跳过投骰子</summary>
    public async void StartRoundWithRole(int roleId)
    {
        _buttonStartRound.interactable = false;
        _buttonLucky.interactable = false;
        _buttonRanking.interactable = false;

        Role roleData = _configManager.GetRole(roleId);
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);
        Map mapData = _uiComponentMapInfo.CurrentMapData;

        bool hasHiddenMap = Random.Range(0, 100) < 100;

        if (hasHiddenMap)
            await _cgManager.CreateCGAsync(roleData.VideoResourcePath);

        if (roleData.RoleType == ERoleType.Bule || roleData.RoleType == ERoleType.Red)
            await UniTask.Delay(2000);
        else
            await _soundManager.PlayIPOpening(roleData.RoleType).ToUniTask();

        await SceneManager.LoadSceneAsync("GameplayForestScene").ToUniTask();
        DynamicGI.UpdateEnvironment();

        if (hasHiddenMap)
        {
#if !UNITY_EDITOR
            await _cgManager.PlayCGAsync();
#endif
        }

        _uiManager.CloseUI("UIPrepare");

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
    #endregion
}
