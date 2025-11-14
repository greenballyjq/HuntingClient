using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 伤害提升幸运仪式处理器
    /// </summary>
    public class LuckyDamageBoostHandler : ILuckyHandler
    {
        /// <summary>
        /// 修正来源ID
        /// </summary>
        private const string ModifierSourceId = "Lucky_DamageBoost";

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager Weapon => GameServiceLocator.GetGameManager<WeaponManager>();

        /// <summary>
        /// 激活幸运仪式效果
        /// </summary>
        public void OnActivate(LuckyContext context)
        {
            // 读取配置中的伤害提升倍率
            float damageMultiplier = context.LuckyData.EffectParamFloat;

            // 注册伤害修正倍率
            Weapon.RegisterDamageModifier(ModifierSourceId, damageMultiplier);
            Debug.Log($"[LuckyDamageBoostHandler] 伤害提升已激活，倍率: {damageMultiplier:F2}");
        }

        /// <summary>
        /// 注销幸运仪式效果
        /// </summary>
        public void OnDeactivate(LuckyContext context)
        {
            // 注销伤害修正倍率
            Weapon.UnregisterDamageModifier(ModifierSourceId);
        }
    }
}

