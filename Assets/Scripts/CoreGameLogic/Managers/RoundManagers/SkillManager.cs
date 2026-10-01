using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : IMapWorld, IRoundUpdatable
{
    private const int SkillEnergyBarCost = 1;
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
    private RoundNumericLayer _numeric;

    public UniTask InitAsync(RoundContext context)
    {
        BindServices();

        var skillData = context.SkillData;

        _currentHandler = SkillHandlerFactory.CreateSkillHandler(skillData.SkillType);

        _currentContext = new SkillContext
        {
            SkillData = skillData,
            RoundContext = context,
        };

        _currentHandler.Init(_currentContext);

        Log.Info("[SkillManager] 初始化完成");
        return UniTask.CompletedTask;
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

    public void Unbind()
    {
        EndSkill();
    }

    public void Bind(Map mapData) { }

    #region 公共方法
    public void TryStartSkill()
    {
        if (_currentHandler.SkillPhase != SkillPhase.None)
            return;

        int cost = _numeric.EvaluateSkillEnergyCost(SkillEnergyBarCost);
        if (!_energyProgressManager.TryConsumeEnergyBars(cost))
            return;

        StartSkill();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _energyProgressManager = GameServiceLocator.GetRoundManager<EnergyProgressManager>();
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
    }

    /// <summary>
    /// 开始技能
    /// </summary>
    private void StartSkill()
    {
        _currentHandler.StartSkill().Forget();

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
