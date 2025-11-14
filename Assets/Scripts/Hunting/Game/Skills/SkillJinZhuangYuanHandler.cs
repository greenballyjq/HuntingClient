using cfg.HuntingConfig.Skill;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Skills
{
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
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 肉度条管理器
        /// </summary>
        private MeatProgressManager Meat => GameServiceLocator.GetGameManager<MeatProgressManager>();

        /// <summary>
        /// 技能开始
        /// </summary>
        public void OnSkillStart(SkillContext context)
        {
            var parameter = Config.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);

            // 计算总增加量 = 单条所需值 * 肉量百分比
            float requiredPerBar = Meat.GetRequiredPerBar();
            float totalMeatAmount = requiredPerBar * parameter.MeatPercent;

            // 计算每秒增加量 = 总增加量 / 技能持续时间
            float skillDuration = context.SkillData.Duration;
            _meatIncreasePerSecond = totalMeatAmount / skillDuration;
        }

        /// <summary>
        /// 技能更新
        /// </summary>
        public void OnSkillUpdate(SkillContext context, float deltaTime)
        {
            if (_meatIncreasePerSecond <= 0f)
                return;

            // 累积本帧增加的肉量
            float deltaMeat = deltaTime * _meatIncreasePerSecond;
            if (deltaMeat > 0f)
                Meat.AddMeat(deltaMeat);
        }

        /// <summary>
        /// 技能结束
        /// </summary>
        public void OnSkillEnd(SkillContext context)
        {
            _meatIncreasePerSecond = 0f;
        }
    }
}

