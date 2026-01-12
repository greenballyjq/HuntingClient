using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.Core;
using GameFramework.Manager;

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
    private readonly EventManager _eventManager = GameServiceLocator.EventManager;
    private readonly AnimalManager _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
    
    private bool _bossDied;
    
    public void Init()
    {
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.AddListener(BossEvents.BossDied, OnBossDied);
        _bossDied = false;
    }

    public void Release()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        _eventManager.RemoveListener(BossEvents.BossDied, OnBossDied);
    }

    private void OnAnimalDied(AnimalDiedEventArgs obj)
    {
        CheckIsHiddenRoundEnd();
    }

    private void OnBossDied(BossDiedEventArgs obj)
    {
        _bossDied = true;
        CheckIsHiddenRoundEnd();
    }
    
    private void CheckIsHiddenRoundEnd()
    {
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
        /// 正在游玩
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
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 粒子特效管理器
    /// </summary>
    private ParticleEffectManager _particleEffectManager => GameServiceLocator.GetFrameworkManager<ParticleEffectManager>();

    /// <summary>
    /// 音效管理器
    /// </summary>
    private SoundManager _soundManager => GameServiceLocator.GetFrameworkManager<SoundManager>();

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
    /// 获取当前单局上下文
    /// </summary>
    public RoundContext GetRoundContext()
    {
        return _currentRoundContext;
    }

    /// <summary>
    /// 开始单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public async UniTask StartRound(RoundContext context)
    {
        #region 测试代码 将来会正式化
        _currentRoundContext = context;
        CreateRoundManagers();
        await _uiManager.OpenUIAsync<UIGameplay>("UIHuntingGameplay", UIManager.UILayer.Fixed);
        InitRoundManagers();

        // 等五秒
        await UniTask.Delay(5000);

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
        DisposeRoundManagers();
        _roundManagers.Clear();
        _currentRoundContext = null;
        _currentState = RoundFlowState.None;
        #endregion

        await UniTask.CompletedTask;
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
    /// 进入隐藏地图
    /// </summary>
    public async UniTask EnterHiddenMapAsync()
    {
        // 伪结算面板动画
        var uiFakeSettlement = _uiManager.GetUI<UIFakeSettlement>("UIFakeSettlement");
        await UniTask.WhenAll(
            uiFakeSettlement.PlayWindowShakeAsync(),
            uiFakeSettlement.PlayButtonGlowAsync()
        );
        _uiManager.CloseUI("UIFakeSettlement");

        // 伪结算面板爆米花动画  
        _particleEffectManager.SpawnParticleEffectAsync("Settlement_Explosion").Forget();

        // 下雪动画
        _particleEffectManager.SpawnParticleEffectAsync("Snow", autoDestroy: false).Forget();

        // 地图过渡动画
        var uiLoading = await _uiManager.OpenUIAsync<UILoading>("UILoading", UIManager.UILayer.Loading);

        await uiLoading.PlayFadeInAsync();

        CleanupManagers();
        await SceneManager.LoadSceneAsync("GameplaySnowMountainScene").ToUniTask();

        ReInitManagers();
        DynamicGI.UpdateEnvironment();
        await uiLoading.PlayFadeOutAsync();

        _uiManager.CloseUI("UILoading");

        // TODO: 未来步骤待考虑
        var animalManager = GetRoundManager<AnimalManager>();
        var uiAlertRed = await _uiManager.OpenUIAsync<UIAlertRed>("UIAlertRed", UIManager.UILayer.Normal);
        
        uiAlertRed.PlayFlashAsync().Forget();
        _soundManager.PlaySound2DByPath("Arts/Audio/SFX/sfx_alert");
        var boss = await animalManager.GenerateBossAsync();

        await UniTask.Delay(2000);

        _soundManager.PlaySound2DByPath("Arts/Audio/SFX/sfx_laugh");
        _soundManager.PlaySound2DByPath("Arts/Audio/BGM/bgm_snow", AudioChannel.Bgm,volume:0.1f);
        await UniTask.Delay(3000);

        boss.EnterCombat();
        
        _currentRoundContext.HiddenRoundEndTrigger.Init();
    }

    /// <summary>
    /// 结束隐藏地图
    /// </summary>
    public async UniTask EndHiddenMapProcessAsync()
    {
        _currentRoundContext.HiddenRoundEndTrigger.Release();
        // TODO: 打开结算面板三
        
        // TODO: 
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
    /// 游玩中更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    private void OnRoundPlaying(float dt)
    {
        for (int i = 0; i < _roundManagers.Count; i++)
        {
            if (_roundManagers[i] is IRoundUpdatable updatable)
                updatable.DoUpdate(dt);
        }
    }

    /// <summary>
    /// 清理管理器
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
    /// 重新初始化管理器
    /// </summary>
    private void ReInitManagers()
    {
        for (int i = 0; i < _roundManagers.Count; i++)
        {
            if (_roundManagers[i] is IRoundResettable resettable)
                resettable.ReInit(_currentRoundContext);
        }
    }
    #endregion
}
