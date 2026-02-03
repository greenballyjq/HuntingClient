/// <summary>
/// 金状元技能处理器
/// </summary>
public class SkillJinZhuangYuanHandler : ISkillHandler
{
    /// <summary>
    /// 每秒增加的肉量
    /// </summary>
    private float _meatIncreasePerSecond;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 肉度条管理器
    /// </summary>
    private MeatProgressManager _meatManager => GameServiceLocator.GetRoundManager<MeatProgressManager>();

    public void OnSkillStart(SkillContext context)
    {
        var parameter = _configManager.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);

        // 计算总增加量 = 单条所需值 * 肉量百分比
        float requiredPerBar = _meatManager.TotalMeatValue;
        float totalMeatAmount = requiredPerBar * parameter.MeatPercent;

        // 计算每秒增加量 = 总增加量 / 技能持续时间
        float skillDuration = context.SkillData.Duration;
        _meatIncreasePerSecond = totalMeatAmount / skillDuration;
    }

    public void OnSkillUpdate(float dt)
    {
        if (_meatIncreasePerSecond <= 0f)
            return;

        // 累积本帧增加的肉量
        float deltaMeat = dt * _meatIncreasePerSecond;
        if (deltaMeat > 0f)
            _meatManager.AddMeatValue(deltaMeat);
    }

    public void OnSkillEnd()
    {
        _meatIncreasePerSecond = 0f;
    }
}