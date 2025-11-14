using cfg.HuntingConfig.Skill;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Skills
{
    /// <summary>
    /// 色块人技能处理器
    /// </summary>
    public class Skill3KPHandler : ISkillHandler
    {
        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager Weapon => GameServiceLocator.GetGameManager<WeaponManager>();

        #region 接口实现
        /// <summary>
        /// 技能开始
        /// </summary>
        public void OnSkillStart(SkillContext context)
        {
            var parameter = Config.GetSkill3KP(context.SkillData.ParamTableID);
            if (parameter == null)
            {
                Debug.LogWarning("[Skill3KPHandler] 未找到技能参数配置");
                return;
            }

            // 应用射速和伤害倍率
            Weapon.SetFireRateMultiplier(parameter.FireRateMultiplier);
            Weapon.SetDamageMultiplier(parameter.DamageMultiplier);

            Debug.Log($"[Skill3KPHandler] 技能开始，射速倍率: {parameter.FireRateMultiplier}, 伤害倍率: {parameter.DamageMultiplier}");
        }

        /// <summary>
        /// 技能更新
        /// </summary>
        public void OnSkillUpdate(SkillContext context, float deltaTime)
        {
            // 当前技能无需逐帧逻辑
        }

        /// <summary>
        /// 技能结束
        /// </summary>
        public void OnSkillEnd(SkillContext context)
        {
            // 重置倍率为1.0
            Weapon.SetFireRateMultiplier(1f);
            Weapon.SetDamageMultiplier(1f);

            Debug.Log("[Skill3KPHandler] 技能结束，已重置倍率");
        }
        #endregion
    }
}

