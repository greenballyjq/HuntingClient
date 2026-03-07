using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Events;
using Hunting.Game.Animal;
using CoreGameLogic.Managers.AppManagers;
using GameFramework.Core.Audio;

/// <summary>
/// 单局上下文
/// </summary>
public class RoundContext
{
    /// <summary>
    /// 角色数据
    /// </summary>
    public Role RoleData { get; set; }

    /// <summary>
    /// 地图数据
    /// </summary>
    public Map MapData { get; set; }

    /// <summary>
    /// 技能数据
    /// </summary>
    public Skill SkillData { get; set; }

    /// <summary>
    /// 幸运仪式增益数据
    /// </summary>
    public LuckyBuff LuckyBuffData { get; set; }

    /// <summary>
    /// 是否有地图联动
    /// </summary>
    public bool HasLinkage { get; set; }

    /// <summary>
    /// 是否有隐藏地图
    /// </summary>
    public bool HasHiddenMap { get; set; }

    /// <summary>
    /// 隐藏地图数据
    /// </summary>
    public Map HiddenMapData { get; set; }

    /// <summary>
    /// 隐藏地图结束触发器
    /// </summary>
    public HiddenRoundEndTrigger HiddenRoundEndTrigger { get; set; }
}

public class HiddenRoundEndTrigger
{
    private EventManager _eventManager => GameServiceLocator.EventManager;
    private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    private bool _bossDied;

    private bool _isLastOne;

    public void Init()
    {
        _eventManager.AddListener(AnimalEvents.AnimalRemoved, OnAnimalRemoved);
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnterDeath);
        // _eventManager.AddListener(BossEvents.BossDied, OnBossDied);
        _bossDied = false;
    }

    public void Release()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalRemoved, OnAnimalRemoved);
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnterDeath);
        // _eventManager.RemoveListener(BossEvents.BossDied, OnBossDied);
    }

    private void OnAnimalRemoved(AnimalRemovedEventArgs _)
    {
        // if (_isLastOne) return;
        // var activeAnimalCount = _animalManager.GetActiveAnimalCount();
        // _isLastOne = activeAnimalCount == 1;
        // Debug.Log($"[{GetType().Name}] 最后一只? {_isLastOne}, activeAnimalCount: {activeAnimalCount}");
    }

    private void OnAnimalEnterDeath(AnimalEnteredDeathEventArgs args)
    {
        CheckIsHiddenRoundEnd();
    }


    private void CheckIsHiddenRoundEnd()
    {
        var isLastOne = _animalManager.GetUnDeathAnimalCount() == 0;

        Debug.Log($"[{GetType().Name}] 检测雪山地图是否结束, _isLastOne: {isLastOne}，{_animalManager.GetUnDeathAnimalCount()}");

        if (isLastOne)
            RoundFlow.Instance.EndHiddenMapAsync().Forget();
    }
}

/// <summary>
/// 单局流程类
/// </summary>
public class RoundFlow : Singleton<RoundFlow>
{
    /// <summary>
    /// 单局流程状态枚举
    /// </summary>
    private enum RoundFlowState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,

        /// <summary>
        /// 过渡状态
        /// </summary>
        Transitioning,

        /// <summary>
        /// 游玩状态
        /// </summary>
        Playing,

        /// <summary>
        /// 结算状态
        /// </summary>
        Settlement,

        /// <summary>
        /// 暂停状态
        /// </summary>
        Paused,
    }

    /// <summary>
    /// 当前状态
    /// </summary>
    private RoundFlowState _currentState = RoundFlowState.None;

    /// <summary>
    /// 当前单局上下文
    /// </summary>
    private RoundContext _currentRoundContext;
    public RoundContext CurrentRoundContext => _currentRoundContext;

    /// <summary>
    /// 单局已用时间（秒）
    /// </summary>
    private float _roundElapsedTime;
    public float RoundElapsedTime => _roundElapsedTime;

    /// <summary>
    /// 单局管理器字典
    /// </summary>
    private readonly Dictionary<Type, IRoundManager> _roundManagers = new Dictionary<Type, IRoundManager>();

    private EventManager _eventManager;
    private UIManager _uiManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private EffectManager _effectManager;
    private HuntingSoundManager _soundManager;

    public RoundFlow()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _effectManager = GameServiceLocator.EffectManager;
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
    }

    #region 公共方法
    /// <summary>
    /// 获取单局管理器
    /// </summary>
    public T GetRoundManager<T>() where T : class, IRoundManager
    {
        if (_roundManagers.TryGetValue(typeof(T), out var manager))
            return manager as T;

        return null;
    }

    /// <summary>
    /// 开始单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public async UniTask EnterRound(RoundContext context)
    {
        _currentState = RoundFlowState.Transitioning;

        _roundElapsedTime = 0f;
        _currentRoundContext = context;

        // 订阅事件
        RegisterEvents();

        // 创建单局管理器
        CreateRoundManagers();

        // 初始化单局管理器
        InitRoundManagers();

        // 打开游玩界面
        var uiGameplay = await _uiManager.OpenUIAsync<UIGameplay>("UIGameplay", UIManager.UILayer.Fixed);

        // 禁止游玩界面操作
        uiGameplay.SetClickable(false);

        // 切换到摇杆控制模式
        //GetRoundManager<PlayerControlManager>().SwitchToDefaultShooting();
        GetRoundManager<PlayerControlManager>().SwitchToJoystick();
        
        // 触发单局进入事件
        TriggerRoundEntered(new RoundEnteredEventArgs
        {
            RoundContext = _currentRoundContext
        });

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync(new[] { "5", "4", "3", "2", "1", "准备..", "开始.", "战斗!!" }, 8f);

        // 恢复游玩界面操作
        uiGameplay.SetClickable(true);

        // 触发单局开始事件
        TriggerRoundStarted(new RoundStartedEventArgs
        {
            RoundContext = _currentRoundContext
        });

        _currentState = RoundFlowState.Playing;        
    }

    /// <summary>
    /// 结束单局
    /// </summary>
    public async UniTask EndRound()
    {
        _currentState = RoundFlowState.Transitioning;

        // 释放单局管理器
        DisposeRoundManagers();
        _roundManagers.Clear();
        _currentRoundContext = null;

        // 清理对象池
        _effectManager.ClearAllEffects(true);
        _soundManager.ClearAllSounds(true);
        _gameObjectPoolManager.ClearAllPools();

        // 关闭游玩界面
        _uiManager.CloseUI("UIGameplay");

        // 取消订阅事件
        UnregisterEvents();

        // 加载准备场景
        await SceneManager.LoadSceneAsync("PrepareScene").ToUniTask();

        _currentState = RoundFlowState.None;
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void DoUpdate(float dt)
    {
        switch (_currentState)
        {
            case RoundFlowState.Playing:
                OnRoundPlaying(dt);
                break;
        }
    }

    /// <summary>
    /// 开始结算
    /// </summary>
    /// <returns></returns>
    public void StartSettlement()
    {
        _currentState = RoundFlowState.Settlement;
    }

    /// <summary>
    /// 暂停单局
    /// </summary>
    public void PauseRound()
    {
        _currentState = RoundFlowState.Paused;
    }

    /// <summary>
    /// 恢复单局
    /// </summary>
    public void ResumeRound()
    {
        _currentState = RoundFlowState.Playing;
    }

    private GameObject _snowEffect;
    /// <summary>
    /// 进入隐藏地图
    /// </summary>
    public async UniTask EnterHiddenMapAsync()
    {
        _currentState = RoundFlowState.Transitioning;

        _currentRoundContext.HiddenRoundEndTrigger = new HiddenRoundEndTrigger();
        _currentRoundContext.HiddenRoundEndTrigger.Init();

        // 播放伪结算面板动画
        var uiFakeSettlement = _uiManager.GetUI<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake");
        await UniTask.WhenAll(
            uiFakeSettlement.PlayWindowShakeAsync(),
            uiFakeSettlement.PlayButtonGlowAsync()
        );

        // 播放伪结算面板爆炸特效与音效
        await _effectManager.PlayOneShotAsync("Assets/Arts/Prefabs/Effects/Settlement_Explosion");
        _soundManager.PlaySettlementPanelExplosion();

        await UniTask.Delay(200); // 等待200毫秒模拟爆炸动画
        _uiManager.CloseUI("UIPopupSettlementSnowFake");
        

        #region 地图过渡
        // 播放淡入动画与音效
        _soundManager.PlayLightTransition();
        var uiLoading = await _uiManager.OpenUIAsync<UILoading>("UILoading", UIManager.UILayer.Loading);
        await uiLoading.PlayFadeInAsync();

        // 清理旧场景
        CleanupManagers(); // 清理单局管理器
        _effectManager.ClearAllEffects(); // 清理对象池
        _soundManager.ClearAllSounds();
        _gameObjectPoolManager.ClearAllPools(); 

        // 加载新场景
        await SceneManager.LoadSceneAsync("GameplaySnowMountainScene").ToUniTask();

        // 初始化新场景
        ReInitManagers(); // 重新初始化单局管理器
        DynamicGI.UpdateEnvironment(); // 更新环境光

        // 禁止游玩界面操作
        var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        uiGameplay.SetClickable(false);

        // UI切换雪山主题
        uiGameplay.SwitchSnow();

        // 播放中间过渡动画
        await uiLoading.PlayMiddleTransitionAsync();

        // 播放下雪特效
        _snowEffect = await _effectManager.PlayLoopAsync("Assets/Arts/Prefabs/Effects/FX_Snow_For_SnowMountainScene_UICamera");

        // 播放雪山环境音效
        _soundManager.PlaySnowMountainMapEnv();

        // 播放淡出动画
        await uiLoading.PlayFadeOutAsync();

        // 关闭加载界面
        _uiManager.CloseUI("UILoading");
        #endregion
        // 触发进入隐藏地图事件
        TriggerHiddenMapEntered();

        // 播放警报动画与音效
        var uiAlertRed = await _uiManager.OpenUIAsync<UIAlertRed>("UIAlertRed", UIManager.UILayer.Normal);
        await UniTask.WhenAll(
            uiAlertRed.PlayFlashAsync(),
            _soundManager.PlayBossAlert().ToUniTask()
        );

        // 派发并播放Boss入场动画
        var bossAnimalBehavior = GetRoundManager<SpawnerManager>().GetSpawner<ManualSpawner>("Boss").Spawn() as BossAnimalBehaviour;
        await UniTask.Delay(2000); // 模拟Boss入场动画

        // 播放Boss咆哮音效
        await _soundManager.PlayBossRoar(bossAnimalBehavior.SpecieData.BossType).ToUniTask();
        await UniTask.Delay(2000); // 等待固定时长控制节奏

        // 播放Boss血量增长动画和音效
        await UniTask.WhenAll(
            uiGameplay.PlayBossHealthIncreaseAnimationAsync(),
            _soundManager.PlayBossHPGrowth().ToUniTask()
        );
        await UniTask.Delay(2000); // 等待固定时长控制节奏

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync(new[] { "5", "4", "3", "2", "1", "准备..","开始.","战斗!!" }, 8f);

        // Boss进入战斗状态
        bossAnimalBehavior.EnterCombat();

        // 恢复游玩界面操作
        uiGameplay.SetClickable(true);

        _currentState = RoundFlowState.Playing;
    }

    /// <summary>
    /// 结束隐藏地图
    /// </summary>
    public async UniTask EndHiddenMapAsync()
    {
        _currentState = RoundFlowState.Transitioning;

        // 播放慢镜头动画
        await PlaySlowMotion(0.3f,4f);

        // 停止播放下雪特效
        _effectManager.Stop(_snowEffect);

        // 播放雪山胜利动画
        var uiMountainVictory = await _uiManager.OpenUIAsync<UISnowMountainVictory>("UISnowMountainVictory", UIManager.UILayer.PopUp);
        _soundManager.PlayLightTransition();
        await uiMountainVictory.PlayLightEffectAsync(GetRoundManager<AnimalManager>().GetLastActiveAnimalPosition()); // 播放光效动画
        await uiMountainVictory.PlayFamilyPortraitFadeInAsync(); // 播放全家福动画

        // 打开结算界面
        await _uiManager.OpenUIAsync<UIPopupSettlementSnowVictory>("UIPopupSettlementSnowVictory", UIManager.UILayer.PopUp);

        // 结算奖励
        GetRoundManager<SettlementRewardManager>().CalculateReward();

        _currentRoundContext.HiddenRoundEndTrigger.Release();

        _currentState = RoundFlowState.None;
    }
#endregion

    #region 私有方法
    /// <summary>
    /// 创建单局管理器
    /// </summary>
    private void CreateRoundManagers()
    {
        // TODO: 根据实际效果调整顺序
        RegisterRoundManager(new WeaponManager());
        RegisterRoundManager(new AnimalManager());
        RegisterRoundManager(new GameplaySceneItemManager());
        RegisterRoundManager(new EnergyProgressManager());
        RegisterRoundManager(new MeatProgressManager());
        RegisterRoundManager(new QuestManager());
        RegisterRoundManager(new SettlementRewardManager());
        RegisterRoundManager(new SpawnerManager());
        RegisterRoundManager(new TrapManager());
        RegisterRoundManager(new LuckyBuffManager());
        RegisterRoundManager(new PlayerControlManager());
        RegisterRoundManager(new PropManager());
        RegisterRoundManager(new BulletManager());
        RegisterRoundManager(new SkillManager());
    }

    /// <summary>
    /// 初始化单局管理器
    /// </summary>
    private void InitRoundManagers()
    {
        foreach (var manager in _roundManagers.Values)
            manager.Init(_currentRoundContext);
    }

    /// <summary>
    /// 释放单局管理器
    /// </summary>
    private void DisposeRoundManagers()
    {
        foreach (var manager in _roundManagers.Values)
            manager.Dispose();
    }

    /// <summary>
    /// 清理单局管理器
    /// </summary>
    private void CleanupManagers()
    {
        foreach (var manager in _roundManagers.Values)
        {
            if (manager is IRoundResettable resettable)
                resettable.Cleanup();
        }
    }

    /// <summary>
    /// 重新初始化单局管理器
    /// </summary>
    private void ReInitManagers()
    {
        foreach (var manager in _roundManagers.Values)
        {
            if (manager is IRoundResettable resettable)
                resettable.ReInit(_currentRoundContext);
        }
    }

    /// <summary>
    /// 游玩中更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    private void OnRoundPlaying(float dt)
    {
        _roundElapsedTime += dt;

        foreach (var manager in _roundManagers.Values)
        {
            if (manager is IRoundUpdatable updatable)
                updatable.DoUpdate(dt);
        }
    }

    /// <summary>
    /// 注册单局管理器
    /// </summary>
    /// <param name="manager">管理器实例</param>
    private void RegisterRoundManager(IRoundManager manager)
    {
        _roundManagers[manager.GetType()] = manager;
    }

    /// <summary>
    /// 播放慢镜头效果
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
        _eventManager.AddListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
    }

    /// <summary>
    /// 肉条刻度完成事件回调
    /// </summary>
    private async void OnMeatScaleCompleted(MeatScaleCompletedEventArgs args)
    {
        var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        int percent = args.CompletedScaleCount * 100 / args.TotalScaleCount;

        if (args.CompletedScaleCount < args.TotalScaleCount)
        {
            uiGameplay.PlayTipAnimationAsync($"狩猎进度已完成{percent}%", Color.yellow, 6f).Forget();
        }
        else
        {
            var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
            uiGameplay.PlayTipAnimationAsync("肉条已满，即将结算！", Color.red, 10f).Forget();
            await uiCountDown.PlayCountdownAsync(new[] { "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" }, 10f);

            StartSettlement();
            if (_currentRoundContext != null && _currentRoundContext.HasHiddenMap)
                await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
            else
                await _uiManager.OpenUIAsync<UIPopupSettlementNormal>("UIPopupSettlementNormal", UIManager.UILayer.PopUp);
            GetRoundManager<SettlementRewardManager>().CalculateReward();
        }
    }

    /// <summary>
    /// 触发进入单局事件
    /// </summary>
    private void TriggerRoundEntered(RoundEnteredEventArgs args)
    {
        _eventManager.Trigger(RoundEvents.RoundEntered, args);
    }

    /// <summary>
    /// 触发单局开始事件
    /// </summary>
    private void TriggerRoundStarted(RoundStartedEventArgs args)
    {
        _eventManager.Trigger(RoundEvents.RoundStarted, args);
    }

    /// <summary>
    /// 触发进入隐藏地图事件
    /// </summary>
    private void TriggerHiddenMapEntered()
    {
        _eventManager.Trigger(HiddenMapEvents.HiddenMapEntered);
    }
    #endregion


}
