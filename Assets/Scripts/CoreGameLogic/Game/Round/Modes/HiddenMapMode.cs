using System;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Audio;
using GameFramework.Manager;
using GameFramework.UI;
using Hunting.Events;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 隐藏地图玩法模式
/// </summary>
public class HiddenMapMode : IGameplayMode
{
    private const int AlertDelayMs = 1000;
    private const int PresentBeatMs = 2000;
    private const float CombatCountdownDuration = 8f;
    private static readonly string[] CombatCountdownTexts =
        { "5", "4", "3", "2", "1", "准备..", "开始.", "战斗!!" };
    /// <summary>
    /// 倒计时剩余时间（秒）
    /// </summary>
    private float _countdownRemaining ;
    public float CountdownRemaining => _countdownRemaining;

    /// <summary>
    /// 隐藏地图上下文
    /// </summary>
    private HiddenMapContext _hiddenMapContext;

    private RoundFlow _roundFlow;
    private EventManager _eventManager;
    private UIManager _uiManager;
    private EffectManager _effectManager;
    private HuntingConfigManager _configManager;
    private PlayerControlManager _playerControlManager;
    private AnimalManager _animalManager;
    private SpawnerManager _spawnerManager;
    private AudioManager _audioManager;
    private bool _fieldCleared;
    private bool _slowMotionStarted;
    private bool _victoryStarted;
    private BossAnimalBehaviour _boss;

    public HiddenMapMode(RoundFlow roundFlow, HiddenMapContext hiddenMapContext)
    {
        BindServices();
        SubscribeEvents();

        _roundFlow = roundFlow;
        _hiddenMapContext = hiddenMapContext;
    }

    public async UniTask EnterAsync()
    {
        _countdownRemaining = _configManager.GetHiddenMapCountdownTime();

        SetPlayRules();

        var uiGameplay = await _uiManager.OpenAsync<UIGameplay>();
        uiGameplay.SwitchToMode(EGameplayMode.HiddenMap);

        _playerControlManager.SwitchToJoystick();
        uiGameplay.SetClickable(false);

        var hiddenMapRef = _configManager.MapRefSo.Get(_roundFlow.RoundContext.HiddenMapData.ID);
        _audioManager.Play(hiddenMapRef?.Ambience);
    }

    public async UniTask PresentAsync()
    {
        var uiGameplay = _uiManager.Get<UIGameplay>();
        var hiddenMapRef = _configManager.MapRefSo.Get(_roundFlow.RoundContext.HiddenMapData.ID);

        await UniTask.Delay(AlertDelayMs);

        var uiAlertRed = await _uiManager.OpenAsync<UIAlertRed>();
        await UniTask.WhenAll(
            uiAlertRed.PlayFlashAsync(),
            AudioWait.UntilEnd(_audioManager, _configManager.UiAudioRefSo.BossAlert)
        );

        var bossAnimalBehavior = _spawnerManager.GetSpawner<ManualSpawner>("Boss").Spawn() as BossAnimalBehaviour;
        _boss = bossAnimalBehavior;
        await UniTask.Delay(PresentBeatMs);

        await AudioWait.UntilEndAttached(
            _audioManager,
            _configManager._AnimalRefSo.Get(bossAnimalBehavior.SpecieData.ID)?.Roar,
            bossAnimalBehavior.transform);
        await UniTask.Delay(PresentBeatMs);

        await UniTask.WhenAll(
            uiGameplay.PlayBossHealthIncreaseAsync(),
            AudioWait.UntilEnd(_audioManager, _configManager.UiAudioRefSo.BossHealthGrowth)
        );
        await UniTask.Delay(PresentBeatMs);

        var uiCountDown = await _uiManager.OpenAsync<UICountdown>();
        await uiCountDown.PlayCountdownAsync(CombatCountdownTexts, CombatCountdownDuration);

        _boss.EnterCombat();
        uiGameplay.SetClickable(true);

        _audioManager.PlayMusic(hiddenMapRef?.Bgm, 0f);
    }

    public UniTask ExitAsync()
    {
        UnsubscribeEvents();
        return UniTask.CompletedTask;
    }

    public void DoUpdate(float dt)
    {
        _countdownRemaining -= dt;

        if (_countdownRemaining < 0)
            _countdownRemaining = 0;

        TriggerTimeUpdated(new TimeUpdatedEventArgs 
        { 
            Seconds = _countdownRemaining, 
            IsCountdown = true 
        });
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _effectManager = GameServiceLocator.EffectManager;
        _configManager = GameServiceLocator.ConfigManager;
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
        _spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        _audioManager = GameServiceLocator.AudioManager;
    }

    /// <summary>
    /// 设置玩法规则
    /// </summary>
    private void SetPlayRules()
    {
        _roundFlow.SetPlayRule<ITimingRule>(new CountdownTimingRule(this));
        _roundFlow.SetPlayRule<IUIGameplayComponentVisibilityRule>(new HiddenMapComponentVisibilityRule());
    }

    /// <summary>
    /// 全场逻辑死亡后的慢动作
    /// </summary>
    private async UniTask PlayAllDeadSlowMotionAsync()
    {
        _uiManager.Get<UIGameplay>().ForceStopAllTips();
        await PlaySlowMotion(0.3f, 4f);
        _effectManager.Stop(_hiddenMapContext.SnowEffect);
    }

    /// <summary>
    /// 最后一具尸体消失后的胜利演出
    /// </summary>
    private async UniTask PlayVictorySequenceAsync()
    {
        _roundFlow.StartSettlement();
        await _uiManager.OpenAsync<UIPopupHiddenSettlement>();
    }

    /// <summary>
    /// 播放慢镜头动画
    /// </summary>
    private async UniTask PlaySlowMotion(float timeScale = 0.25f, float duration = 3f)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = timeScale;

        await UniTask.Delay(Mathf.RoundToInt(duration * 1000), ignoreTimeScale: true);

        Time.timeScale = originalTimeScale;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void SubscribeEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.AddListener(BossEvents.BossDied, OnBossDied);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnsubscribeEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.RemoveListener(BossEvents.BossDied, OnBossDied);
    }

    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private async void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (args.SpecieData.BossType != EBossType.None && !_fieldCleared)
        {
            _fieldCleared = true;
            _animalManager.KillAllAliveExcept(args.Animal);
        }

        if (_animalManager.GetAliveAnimalCount() != 0)
            return;
        if (_slowMotionStarted)
            return;

        _slowMotionStarted = true;
        await PlayAllDeadSlowMotionAsync();
    }

    /// <summary>
    /// 动物尸体消失事件回调
    /// </summary>
    private async void OnAnimalDied(AnimalDiedEventArgs args)
    {
        await TryPlayVictoryAsync(args.Animal);
    }

    /// <summary>
    /// Boss尸体消失事件回调
    /// </summary>
    private async void OnBossDied(BossDiedEventArgs args)
    {
        await TryPlayVictoryAsync(args.Boss);
    }

    private async UniTask TryPlayVictoryAsync(BaseAnimalBehaviour excluding)
    {
        if (_animalManager.HasRemainingOnField(excluding))
            return;
        if (_victoryStarted)
            return;

        _victoryStarted = true;
        await PlayVictorySequenceAsync();
    }

    /// <summary>
    /// 触发时间更新事件
    /// </summary>
    private void TriggerTimeUpdated(TimeUpdatedEventArgs args)
    {
        _eventManager.Trigger(RoundEvents.TimeUpdated, args);
    }
    #endregion
}