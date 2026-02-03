using cfg.HuntingConfig.Skill;
using UnityEngine;


/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 当前技能处理器
    /// </summary>
    private ISkillHandler _currentSkillHandler;

    /// <summary>
    /// 当前技能上下文
    /// </summary>
    private SkillContext _currentSkillContext;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager = GameServiceLocator.EventManager;

    /// <summary>
    /// 能量条管理器
    /// </summary>
    private EnergyProgressManager _energyProgressManager = GameServiceLocator.GetRoundManager<EnergyProgressManager>();

    public void Init(RoundContext context)
    {
        Skill skillData = context.SkillData;

        _currentSkillHandler = SkillHandlerFactory.CreateSkillHandler(skillData.SkillType);

        _currentSkillContext = new SkillContext
        {
            SkillData = skillData,
            RoundContext = context,
        };

        Debug.Log("[SkillManager] 初始化完成");
    }

    public void DoUpdate(float dt)
    {
        if (_currentSkillHandler.SkillPhase is SkillPhase.Finished)
            EndSkill();

        if (_currentSkillHandler.SkillPhase is SkillPhase.Running)
            _currentSkillHandler.DoUpdate(dt);
    }

    public void Dispose()
    {
        EndSkill();

        Debug.Log("[SkillManager] 已释放");
    }

    public void Cleanup()
    {
        EndSkill();
    }

    public void ReInit(RoundContext context){}

    #region 公有方法
    public void TryStartSkill()
    {
        if(_currentSkillHandler.SkillPhase is SkillPhase.None || _currentSkillHandler.SkillPhase is SkillPhase.Finished)
        {
            if (!_energyProgressManager.UseEnergyOneBar())
                return;

            StartSkill();
        }   
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 启动技能
    /// </summary>
    private void StartSkill()
    {
        _currentSkillHandler.OnSkillStart(_currentSkillContext);

        TriggerSkillStarted(new SkillStartedEventArgs
        {
            Sender = this,
            SkillData = _currentSkillContext.SkillData
        });
    }

    /// <summary>
    /// 结束技能
    /// </summary>
    private void EndSkill()
    {
        _currentSkillHandler.OnSkillEnd();

        TriggerSkillEnded(new SkillEndedEventArgs
        {
            Sender = this,
            SkillData = _currentSkillContext.SkillData
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
