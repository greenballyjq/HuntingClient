using UnityEngine;
using System;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 生命值组件
    /// </summary>
    public class Health : MonoBehaviour, IDamageable, IHealth
    {
        /// <summary>
        /// 最大血量
        /// </summary>
        private float _maxHealth;

        /// <summary>
        /// 当前血量
        /// </summary>
        private float _currentHealth;
        
        /// <summary>
        /// 是否已死亡
        /// </summary>
        private bool _isDead;

        /// <summary>
        /// 当前血量
        /// </summary>
        public float CurrentHealth => _currentHealth;
        
        /// <summary>
        /// 最大血量
        /// </summary>
        public float MaxHealth => _maxHealth;
        
        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead => _isDead;
        
        /// <summary>
        /// 受伤事件
        /// </summary>
        public event Action OnDamaged;
        
        /// <summary>
        /// 死亡事件
        /// </summary>
        public event Action OnDeath;

        #region 公共方法
        /// <summary>
        /// 初始化生命值
        /// </summary>
        /// <param name="maxHealth">最大血量</param>
        public void Init(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _isDead = false;
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="info">伤害信息</param>
        public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDir)
        {
            if (_isDead)
                return;
            
            _currentHealth = _currentHealth - amount;
            
            OnDamaged?.Invoke();
            
            if (_currentHealth <= 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
            }
        }
        #endregion
    }
}

