using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Audio;
using GameFramework.Core;
using GameFramework.Manager;
using GameFramework.UI;
using Hunting.Game.Animal;

public class RoundContext
{
    public Role RoleData { get; set; }
    public Map MapData { get; set; }
    public Skill SkillData { get; set; }
    public LuckyBuff LuckyBuffData { get; set; }
    public bool HasHiddenMap { get; set; }
    public Map HiddenMapData { get; set; }
}

public class HiddenMapContext
{
    public GameObject SnowEffect { get; set; }
}

public enum EGameplayMode
{
    MainMap,
    HiddenMap,
}

public class RoundFlow
{
    private enum ERoundFlowState
    {
        None,
        Transitioning,
        Presenting,
        Playing,
        Settlement,
        Paused,
    }

    private ERoundFlowState _currentState = ERoundFlowState.None;
    private IGameplayMode _currentMode;
    private RoundContext _roundContext;
    public RoundContext RoundContext => _roundContext;

    private readonly Dictionary<Type, object> _playRules = new Dictionary<Type, object>();
    private readonly List<IRoundManager> _roundManagerList = new List<IRoundManager>();
    private readonly List<IRoundUpdatable> _updatables = new List<IRoundUpdatable>();
    private readonly List<IRoundPausable> _pausables = new List<IRoundPausable>();
    private readonly List<IMapWorld> _worlds = new List<IMapWorld>();
    private readonly Dictionary<Type, IRoundManager> _roundManagers = new Dictionary<Type, IRoundManager>();

    private UIManager _uiManager;
    private EventManager _eventManager;
    private AudioManager _audioManager;
    private EffectManager _effectManager;
    private ResourceManager _resourceManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private InputManager _inputManager;

    public T GetRoundManager<T>() where T : class, IRoundManager
    {
        return (T)_roundManagers[typeof(T)];
    }

    public void SetPlayRule<T>(T rule) where T : class
    {
        _playRules[typeof(T)] = rule;
    }

    public T GetPlayRule<T>() where T : class
    {
        if (_playRules.TryGetValue(typeof(T), out object rule))
            return rule as T;

        return null;
    }

    public void ClearPlayRules()
    {
        _playRules.Clear();
    }

    public async UniTask EnterRound(RoundContext context)
    {
        _currentState = ERoundFlowState.Transitioning;
        _roundContext = context;
        BindServices();
        _eventManager.PushScope(ResourceScope.Round);
        RegisterRoundManagers();
        await InitRoundManagersAsync();
        await _uiManager.OpenAsync<UIQuest>();
        _currentMode = new MainMapMode(this);
        await _currentMode.EnterAsync();
    }

    public async UniTask PresentAsync()
    {
        _currentState = ERoundFlowState.Presenting;
        if (_currentMode != null)
            await _currentMode.PresentAsync();
        _currentState = ERoundFlowState.Playing;
    }

    public async UniTask EndRound()
    {
        _currentState = ERoundFlowState.Transitioning;
        ClearPlayRules();
        if (_currentMode != null)
            await _currentMode.ExitAsync();
        _currentMode = null;

        UnbindWorlds();
        DisposeRoundManagers();
        _roundManagerList.Clear();
        _updatables.Clear();
        _pausables.Clear();
        _worlds.Clear();
        _roundManagers.Clear();
        _roundContext = null;

        UnloadRuntime();
        CloseRoundUi();
        PopRoundEvents();
        _currentState = ERoundFlowState.None;
    }

    public void DoUpdate(float dt)
    {
        switch (_currentState)
        {
            case ERoundFlowState.Playing:
                OnRoundPlaying(dt);
                break;
        }
    }

    public void StartSettlement()
    {
        _currentState = ERoundFlowState.Settlement;
        GetRoundManager<QuestManager>()?.Abort();
    }

    public void PauseRound()
    {
        _currentState = ERoundFlowState.Paused;
        for (int i = 0; i < _pausables.Count; i++)
            _pausables[i].Pause();

        _audioManager.PauseMusic();
        _effectManager.PauseAll();
        _inputManager.PauseGameplayInput();
    }

    public void ResumeRound()
    {
        _currentState = ERoundFlowState.Playing;
        for (int i = 0; i < _pausables.Count; i++)
            _pausables[i].Resume();

        _audioManager.ResumeMusic();
        _effectManager.ResumeAll();
        _inputManager.ResumeGameplayInput();
    }

    public async UniTask SwitchToHiddenMapAsync()
    {
        _currentState = ERoundFlowState.Transitioning;
        await _currentMode.ExitAsync();

        UnbindWorlds();
        UnloadRuntime();

        RoundLoadManifest manifest = RoundLoadManifest.ForHiddenMap(_roundContext);
        await _resourceManager.LoadSceneAsync(manifest.ScenePath);
        DynamicGI.UpdateEnvironment();
        manifest.Prewarm();
        BindWorlds(_roundContext.HiddenMapData);

        GameObject snow = await _effectManager.PlayLoopAsync(RoundScopeUnload.SnowEffect);
        _currentMode = new HiddenMapMode(this, new HiddenMapContext { SnowEffect = snow });
        await _currentMode.EnterAsync();
    }

    public async UniTask SwitchToNextMode(EGameplayMode nextMode)
    {
        if (nextMode == EGameplayMode.HiddenMap)
            await HuntingAppFlow.Instance.EnterHiddenMapAsync();
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _eventManager = GameServiceLocator.EventManager;
        _audioManager = GameServiceLocator.AudioManager;
        _effectManager = GameServiceLocator.EffectManager;
        _resourceManager = GameServiceLocator.ResourceManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _inputManager = GameServiceLocator.GetAppManager<InputManager>();
    }

    private void UnloadRuntime()
    {
        _effectManager.ClearAllEffects(true);
        _audioManager.StopMusic(0f);
        _audioManager.StopSfx();
        _gameObjectPoolManager.ClearAllPools();
    }

    private void CloseRoundUi()
    {
        _uiManager.Close(UILifetime.Round);
    }

    private void PopRoundEvents()
    {
        _eventManager.PopScope();
    }

    private void RegisterRoundManagers()
    {
        RegisterRoundManager(new RoundNumericLayer());
        RegisterRoundManager(new WeaponManager());
        RegisterRoundManager(new AnimalManager());
        RegisterRoundManager(new GameplaySceneItemManager());
        RegisterRoundManager(new EnergyProgressManager());
        RegisterRoundManager(new MeatProgressManager());
        RegisterRoundManager(new QuestManager());
        RegisterRoundManager(new SettlementManager());
        RegisterRoundManager(new SpawnerManager());
        RegisterRoundManager(new TrapManager());
        RegisterRoundManager(new LuckyBuffManager());
        RegisterRoundManager(new PlayerControlManager());
        RegisterRoundManager(new PropManager());
        RegisterRoundManager(new BulletManager());
        RegisterRoundManager(new SkillManager());
    }

    private void RegisterRoundManager(IRoundManager manager)
    {
        _roundManagers.Add(manager.GetType(), manager);
        _roundManagerList.Add(manager);
        if (manager is IRoundUpdatable updatable)
            _updatables.Add(updatable);
        if (manager is IRoundPausable pausable)
            _pausables.Add(pausable);
        if (manager is IMapWorld world)
            _worlds.Add(world);
    }

    private async UniTask InitRoundManagersAsync()
    {
        for (int i = 0; i < _roundManagerList.Count; i++)
            await _roundManagerList[i].InitAsync(_roundContext);
    }

    private void DisposeRoundManagers()
    {
        for (int i = _roundManagerList.Count - 1; i >= 0; i--)
            _roundManagerList[i].Dispose();
    }

    private void UnbindWorlds()
    {
        for (int i = 0; i < _worlds.Count; i++)
            _worlds[i].Unbind();
    }

    private void BindWorlds(Map mapData)
    {
        for (int i = 0; i < _worlds.Count; i++)
            _worlds[i].Bind(mapData);
    }

    private void OnRoundPlaying(float dt)
    {
        _currentMode?.DoUpdate(dt);
        for (int i = 0; i < _updatables.Count; i++)
            _updatables[i].DoUpdate(dt);
    }
}
