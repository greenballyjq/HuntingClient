using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.Core;
using GameFramework.Manager;
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
            RoundFlow.Instance.EndHiddenMapProcessAsync().Forget();
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
        /// 进入单局加载 进入雪山加载
        /// </summary>
        Transitioning,

        /// <summary>
        /// 游玩状态
        /// </summary>
        Playing,
    }

    /// <summary>
    /// 单局流程当前状态
    /// </summary>
    private RoundFlowState _currentState = RoundFlowState.None;

    /// <summary>
    /// 当前单局上下文
    /// </summary>
    private RoundContext _currentRoundContext;

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

    #region 测试
    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.GetFrameworkManager<EffectManager>();

    /// <summary>
    /// 音效管理器
    /// </summary>
    private SoundManager _soundManager => GameServiceLocator.GetFrameworkManager<SoundManager>();
    #endregion

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
    public async UniTask StartRound(RoundContext context)
    {
        #region 测试代码 将来会正式化
        _currentState = RoundFlowState.Transitioning;

        _currentRoundContext = context;

        CreateRoundManagers();

        InitRoundManagers();

        await _uiManager.OpenUIAsync<UIGameplay>("UIGameplay", UIManager.UILayer.Fixed);

        TriggerRoundStarted(new RoundStartedEventArgs
        {
            RoundContext = _currentRoundContext
        });

        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown",UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync();

        _currentState = RoundFlowState.Playing;
        
        _currentRoundContext.HiddenRoundEndTrigger = new HiddenRoundEndTrigger();

        #endregion
    }

    /// <summary>
    /// 结束单局
    /// </summary>
    public async UniTask EndRound()
    {
        #region 测试代码 将来会正式化
        _currentState = RoundFlowState.Transitioning;
        DisposeRoundManagers();
        _roundManagers.Clear();
        _currentRoundContext = null;
        _currentState = RoundFlowState.None;
        #endregion
        
        _uiManager.CloseUI("UISnowMountainSettlement");
        _uiManager.CloseUI("UIHuntingGameplay");

        await UniTask.CompletedTask;
    }

    GameObject fxSnow;
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
        _effectManager.PlayOneShotAsync("Arts/Prefabs/Particles/Settlement_Explosion", Vector3.zero, Quaternion.identity,persistAcrossScenes: true).Forget();

        // 下雪动画（循环特效，过场景不移除）
        fxSnow = await _effectManager.PlayLoopAsync("Arts/Prefabs/Particles/FX_Snow", new Vector3(0,5,0), Quaternion.identity, persistAcrossScenes: true);

        // 地图过渡动画
        var uiLoading = await _uiManager.OpenUIAsync<UILoading>("UILoading", UIManager.UILayer.Loading);
        await uiLoading.PlayFadeInAsync(); // 播放淡入动画
        CleanupManagers(); // 清理本局管理器
        await SceneManager.LoadSceneAsync("GameplaySnowMountainScene").ToUniTask(); // 加载场景
        ReInitManagers(); // 重新初始化管理器
        DynamicGI.UpdateEnvironment(); // 更新环境光
        await uiLoading.PlayFadeOutAsync(); // 播放淡出动画
        _uiManager.CloseUI("UILoading");

        var uiAlertRed = await _uiManager.OpenUIAsync<UIAlertRed>("UIAlertRed", UIManager.UILayer.Normal);
        //var boss = await GetRoundManager<AnimalManager>().GenerateBossAsync(); // Boss登场动画
        uiAlertRed.PlayFlashAsync().Forget(); // 播放红屏闪动动画
        _soundManager.PlaySound2DByPathAsync("Audio/SFX/sfx_alert").Forget(); // 播放警报声
        await UniTask.Delay(2000);
        //_soundManager.PlaySound2DByPathAsync("Audio/BGM/bgm_snow", AudioChannel.Bgm, volume: 0.1f).Forget(); // 播放远古雪山BGM
        _soundManager.PlaySound2DByPathAsync("Audio/SFX/sfx_laugh").Forget(); // 播放Boss台词
        await UniTask.Delay(3000);

        // 准备倒计时
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync();

        //boss.EnterCombat();
        _currentState = RoundFlowState.Playing;
        
        // _eventManager.Trigger(HiddenMapEvents.HiddenMapPlayStart);
        var spawnManager = GameServiceLocator.GetRoundManager<SpawnerManager>();
        var bossSpawner = spawnManager.GetSpawner<ManualSpawner>("Boss");
        bossSpawner.Spawn();

        _currentRoundContext.HiddenRoundEndTrigger.Init();
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
            
            case RoundFlowState.None:
            default:
                break;
        }
    }

    /// <summary>
    /// 结束隐藏地图
    /// </summary>
    public async UniTask EndHiddenMapProcessAsync()
    {
        _currentRoundContext.HiddenRoundEndTrigger.Release();

        // TODO: 特效动画
        await _effectManager.PlayOneShotAsync("Arts/Prefabs/Particles/FX_DGB_PTFH", new Vector3(0, 0, 5), Quaternion.identity);
        _effectManager.Stop(fxSnow,EffectStopMode.Graceful);
        await UniTask.Delay(3000);
        
        // TODO: 打开结算面板三
        var uiSnowMountainSettlement = await _uiManager.OpenUIAsync<UIPopupSettlementSnowVictory>("UIPopupSettlementSnowVictory", UIManager.UILayer.PopUp);
        
    }

    #endregion

    #region 私有方法
    /// <summary>
    /// 创建单局管理器
    /// </summary>
    private void CreateRoundManagers()
    {
        // TODO: 未来会根据情况调整顺序
        _roundManagers.Add(new WeaponManager());
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
    /// 初始化本局管理器
    /// </summary>
    private void InitRoundManagers()
    {
        for (int i = 0; i < _roundManagers.Count; i++)
            _roundManagers[i].Init(_currentRoundContext);
    }

    /// <summary>
    /// 释放本局管理器
    /// </summary>
    private void DisposeRoundManagers()
    {
        for (int i = _roundManagers.Count - 1; i >= 0; i--)
            _roundManagers[i].Dispose();
    }

    /// <summary>
    /// 清理本局管理器
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
    /// 重新初始化本局管理器
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
        for (int i = 0; i < _roundManagers.Count; i++)
        {
            if (_roundManagers[i] is IRoundUpdatable updatable)
                updatable.DoUpdate(dt);
            // _currentRoundContext.
        }
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
