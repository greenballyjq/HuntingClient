using UnityEngine;
using cfg.HuntingConfig.Bean;

/// <summary>
/// 开局丰收能量幸运仪式增益处理器
/// </summary>
public class LuckyBuffStartEnergyHandler : ILuckyBuffHandler
{
    /// <summary>
    /// 能量条管理器
    /// </summary>
    private EnergyProgressManager _energyProgressManager => GameServiceLocator.GetRoundManager<EnergyProgressManager>();


    /// <summary>
    /// 激活幸运仪式增益效果
    /// </summary>
    public void OnActivate(LuckyBuffContext context)
    {
        // 读取配置中的能量条条数
        var param = context.LuckyBuffData.EffectParam as LuckyBuffParamStartEnergy;
        int barCount = param.EnergyBarCount;

        // 获取单条所需能量值
        float requiredPerBar = _energyProgressManager.ValuePerBar;

        // 计算需要增加的能量总量 = 单条所需值 * 条数
        float totalEnergy = requiredPerBar * barCount;
        _energyProgressManager.AddEnergyValue(totalEnergy);
        Debug.Log($"[LuckyStartEnergyHandler] 开局获得 {barCount} 条丰收能量");
    }

    /// <summary>
    /// 注销幸运仪式增益效果
    /// </summary>
    public void OnDeactivate(LuckyBuffContext context)
    {

    }
}