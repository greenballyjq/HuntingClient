using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Game.Animal;
using CoreGameLogic.Managers.AppManagers;
using GameFramework.Core.Audio;
using cfg.HuntingConfig.Enum;

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
}

/// <summary>
/// 隐藏地图上下文
/// </summary>
public class HiddenMapContext
{
    public GameObject SnowEffect { get; set; }
    public AudioCallback EnvSoundCallback { get; set; }
}

/// <summary>
/// 玩法模式
/// </summary>
public enum EGameplayMode
{
    /// <summary>
    /// 主地图
    /// </summary>
    MainMap,

    /// <summary>
    /// 隐藏地图
    /// </summary>
    HiddenMap,
}

/// <summary>
/// 单局流程类
/// </summary>
public class RoundFlow : Singleton<RoundFlow>
{
    /// <summary>
    /// 单局流程状态
    /// </summary>
    private enum ERoundFlowState
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
    private ERoundFlowState _currentState = ERoundFlowState.None;

    /// <summary>
    /// 当前玩法模式
    /// </summary>
    private IGameplayMode _currentMode;

    /// <summary>
    /// 单局上下文
    /// </summary>
    private RoundContext _roundContext;
    public RoundContext RoundContext => _roundContext;

    /// <summary>
    /// 玩法规则字典
    /// </summary>
    private readonly Dictionary<Type, object> _playRules = new Dictionary<Type, object>();

    /// <summary>
    /// 单局管理器字典
    /// </summary>
    private readonly Dictionary<Type, IRoundManager> _roundManagers = new Dictionary<Type, IRoundManager>();

    private UIManager _uiManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private EffectManager _effectManager;
    private HuntingSoundManager _soundManager;

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
    /// 设置玩法规则
    /// </summary>
    /// <typeparam name="T">规则类型</typeparam>
    /// <param name="rule">规则实例</param>
    public void SetPlayRule<T>(T rule) where T : class
    {
        _playRules[typeof(T)] = rule;
    }

    /// <summary>
    /// 获取玩法规则
    /// </summary>
    /// <typeparam name="T">规则类型</typeparam>
    /// <returns>规则实例，未设置时返回 null</returns>
    public T GetPlayRule<T>() where T : class
    {
        if (_playRules.TryGetValue(typeof(T), out object rule))
            return rule as T;

        return null;
    }

    /// <summary>
    /// 清空玩法规则
    /// </summary>
    public void ClearPlayRules()
    {
        _playRules.Clear();
    }

    /// <summary>
    /// 开始单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public async UniTask EnterRound(RoundContext context)
    {
        _currentState = ERoundFlowState.Transitioning;

        // 初始化单局
        _roundContext = context;
        RegisterServices();
        RegisterRoundManagers();
        InitRoundManagers();

        // 进入主地图模式
        _currentMode = new MainMapMode(this);
        await _currentMode.EnterAsync();

        _currentState = ERoundFlowState.Playing;
    }

    /// <summary>
    /// 结束单局
    /// </summary>
    public async UniTask EndRound()
    {
        _currentState = ERoundFlowState.Transitioning;

        // 退出当前玩法模式
        ClearPlayRules();
        await _currentMode.ExitAsync(null);
        _currentMode = null;

        // 释放单局管理器
        DisposeRoundManagers();
        _roundManagers.Clear();
        _roundContext = null;

        // 清理对象池
        _effectManager.ClearAllEffects(true);
        _soundManager.ClearAllSounds(true);
        _gameObjectPoolManager.ClearAllPools();

        // 关闭游玩界面
        _uiManager.CloseUI("UIGameplay");

        // 加载准备场景
        await SceneManager.LoadSceneAsync("PrepareScene").ToUniTask();

        _currentState = ERoundFlowState.None;
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void DoUpdate(float dt)
    {
        switch (_currentState)
        {
            case ERoundFlowState.Playing:
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
        _currentState = ERoundFlowState.Settlement;
    }

    /// <summary>
    /// 暂停单局
    /// </summary>
    public void PauseRound()
    {
        _currentState = ERoundFlowState.Paused;
    }

    /// <summary>
    /// 恢复单局
    /// </summary>
    public void ResumeRound()
    {
        _currentState = ERoundFlowState.Playing;
    }

    /// <summary>
    /// 切换到下一个玩法模式
    /// </summary>
    /// <param name="nextMode">下一个玩法模式</param>
    public async UniTask SwitchToNextMode(EGameplayMode nextMode)
    {
        _currentState = ERoundFlowState.Transitioning;
        switch (nextMode)
        {
            case EGameplayMode.HiddenMap:
                await SwitchToHiddenMapAsync(nextMode);
                break;
        }
        _currentState = ERoundFlowState.Playing;
    }

    /// <summary>
    /// 切换到隐藏地图模式
    /// </summary>
    private async UniTask SwitchToHiddenMapAsync(EGameplayMode mode)
    {
        // 播放下雪特效和雪山环境音效
        var snowEffect = await _effectManager.PlayLoopAsync("Assets/Arts/Prefabs/Effects/FX_Snow_For_SnowMountainScene_UICamera",dontDestroyOnLoad:true);
        var envSoundCallback = _soundManager.PlayMapEnvSound(EMapType.Hidden,-1,true);

        // 播放假结算面板动画
        var uiPopupFakeSettlement = _uiManager.GetUI<UIPopupFakeSettlement>("UIPopupFakeSettlement");
        UniTask.WhenAll(
            uiPopupFakeSettlement.PlayWindowShakeAsync(),
            uiPopupFakeSettlement.PlayButtonGlowAsync(),
            uiPopupFakeSettlement.PlayPanelFadeOutAsync()
        ).Forget();
        
        // 播放过渡动画
        var uiLoading = await _uiManager.OpenUIAsync<UINormalLoading>("UINormalLoading", UIManager.UILayer.Loading);
        await uiLoading.PlayFadeInAsync();

        // 退出当前玩法模式
        await _currentMode.ExitAsync(EGameplayMode.HiddenMap);

        // 清理旧场景
        CleanupManagers();
        _effectManager.ClearAllEffects(); 
        _soundManager.ClearAllSounds();
        _gameObjectPoolManager.ClearAllPools();

        // 加载新场景
        await SceneManager.LoadSceneAsync("GameplaySnowMountainScene").ToUniTask();
        DynamicGI.UpdateEnvironment();
        ReInitManagers();

        // 进入隐藏地图模式
        _currentMode = new HiddenMapMode(new HiddenMapContext 
        { 
            SnowEffect = snowEffect,
            EnvSoundCallback = envSoundCallback 
        });
        await _currentMode.EnterAsync();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _effectManager = GameServiceLocator.EffectManager;
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
    }

    /// <summary>
    /// 注册单局管理器
    /// </summary>
    private void RegisterRoundManagers()
    {
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

    /// <summary>
    /// 注册单个单局管理器
    /// </summary>
    /// <param name="manager">管理器实例</param>
    private void RegisterRoundManager(IRoundManager manager)
    {
        _roundManagers[manager.GetType()] = manager;
    }

    /// <summary>
    /// 初始化单局管理器
    /// </summary>
    private void InitRoundManagers()
    {
        foreach (var manager in _roundManagers.Values)
            manager.Init(_roundContext);
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
                resettable.ReInit(_roundContext.HiddenMapData);
        }
    }

    /// <summary>
    /// 游玩中更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    private void OnRoundPlaying(float dt)
    {
        _currentMode?.DoUpdate(dt);

        foreach (var manager in _roundManagers.Values)
        {
            if (manager is IRoundUpdatable updatable)
                updatable.DoUpdate(dt);
        }
    }
    #endregion
}
