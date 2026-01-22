using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物动画组件
    /// </summary>
    public class AnimalAnimator : MonoBehaviour
    {
        /// <summary>
        /// 动画组件
        /// </summary>
        protected Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        #region 公共方法
        /// <summary>
        /// 初始化动画组件
        /// </summary>
        public void Init()
        {
            ResetAll();
        }
        
        /// <summary>
        /// 播放移动动画
        /// </summary>
        public void PlayMove()
        {
            ResetAll();
            _animator.SetBool("Move", true);
        }
        
        /// <summary>
        /// 播放受击动画
        /// </summary>
        public void PlayHit()
        {
            ResetAll();
            _animator.SetBool("Hit", true);
        }
        
        /// <summary>
        /// 播放死亡动画
        /// </summary>
        public void PlayDeath()
        {
            ResetAll();
            _animator.SetBool("Death", true);
        }
        
        /// <summary>
        /// 播放逃跑动画
        /// </summary>
        public void PlayFlee()
        {
            ResetAll();
            _animator.SetBool("Flee", true);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 重置所有状态
        /// </summary>
        protected virtual void ResetAll()
        {
            _animator.SetBool("Move", false);
            _animator.SetBool("Hit", false);
            _animator.SetBool("Death", false);
            _animator.SetBool("Flee", false);
        }
        #endregion
    }
}
