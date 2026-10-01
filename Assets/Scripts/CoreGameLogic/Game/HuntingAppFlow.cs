using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.Game;
using GameFramework.Manager;
using GameFramework.UI;

/// <summary>
/// 打猎应用流程
/// </summary>
public class HuntingAppFlow : GameAppFlow<HuntingAppFlow>
{
    private RoundFlow _currentRoundFlow;
    public RoundFlow RoundFlow => _currentRoundFlow;

    private UIManager _uiManager;
    private AudioManager _audioManager;
    private HuntingConfigManager _configManager;
    private TransitionGate _transitionGate;
    private PlayerDataManager _playerDataManager;

    public RoundFlow CreateRoundFlow()
    {
        _currentRoundFlow = new RoundFlow();
        return _currentRoundFlow;
    }

    public void ClearRoundFlow()
    {
        _currentRoundFlow = null;
    }

    public async UniTask EnterPrepareAsync()
    {
        await _transitionGate.Run(new ToPrepareJob());
        _audioManager.PlayMusic(_configManager.UiAudioRefSo.PrepareBgm, 0f);
    }

    public async UniTask EnterRound(RoundContext context)
    {
        await _transitionGate.Run(new ToRoundJob(context));
        if (_currentRoundFlow != null)
            await _currentRoundFlow.PresentAsync();
    }

    public async UniTask EnterHiddenMapAsync()
    {
        await _transitionGate.Run(new ToHiddenMapJob());
        if (_currentRoundFlow != null)
            await _currentRoundFlow.PresentAsync();
    }

    protected override void RegisterAppManagers()
    {
        RegisterAppManager(new HuntingConfigManager());
        RegisterAppManager(new SaveService());
        RegisterAppManager(new PlayerDataManager());
        RegisterAppManager(new InputManager());
        RegisterAppManager(new CameraManager());
        RegisterAppManager(new TransitionGate());
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _audioManager = GameServiceLocator.AudioManager;
        _configManager = GameServiceLocator.ConfigManager;
        _transitionGate = GetAppManager<TransitionGate>();
        _playerDataManager = GetAppManager<PlayerDataManager>();
    }

    protected override UniTask OnAppExitAsync()
    {
        if (_playerDataManager != null)
            _playerDataManager.Save();
        return UniTask.CompletedTask;
    }

    protected override async UniTask OnAppStartAsync()
    {
        BindServices();
        await _uiManager.PreloadAsync(UILifetime.Overlay);
        await EnterPrepareAsync();
    }

    protected override void OnAppRunning(float dt)
    {
        _currentRoundFlow?.DoUpdate(dt);
    }
}
