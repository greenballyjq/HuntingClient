using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;
using UnityEngine;

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
    /// 打猎应用流程状态枚举
    /// </summary>
    private enum HuntingAppFlowState
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
    /// 打猎应用流程当前状态
    /// </summary>
    private HuntingAppFlowState _currentState = HuntingAppFlowState.None;

    /// <summary>
    /// 当前单局流程
    /// </summary>
    private RoundFlow _currentRoundFlow;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    private void Start()
    {
        // TODO: 将来可能在别处调用
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

        var uiPrepare = await _uiManager.OpenUIAsync<UIPrepare>("UIPrepare", isActive : false);
        await uiPrepare.PreloadResourcesAsync();
        uiPrepare.gameObject.SetActive(true);

        _currentState = HuntingAppFlowState.Prepare;
    }

    /// <summary>
    /// 进入单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public async UniTask EnterRound(RoundContext context)
    {
        _currentRoundFlow = new RoundFlow();
        await _currentRoundFlow.StartRound(context);
        
        _currentState = HuntingAppFlowState.Round;
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
        // TODO: 将来按需调整顺序
        RegisterAppManager(new PlayerDataManager());
        RegisterAppManager(new InputManager());
        RegisterAppManager(new CameraManager());
        
    }
    #endregion

    #region 钩子方法
    /// <summary>
    /// 应用启动钩子
    /// </summary>
    protected override async UniTask OnAppStartAsync()
    {
        // TODO: 预加载所有会用到的UIBase预制体 将来可能在别处调用
        await _uiManager.PreloadUIAsync("UIPrepare");
        await _uiManager.PreloadUIAsync("UIPopupLucky");
        await _uiManager.PreloadUIAsync("UIPopupLuckyBuff");
        await _uiManager.PreloadUIAsync("UIPopupLucky");
        await _uiManager.PreloadUIAsync("UIRanking");

        await _uiManager.PreloadUIAsync("UIGameplay");
        await _uiManager.PreloadUIAsync("UICountdown");

        await _uiManager.PreloadUIAsync("UIPopupSettlementNormal");

        Test();

        // TODO: 将来可能在别处调用
        await EnterPrepareAsync();

        
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

    #region 测试
    [SerializeField] private HuntingAudioRefSo HuntingAudioRefSo;
    [SerializeField] private AudioSource AudioSource;
    private void Test()
    {
        AudioSource.clip = HuntingAudioRefSo.GetAudioFromType(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_Beatch);
        AudioSource.Play();
    }
    #endregion
}
