using Cysharp.Threading.Tasks;
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
    /// 剩余时间
    /// </summary>
    private float _remainingTime;
    
    /// <summary>
    /// 玩家变换组件
    /// </summary>
    protected Transform _player;

    /// <summary>
    /// 配置管理器
    /// </summary>
    protected HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    public async UniTask StartSkill(SkillContext context)
    {
        SkillPhase = SkillPhase.Starting;

        _remainingTime = context.SkillData.Duration;
        _player = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>().Player;

        await OnSkillStart(context);

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
    /// 技能开始钩子
    /// </summary>
    /// <param name="context">技能上下文</param>
    protected abstract UniTask OnSkillStart(SkillContext context);

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
