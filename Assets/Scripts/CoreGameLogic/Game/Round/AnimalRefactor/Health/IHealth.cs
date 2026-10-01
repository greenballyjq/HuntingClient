using System;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 生命值接口
    /// </summary>
    public interface IHealth
    {
        /// <summary>
        /// 当前血量
        /// </summary>
        float CurrentHealth { get; }
        
        /// <summary>
        /// 最大血量
        /// </summary>
        float MaxHealth { get; }
        
        /// <summary>
        /// 是否死亡
        /// </summary>
        bool IsDead { get; }
        
        /// <summary>
        /// 初始化生命值组件
        /// </summary>
        /// <param name="maxHealth">最大血量</param>
        void Init(float maxHealth);

        /// <summary>
        /// 立即死亡
        /// </summary>
        void Kill();
        
        /// <summary>
        /// 受伤事件
        /// </summary>
        event Action OnDamaged;
        
        /// <summary>
        /// 死亡事件
        /// </summary>
        event Action OnDeath;

        /// <summary>
        /// 控制事件
        /// </summary>
        event Action OnHeld;
    }
}
