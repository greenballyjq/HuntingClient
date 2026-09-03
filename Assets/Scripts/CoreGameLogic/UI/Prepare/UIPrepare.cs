using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 准备界面
/// </summary>
[UIForm(UILayer.Page)]
public class UIPrepare : UIForm
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
    /// 准备页入场编排
    /// </summary>
    [SerializeField] private UIPrepareIntro _uiPrepareIntro;

    /// <summary>
    /// 开始单局按钮
    /// </summary>
    [SerializeField] private Button _buttonStartRound;

    /// <summary>
    /// 幸运仪式按钮
    /// </summary>
    [SerializeField] private Button _buttonLucky;

    /// <summary>
    /// 退出按钮
    /// </summary>
    [SerializeField] private Button _buttonQuit;

    private EventManager _eventManager;
    private UIManager _uiManager;
    private HuntingConfigManager _configManager;

    /// <summary>
    /// 当前选中的角色ID
    /// </summary>
    private int _currentRoleId;

    /// <summary>
    /// 当前选中幸运仪式增益数据
    /// </summary>
    private LuckyBuff _currentLuckyBuffData;
    private UIComponentStartRoundBlink _startRoundBlink;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _configManager = GameServiceLocator.ConfigManager;
        _startRoundBlink = _buttonStartRound.GetComponent<UIComponentStartRoundBlink>();

        _buttonStartRound.onClick.AddListener(OnClickStartRound);
        _buttonLucky.onClick.AddListener(OnClickLuckyRitual);
        _buttonQuit.onClick.AddListener(OnClickQuit);
    }

    private void OnDestroy()
    {
        _buttonStartRound.onClick.RemoveListener(OnClickStartRound);
        _buttonLucky.onClick.RemoveListener(OnClickLuckyRitual);
        _buttonQuit.onClick.RemoveListener(OnClickQuit);
    }

    protected override void OnOpen()
    {
        RegisterEvents();

        _uiComponentRollRole.Init();
        _uiComponentRoleInfo.Init();
        _uiComponentSkillInfo.Init();
        _uiComponentMapInfo.Init();
        _uiPrepareIntro?.Init();
    }

    protected override void OnClose()
    {
        _uiComponentRollRole.CleanUp();
        _uiComponentRoleInfo.CleanUp();
        _uiComponentSkillInfo.CleanUp();
        _uiComponentMapInfo.CleanUp();
        _uiPrepareIntro?.CleanUp();
        _startRoundBlink?.StopBlink();

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
        _eventManager.AddListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
        _eventManager.AddListener(LuckyEvents.GiftOpened, OnGiftOpened);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    public void UnregisterEvents()
    {
        _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.RemoveListener(PrepareEvents.DiceAnimationStarted, OnDiceAnimationStarted);
        _eventManager.RemoveListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
        _eventManager.RemoveListener(LuckyEvents.GiftOpened, OnGiftOpened);
    }

    /// <summary>
    /// 开始单局按钮回调
    /// </summary>
    private async void OnClickStartRound()
    {
        await _uiComponentRollRole.PlayRollDiceAsync();
        await EnterRoundAsync(_currentRoleId);
    }

    public void StartRoundWithRole(int roleId)
    {
        EnterRoundAsync(roleId).Forget();
    }

    private async UniTask EnterRoundAsync(int roleId)
    {
        Role roleData = _configManager.GetRole(roleId);
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);
        Map mapData = _uiComponentMapInfo.CurrentMapData;

        bool hasHiddenMap = Random.Range(0, 100) <= _configManager.GetHiddenMapProbability();

        RoleRefSo.RoleRef roleRef = _configManager.RoleRefSo.Get(roleData.ID);
        AudioCue selected = roleRef?.Selected;
        if (selected != null)
            await AudioWait.UntilEnd(selected);
        else if (roleData.RoleType == ERoleType.Bule || roleData.RoleType == ERoleType.Red)
            await UniTask.Delay(2000);

        HuntingAppFlow.Instance.EnterRound(new RoundContext
        {
            RoleData = roleData,
            MapData = mapData,
            SkillData = skillData,
            LuckyBuffData = _currentLuckyBuffData,
            HasHiddenMap = hasHiddenMap,
            HiddenMapData = hasHiddenMap ? _configManager.GetMap(EMapType.Hidden) : null
        }).Forget();
    }

    /// <summary>
    /// 幸运仪式按钮回调
    /// </summary>
    private async void OnClickLuckyRitual()
    {
        await _uiManager.OpenAsync<UIPopupLucky>();
    }

    /// <summary>
    /// 退出按钮回调
    /// </summary>
    private void OnClickQuit()
    {
        HuntingAppFlow.Instance.ExitAppAsync().Forget();
    }

    /// <summary>
    /// 角色选中事件回调
    /// </summary>
    private void OnRoleSelected(RoleSelectedEventArgs args)
    {
        _currentRoleId = args.RoleId;
    }

    /// <summary>
    /// 金币人停稳后再闪开始按钮，并一直闪到关准备页
    /// </summary>
    private void OnSlotAnimationEnded()
    {
        _startRoundBlink?.StartBlink();
    }

    /// <summary>
    /// 骰子动画开始事件
    /// </summary>
    private void OnDiceAnimationStarted()
    {
        _buttonStartRound.interactable = false;
        _buttonLucky.interactable = false;
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
