using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 开局丰收能量幸运仪式处理器
    /// </summary>
    public class LuckyStartEnergyHandler : ILuckyHandler
    {
        /// <summary>
        /// 能量条管理器
        /// </summary>
        private EnergyProgressManager Energy => GameServiceLocator.GetGameManager<EnergyProgressManager>();

        /// <summary>
        /// 激活幸运仪式效果
        /// </summary>
        public void OnActivate(LuckyContext context)
        {
            // 读取配置中的能量条条数
            int barCount = context.LuckyData.EffectParamInt;

            // 获取单条所需能量值
            float requiredPerBar = Energy.GetRequiredPerBar();

            // 计算需要增加的能量总量 = 单条所需值 * 条数
            float totalEnergy = requiredPerBar * barCount;
            Energy.AddEnergy(totalEnergy);
            Debug.Log($"[LuckyStartEnergyHandler] 开局获得 {barCount} 条丰收能量");
        }

        /// <summary>
        /// 注销幸运仪式效果
        /// </summary>
        public void OnDeactivate(LuckyContext context)
        {

        }
    }
}

