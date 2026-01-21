using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 受伤接口
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="amount">伤害值</param>
        /// <param name="hitPoint">命中点</param>
        /// <param name="hitDir">命中方向</param>
        void TakeDamage(float amount, Vector3 hitPoint = default, Vector3 hitDir = default);
    }
}

