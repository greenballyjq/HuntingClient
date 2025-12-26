using Hunting.App;
using Hunting.Manager;
using UnityEngine;
using cfg.HuntingConfig.Bean;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 伤害提升幸运仪式增益处理器
    /// </summary>
    public class LuckyBuffDamageBoostHandler : ILuckyBuffHandler
    {
        /// <summary>
        /// 修正来源ID
        /// </summary>
        private const string ModifierSourceId = "Lucky_DamageBoost";

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

        /// <summary>
        /// 激活幸运仪式增益效果
        /// </summary>
        public void OnActivate(LuckyBuffContext context)
        {
            // 读取配置中的伤害提升倍率
            var param = context.LuckyBuffData.EffectParam as LuckyBuffParamDamageBoost;
            float damageMultiplier = param.DamageIncreaseMultiplier;

            // 注册伤害修正倍率
            _weaponManager.RegisterDamageModifier(ModifierSourceId, damageMultiplier);
        }

        /// <summary>
        /// 注销幸运仪式增益效果
        /// </summary>
        public void OnDeactivate(LuckyBuffContext context)
        {
            // 注销伤害修正倍率
            _weaponManager.UnregisterDamageModifier(ModifierSourceId);
        }
    }
}

