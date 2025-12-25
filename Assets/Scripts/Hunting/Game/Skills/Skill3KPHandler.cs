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
        /// 技能修正来源ID
        /// </summary>
        private const string ModifierSourceId = "Skill_3KP";

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager Weapon => GameServiceLocator.GetHuntingAppManager<WeaponManager>();

        /// <summary>
        /// 技能开始
        /// </summary>
        public void OnSkillStart(SkillContext context)
        {
            var parameter = Config.GetSkill3KP(context.SkillData.ParamTableID);

            // 注册射速和伤害倍率修正
            Weapon.RegisterFireRateModifier(ModifierSourceId, parameter.FireRateMultiplier);
            Weapon.RegisterDamageModifier(ModifierSourceId, parameter.DamageMultiplier);
        }

        /// <summary>
        /// 技能更新
        /// </summary>
        public void OnSkillUpdate(SkillContext context, float deltaTime)
        {

        }

        /// <summary>
        /// 技能结束
        /// </summary>
        public void OnSkillEnd(SkillContext context)
        {
            // 注销射速和伤害倍率修正
            Weapon.UnregisterFireRateModifier(ModifierSourceId);
            Weapon.UnregisterDamageModifier(ModifierSourceId);
        }
    }
}

