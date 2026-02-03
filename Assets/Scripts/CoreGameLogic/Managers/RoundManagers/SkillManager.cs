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
    /// 技能剩余时间
    /// </summary>
    private float _remainingTime;

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
        // 读取本局技能配置
        Skill skillData = context.SkillData;

        // 创建处理器
        _currentSkillHandler = SkillHandlerFactory.CreateSkillHandler(skillData.SkillType);

        // 构造上下文
        _currentSkillContext = new SkillContext
        {
            SkillData = skillData,
            RoundContext = context
        };
            
        Debug.Log("[SkillManager] 初始化完成");
    }

    public void DoUpdate(float dt)
    {
        _remainingTime -= dt;
        _currentSkillHandler.OnSkillUpdate(dt);

        if (_remainingTime <= 0f)
            EndSkill();
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

    #region 公共方法
    /// <summary>
    /// 尝试启动技能
    /// </summary>
    public bool TryStartSkill()
    {
        if (!_energyProgressManager.UseEnergyOneBar())
            return false;

        BeginSkill();
        return true;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 启动技能
    /// </summary>
    private void BeginSkill()
    {
        _remainingTime = _currentSkillContext.SkillData.Duration;

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

        _remainingTime = 0f;

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
