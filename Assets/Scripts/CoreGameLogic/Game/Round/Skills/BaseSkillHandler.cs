using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 技能处理器基类
/// </summary>
public abstract class BaseSkillHandler : ISkillHandler
{
    /// <summary>
    /// 技能阶段
    /// </summary>
    public SkillPhase SkillPhase { get; private set; }

    /// <summary>
    /// 技能上下文
    /// </summary>
    protected SkillContext SkillContext;

    /// <summary>
    /// 剩余时间
    /// </summary>
    private float _remainingTime;
    
    /// <summary>
    /// 玩家变换组件
    /// </summary>
    protected Transform Player;

    /// <summary>
    /// 可游玩区域
    /// </summary>
    protected BaseAreaShape PlayableArea;

    protected EffectManager EffectManager;
    protected HuntingConfigManager ConfigManager;
    protected HuntingSoundManager _soundManager;
    protected GameplaySceneItemManager GameplaySceneItemManager;

    public void Init(SkillContext context)
    {
        EffectManager = GameServiceLocator.EffectManager;
        ConfigManager = GameServiceLocator.ConfigManager;
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
        GameplaySceneItemManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();

        SkillContext = context;

        OnInit();
    }

    public async UniTask StartSkill()
    {
        SkillPhase = SkillPhase.Starting;

        Player = GameplaySceneItemManager.Player;
        PlayableArea = GameplaySceneItemManager.PlayableArea;

        _remainingTime = SkillContext.SkillData.Duration;

        await OnSkillStart();

        SkillPhase = SkillPhase.Running;
    }

    public void DoUpdate(float dt)
    {
        _remainingTime -= dt;

        if (_remainingTime <= 0)
            SkillPhase = SkillPhase.Finished;

        OnSkillUpdate(dt);
    }

    public void EndSkill()
    {
        OnSkillEnd();
        SkillPhase = SkillPhase.None;
    }

    /// <summary>
    /// 初始化钩子
    /// </summary>
    protected abstract void OnInit();

    /// <summary>
    /// 技能开始钩子
    /// </summary>
    protected abstract UniTask OnSkillStart();

    /// <summary>
    /// 技能更新钩子
    /// </summary>
    /// <param name="dt">时间增量</param>
    protected abstract void OnSkillUpdate(float dt);

    /// <summary>
    /// 技能结束钩子
    /// </summary>
    protected abstract void OnSkillEnd();
}
