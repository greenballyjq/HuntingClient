using Cysharp.Threading.Tasks;

/// <summary>
/// 金状元技能处理器
/// </summary>
public class SkillJinZhuangYuanHandler : BaseSkillHandler
{
    /// <summary>
    /// 肉条每秒增加量
    /// </summary>
    private float _meatIncreasePerSecond;

    /// <summary>
    /// 肉条管理器
    /// </summary>
    private MeatProgressManager _meatManager => GameServiceLocator.GetRoundManager<MeatProgressManager>();

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);

        float totalIncreaseMeat = _meatManager.TotalMeatValue * skillParam.MeatPercent;
        _meatIncreasePerSecond = totalIncreaseMeat / context.SkillData.Duration;

        await UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt)
    {
        _meatManager.AddMeatValue(dt * _meatIncreasePerSecond);
    }

    protected override void OnSkillEnd() {}
}