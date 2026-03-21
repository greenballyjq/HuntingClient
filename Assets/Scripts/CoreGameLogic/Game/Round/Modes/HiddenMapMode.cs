using System;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Events;
using Hunting.Game.Animal;
using UnityEngine;
using CoreGameLogic.Managers.AppManagers;
using GameFramework.Core.Audio;

/// <summary>
/// 隐藏地图玩法模式
/// </summary>
public class HiddenMapMode : IGameplayMode
{
    /// <summary>
    /// 倒计时剩余时间（秒）
    /// </summary>
    private float _countdownRemaining ;
    public float CountdownRemaining => _countdownRemaining;

    /// <summary>
    /// 隐藏地图上下文
    /// </summary>
    private HiddenMapContext _hiddenMapContext;

    private RoundFlow _roundFlow => RoundFlow.Instance;
    private EventManager _eventManager;
    private UIManager _uiManager;
    private EffectManager _effectManager;
    private HuntingConfigManager _configManager;
    private HuntingSoundManager _soundManager;
    private PlayerControlManager _playerControlManager;
    private AnimalManager _animalManager;
    private SpawnerManager _spawnerManager;
    private SettlementManager _settlementManager;

    public HiddenMapMode(HiddenMapContext hiddenMapContext)
    {
        RegisterServices();
        RegisterEvents();

        _hiddenMapContext = hiddenMapContext;
    }

    public async UniTask EnterAsync()
    {
        _countdownRemaining = _configManager.GetHiddenMapCountdownTime();

        //设置玩法规则
        SetPlayRules();

        // 切换到隐藏地图游玩界面
        var uiGameplay = await _uiManager.OpenUIAsync<UIGameplay>("UIGameplay",UIManager.UILayer.Fixed);
        uiGameplay.SwitchToMode(EGameplayMode.HiddenMap);

        // 切换到摇杆操作
        _playerControlManager.SwitchToJoystick();

        // 禁止游玩界面操作
        uiGameplay.SetClickable(false);

        // 结束过渡动画
        var uiLoading = _uiManager.GetUI<UINormalLoading>("UINormalLoading");
        await UniTask.Delay(6000);
        await uiLoading.PlayFadeOutAsync();
        await UniTask.Delay(500);

        // 播放警报动画与音效
        var uiAlertRed = await _uiManager.OpenUIAsync<UIAlertRed>("UIAlertRed", UIManager.UILayer.Normal);
        await UniTask.WhenAll(
            uiAlertRed.PlayFlashAsync(),
            _soundManager.PlayBossAlert().ToUniTask()
        );

        // 派发并播放Boss入场动画
        var bossAnimalBehavior = _spawnerManager.GetSpawner<ManualSpawner>("Boss").Spawn() as BossAnimalBehaviour;
        await UniTask.Delay(3000); // 模拟Boss入场动画

        // 播放Boss血量增长动画
        uiGameplay.PlayBossHealthIncreaseAsync().Forget();

        // 播放Boss咆哮音效
        await _soundManager.PlayBossRoar(bossAnimalBehavior.SpecieData.BossType).ToUniTask();
        await UniTask.Delay(2000); // 等待固定时长控制节奏

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync(new[] { "5", "4", "3", "2", "1", "准备..", "开始.", "战斗!!" }, 8f);

        // Boss进入战斗状态
        bossAnimalBehavior.EnterCombat();

        // 恢复游玩界面操作
        uiGameplay.SetClickable(true);
    }

    public async UniTask ExitAsync(EGameplayMode? nextTarget)
    {
        UnregisterEvents();

        await UniTask.CompletedTask;
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
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _effectManager = GameServiceLocator.EffectManager;
        _configManager = GameServiceLocator.ConfigManager;
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
        _spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        _settlementManager = GameServiceLocator.GetRoundManager<SettlementManager>();
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
    /// 结束隐藏地图流程
    /// </summary>
    private async UniTask EndHiddenMapAsync()
    {
        _roundFlow.StartSettlement();

        _uiManager.GetUI<UIGameplay>("UIGameplay").ForceStopAllTips();

        // 播放慢镜头动画
        await PlaySlowMotion(0.3f, 4f);

        // 停止下雪特效
        _effectManager.Stop(_hiddenMapContext.SnowEffect);

        // 播放光效动画和音效
        var uiSnowMountainVictory = await _uiManager.OpenUIAsync<UISnowMountainVictory>("UISnowMountainVictory", UIManager.UILayer.PopUp);
        await uiSnowMountainVictory.PlayLightEffectAsync(_animalManager.GetLastActiveAnimalPosition());

        // 播放全家福动画
        await uiSnowMountainVictory.PlayFamilyPortraitFadeInAsync();
        
        // 打开结算面板
        await _uiManager.OpenUIAsync<UIPopupHiddenSettlement>("UIPopupHiddenSettlement", UIManager.UILayer.PopUp);
        _settlementManager.CalculateReward();
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
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private async void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (_animalManager.GetUnDeathAnimalCount() != 0)
            return;

        await EndHiddenMapAsync();
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