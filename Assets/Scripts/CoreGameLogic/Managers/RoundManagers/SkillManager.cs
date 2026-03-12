using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 当前处理器
    /// </summary>
    private BaseSkillHandler _currentHandler;

    /// <summary>
    /// 当前上下文
    /// </summary>
    private SkillContext _currentContext;

    private EventManager _eventManager;
    private EnergyProgressManager _energyProgressManager;

    public void Init(RoundContext context)
    {
        RegisterServices();

        var skillData = context.SkillData;

        _currentHandler = SkillHandlerFactory.CreateSkillHandler(skillData.SkillType);

        _currentContext = new SkillContext
        {
            SkillData = skillData,
            RoundContext = context,
        };

        Log.Info("[SkillManager] 初始化完成");
    }

    public void DoUpdate(float dt)
    {
        if (_currentHandler.SkillPhase is SkillPhase.Finished)
            EndSkill();
            
        if (_currentHandler.SkillPhase is SkillPhase.Running)
            _currentHandler.DoUpdate(dt);
    }

    public void Dispose()
    {
        EndSkill();

        Log.Info("[SkillManager] 已释放");
    }

    public void Cleanup()
    {
        EndSkill();
    }

    public void ReInit(RoundContext context){}

    #region 公共方法
    public void TryStartSkill()
    {
        if (_currentHandler.SkillPhase != SkillPhase.None)
            return;

        if (!_energyProgressManager.UseEnergyOneBar())
            return;

        StartSkill();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _energyProgressManager = GameServiceLocator.GetRoundManager<EnergyProgressManager>();
    }

    /// <summary>
    /// 开始技能
    /// </summary>
    private void StartSkill()
    {
        _currentHandler.StartSkill(_currentContext).Forget();

        TriggerSkillStarted(new SkillStartedEventArgs
        {
            Sender = this,
            SkillData = _currentContext.SkillData
        });
    }

    /// <summary>
    /// 结束技能
    /// </summary>
    private void EndSkill()
    {
        _currentHandler.EndSkill();

        TriggerSkillEnded(new SkillEndedEventArgs
        {
            Sender = this,
            SkillData = _currentContext.SkillData
        });
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 触发技能开始事件
    /// </summary>
    private void TriggerSkillStarted(SkillStartedEventArgs args)
    {
        _eventManager.Trigger(SkillEvents.SkillStarted, args);
    }

    /// <summary>
    /// 触发技能结束事件
    /// </summary>
    private void TriggerSkillEnded(SkillEndedEventArgs args)
    {
        _eventManager.Trigger(SkillEvents.SkillEnded, args);
    }
    #endregion
}
