/// <summary>
/// 金状元技能处理器
/// </summary>
public class SkillJinZhuangYuanHandler : ISkillHandler
{
    /// <summary>
    /// 每秒增加肉量
    /// </summary>
    private float _meatIncreasePerSecond;

    /// <summary>
    /// 剩余时间
    /// </summary>
    private float _remainingTime;

    public SkillPhase SkillPhase { get; set; }

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 肉条管理器
    /// </summary>
    private MeatProgressManager _meatManager => GameServiceLocator.GetRoundManager<MeatProgressManager>();

    public void OnSkillStart(SkillContext context)
    {
        SkillPhase = SkillPhase.Starting;

        var skillParam = _configManager.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);
        _remainingTime = skillParam.Duration;

        float totalMeatAmount = _meatManager.TotalMeatValue * skillParam.MeatPercent;
        _meatIncreasePerSecond = totalMeatAmount / skillParam.Duration;

        SkillPhase = SkillPhase.Running;
    }

    public void DoUpdate(float dt)
    {
        _remainingTime -= dt;

        _meatManager.AddMeatValue(dt * _meatIncreasePerSecond);

        if (_remainingTime <= 0)
            SkillPhase = SkillPhase.Finished;
    }

    public void OnSkillEnd(){}
}