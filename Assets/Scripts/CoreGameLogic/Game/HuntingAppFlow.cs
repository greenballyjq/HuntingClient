using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;

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

    #region 测试
    private UIManager _uiManager => GameServiceLocator.UIManager;

    private void Start()
    {
        // TODO: 可能由外部调用，暂时在这里启动应用，之后会按需调整
        StartAppAsync().Forget();
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 进入准备
    /// </summary>
    public async UniTask EnterPrepareAsync()
    {
        #region 测试 将来这些逻辑可能在别处
        _currentRoundFlow?.EndRound();
        _currentRoundFlow = null;
        await _uiManager.OpenUIAsync<UIPrepare>("UIPrepare");
        #endregion

        _currentState = HuntingAppFlowState.Prepare;
    }

    /// <summary>
    /// 进入单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public async UniTask EnterRound(RoundContext context)
    {
        #region 测试 将来这些逻辑可能在别处
        // 创建单局流程并开始
        _currentRoundFlow = new RoundFlow();
        await _currentRoundFlow.StartRound(context);
        #endregion

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
        #region 测试 将来不一定就是在这里进入准备
        await EnterPrepareAsync();
        #endregion
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
