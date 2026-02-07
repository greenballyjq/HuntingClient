using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Events;
using Hunting.Game.Animal;

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
    
    public void Init()
    {
        _eventManager.AddListener(AnimalEvents.AnimalRemoved, OnAnimalRemoved);
        _eventManager.AddListener(BossEvents.BossDied, OnBossDied);
        _bossDied = false;
    }

    public void Release()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalRemoved, OnAnimalRemoved);
        _eventManager.RemoveListener(BossEvents.BossDied, OnBossDied);
    }

    private void OnAnimalRemoved(AnimalRemovedEventArgs _)
    {
        CheckIsHiddenRoundEnd();
    }

    private void OnBossDied(BossDiedEventArgs _)
    {
        _bossDied = true;
        CheckIsHiddenRoundEnd();
    }
    
    private void CheckIsHiddenRoundEnd()
    {
        Debug.Log($"[{GetType().Name}] 检测雪山地图是否结束, bossDied: {_bossDied}, count: {_animalManager.GetActiveAnimalCount()}");
        if (_bossDied && !_animalManager.HasActiveAnimal())
        {
            // 隐藏地图结束
            RoundFlow.Instance.EndHiddenMapAsync().Forget();
        }
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
    /// 单局流程当前状态
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
    /// 单局管理器列表
    /// </summary>
    private readonly List<IRoundManager> _roundManagers = new List<IRoundManager>();

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 对象池管理器
    /// </summary>
    private GameObjectPoolManager _gameObjectPoolManager => GameServiceLocator.GameObjectPoolManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.GetFrameworkManager<EffectManager>();

    #region 公共方法
    /// <summary>
    /// 获取单局管理器
    /// </summary>
    public T GetRoundManager<T>() where T : class, IRoundManager
    {
        foreach (var manager in _roundManagers)
        {
            if (manager is T result)
                return result;
        }
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
        _currentRoundContext.HiddenRoundEndTrigger = new HiddenRoundEndTrigger();

        // 创建单局管理器
        CreateRoundManagers();

        // 初始化单局管理器
        InitRoundManagers();

        // 打开游玩界面
        var uiGameplay = await _uiManager.OpenUIAsync<UIGameplay>("UIGameplay", UIManager.UILayer.Fixed);

        // 禁止游玩界面操作
        uiGameplay.SetClickable(false);

        // 触发单局进入事件
        TriggerRoundEntered(new RoundEnteredEventArgs
        {
            RoundContext = _currentRoundContext
        });

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync();

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

        // 清理单局管理器
        DisposeRoundManagers();
        _roundManagers.Clear();
        _currentRoundContext = null;

        // 清理对象池
        _gameObjectPoolManager.ClearAllPools();

        // 关闭游玩界面
        _uiManager.CloseUI("UIGameplay");

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

        // 伪结算面板动画
        var uiFakeSettlement = _uiManager.GetUI<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake");
        await UniTask.WhenAll(
            uiFakeSettlement.PlayWindowShakeAsync(),
            uiFakeSettlement.PlayButtonGlowAsync()
        );
        _uiManager.CloseUI("UIPopupSettlementSnowFake");

        // 伪结算面板爆米花动画  
        _effectManager.PlayOneShotAsync("Assets/Arts/Prefabs/Particles/Settlement_Explosion", Vector3.zero, Quaternion.identity,dontDestroyOnLoad: true).Forget();

        // 下雪动画
        _snowEffect = await _effectManager.PlayLoopAsync("Assets/Arts/Prefabs/Particles/FX_Snow", new Vector3(0,5,0), Quaternion.identity, dontDestroyOnLoad: true);

        #region 地图过渡
        // 打开加载界面
        var uiLoading = await _uiManager.OpenUIAsync<UILoading>("UILoading", UIManager.UILayer.Loading);

        // 播放淡入动画
        await uiLoading.PlayFadeInAsync();

        // 清理本局管理器
        CleanupManagers();

        // 加载场景
        await SceneManager.LoadSceneAsync("GameplaySnowMountainScene").ToUniTask();
        
        // 触发隐藏地图进入事件
        _eventManager.Trigger(HiddenMapEvents.HiddenMapEntered);

        // 重新初始化本局管理器
        ReInitManagers();

        // 更新环境光
        DynamicGI.UpdateEnvironment();

        // 切换雪山主题
        var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay"); 
        uiGameplay.SwitchSnow();

        // 禁止游玩界面操作
        uiGameplay.SetClickable(false);

        // 播放淡出动画
        await uiLoading.PlayFadeOutAsync(); 

        // 关闭加载界面
        _uiManager.CloseUI("UILoading");
        #endregion

        // 播放警报声
        //_soundManager.PlaySound2DAsync("Assets/Arts/Audio/SFX/sfx_alert").Forget();

        // 播放闪烁动画
        var uiAlertRed = await _uiManager.OpenUIAsync<UIAlertRed>("UIAlertRed", UIManager.UILayer.Normal);
        await uiAlertRed.PlayFlashAsync();

        // Boss登场动画
        GetRoundManager<SpawnerManager>().GetSpawner<ManualSpawner>("Boss").Spawn();

        // 播放Boss血量增长动画
        await uiGameplay.PlayBossHealthIncreaseAnimationAsync();

        // 播放Boss笑声
        //await _soundManager.PlaySound2DAsync("Assets/Arts/Audio/SFX/sfx_laugh");
        await UniTask.Delay(3000);

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync();

        // 恢复游玩界面操作
        uiGameplay.SetClickable(true);

        // Boss进入战斗状态
        GetRoundManager<AnimalManager>().GetBossAnimalBehaviour().EnterCombat();

        _currentRoundContext.HiddenRoundEndTrigger.Init();

        _currentState = RoundFlowState.Playing;
    }

    /// <summary>
    /// 结束隐藏地图
    /// </summary>
    public async UniTask EndHiddenMapAsync()
    {
        _currentState = RoundFlowState.Transitioning;

        #region 测试
        _currentRoundContext.HiddenRoundEndTrigger.Release();
        await _effectManager.PlayOneShotAsync("Assets/Arts/Prefabs/Particles/FX_DGB_PTFH", new Vector3(0, 0, 5), Quaternion.identity);
        _effectManager.Stop(_snowEffect);
        await UniTask.Delay(3000);
        #endregion

        // 打开结算界面
        await _uiManager.OpenUIAsync<UIPopupSettlementSnowVictory>("UIPopupSettlementSnowVictory", UIManager.UILayer.PopUp);
        GetRoundManager<SettlementRewardManager>().CalculateReward();

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
        _roundManagers.Add(new WeaponManager());
        _roundManagers.Add(new PlayerManager());
        _roundManagers.Add(new AnimalManager());
        _roundManagers.Add(new EnergyProgressManager());
        _roundManagers.Add(new MeatProgressManager());
        _roundManagers.Add(new QuestManager());
        _roundManagers.Add(new SettlementRewardManager());
        _roundManagers.Add(new SpawnerManager());
        _roundManagers.Add(new TrapManager());
        _roundManagers.Add(new LuckyBuffManager());
        _roundManagers.Add(new PlayerControlManager());
        _roundManagers.Add(new PropManager());
        _roundManagers.Add(new BulletManager());
        _roundManagers.Add(new SkillManager());    
    }

    /// <summary>
    /// 初始化单局管理器
    /// </summary>
    private void InitRoundManagers()
    {
        for (int i = 0; i < _roundManagers.Count; i++)
            _roundManagers[i].Init(_currentRoundContext);
    }

    /// <summary>
    /// 释放单局管理器
    /// </summary>
    private void DisposeRoundManagers()
    {
        for (int i = _roundManagers.Count - 1; i >= 0; i--)
            _roundManagers[i].Dispose();
    }

    /// <summary>
    /// 清理单局管理器
    /// </summary>
    private void CleanupManagers()
    {
        for (int i = 0; i < _roundManagers.Count; i++)
        {
            if (_roundManagers[i] is IRoundResettable resettable)
                resettable.Cleanup();
        }
    }

    /// <summary>
    /// 重新初始化单局管理器
    /// </summary>
    private void ReInitManagers()
    {
        for (int i = 0; i < _roundManagers.Count; i++)
        {
            if (_roundManagers[i] is IRoundResettable resettable)
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

        for (int i = 0; i < _roundManagers.Count; i++)
        {
            if (_roundManagers[i] is IRoundUpdatable updatable)
                updatable.DoUpdate(dt);
        }
    }
    #endregion

    #region 事件相关
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
    #endregion
}
