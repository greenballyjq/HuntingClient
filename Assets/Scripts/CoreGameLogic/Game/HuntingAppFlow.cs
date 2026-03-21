using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 打猎应用流程
/// </summary>
public class HuntingAppFlow : GameAppFlow
{
    private static HuntingAppFlow _instance;
    public static HuntingAppFlow Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    /// <summary>
    /// 打猎应用流程状态
    /// </summary>
    private enum EHuntingAppFlowState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,

        /// <summary>
        /// 准备状态
        /// </summary>
        Prepare,

        /// <summary>
        /// 单局状态
        /// </summary>
        Round
    }

    /// <summary>
    /// 当前状态
    /// </summary>
    private EHuntingAppFlowState _currentState = EHuntingAppFlowState.None;

    /// <summary>
    /// 当前单局流程
    /// </summary>
    private RoundFlow _currentRoundFlow;
    public RoundFlow RoundFlow => _currentRoundFlow;

    private UIManager _uiManager;
    
    private void Start()
    {
        StartAppAsync().Forget();
    }

    #region 公共方法
    /// <summary>
    /// 进入准备
    /// </summary>
    public async UniTask EnterPrepareAsync()
    {
        if(_currentRoundFlow != null)
        {
            await _currentRoundFlow.EndRound();
            _currentRoundFlow = null;
        }

        await _uiManager.OpenUIAsync<UIPrepare>("UIPrepare");

        _currentState = EHuntingAppFlowState.Prepare;
    }

    /// <summary>
    /// 进入单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public async UniTask EnterRound(RoundContext context)
    {
        // 播放过渡动画
        var uiLoading = await _uiManager.OpenUIAsync<UINormalLoading>("UINormalLoading", UIManager.UILayer.Loading);
        await uiLoading.PlayFadeInAsync();
        _uiManager.CloseUI("UIPrepare");

        // 加载场景
        await SceneManager.LoadSceneAsync("GameplayForestScene").ToUniTask();
        DynamicGI.UpdateEnvironment();
        
        // 进入单局
        _currentRoundFlow = new RoundFlow();
        await _currentRoundFlow.EnterRound(context);

        _currentState = EHuntingAppFlowState.Round;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 获取配置管理器
    /// </summary>
    protected override BaseConfigManager GetConfigManager()
    {
        return HuntingConfigManager.Instance;
    }

    /// <summary>
    /// 注册应用级管理器
    /// </summary>
    protected override void RegisterAppManagers()
    {
        RegisterAppManager(new PlayerDataManager());
        RegisterAppManager(new InputManager());
        RegisterAppManager(new CameraManager());
        RegisterAppManager(new CGManager());
        RegisterAppManager(new HuntingSoundManager());
        RegisterAppManager(new MallManager());
    }
    #endregion

    #region 私有方法
    public void RegisterServers()
    {
        _uiManager = GameServiceLocator.UIManager;
    }
    #endregion

    #region 钩子方法
    /// <summary>
    /// 应用启动钩子
    /// </summary>
    protected override async UniTask OnAppStartAsync()
    {
        RegisterServers();

        // TODO: 预加载所有会用到的UIBase预制体 将来可能在别处调用
        await _uiManager.PreloadUIAsync("UIPrepare");
        await _uiManager.PreloadUIAsync("UIPopupLucky");
        await _uiManager.PreloadUIAsync("UIPopupLuckyBuff");
        await _uiManager.PreloadUIAsync("UIPopupLucky");
        await _uiManager.PreloadUIAsync("UIRanking");

        await _uiManager.PreloadUIAsync("UIGameplay");
        await _uiManager.PreloadUIAsync("UICountdown");

        await _uiManager.PreloadUIAsync("UIPopupNormalSettlement");
        await _uiManager.PreloadUIAsync("UIPopupFakeSettlement");
        await _uiManager.PreloadUIAsync("UIPopupHiddenSettlement");
        await _uiManager.PreloadUIAsync("UISnowMountainVictory");

        _uiManager.SetCanvasScaler(CanvasScaler.ScaleMode.ScaleWithScreenSize, new Vector2(1920, 1080), 1);
    }

    /// <summary>
    /// 应用运行时钩子
    /// </summary>
    /// <param name="dt">时间增量</param>
    protected override void OnAppRunning(float dt)
    {
        _currentRoundFlow?.DoUpdate(dt);
    }
    #endregion
}
