using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Spine.Unity;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物视觉组件基类
    /// </summary>
    public class BaseAnimalVisual : MonoBehaviour
    {
        protected virtual void Awake()
        {
            _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
            _moveable = GetComponentInChildren<IMoveable>();

            _animator = GetComponentInChildren<Animator>();
        }

        /// <summary>
        /// 动物行为类
        /// </summary>
        protected BaseAnimalBehaviour _animalBehaviour;

        #region 公共方法
        /// <summary>
        /// 初始化动物视觉组件
        /// </summary>
        /// <param name="animalBehaviour">动物行为类</param>
        public void Init(BaseAnimalBehaviour animalBehaviour)
        {
            _animalBehaviour = animalBehaviour;

            UpdateDirectionVisual();

            ResetAnimationStates();
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            UpdateDirectionVisual();
        }
        #endregion       

        #region Spine视觉
        /// <summary>
        /// 骨骼动画组件
        /// </summary>
        private SkeletonMecanim _skeletonMecanim;

        /// <summary>
        /// 移动组件
        /// </summary>
        private IMoveable _moveable;
        
        /// <summary>
        /// 上一次的X方向
        /// </summary>
        private float _lastDirectionX;

        /// <summary>
        /// 更新方向视觉效果
        /// </summary>
        private void UpdateDirectionVisual()
        {
            float currentDirX = _moveable.CurrentMoveDirection.x;
            if (currentDirX != _lastDirectionX)
            {
                UpdateFlip(currentDirX);
                _lastDirectionX = currentDirX;
            }
        }

        /// <summary>
        /// 更新翻转
        /// </summary>
        /// <param name="directionX">X方向分量</param>
        private void UpdateFlip(float directionX)
        {
            if (directionX > 0)
                _skeletonMecanim.Skeleton.ScaleX = 1f;
            else if (directionX < 0)
                _skeletonMecanim.Skeleton.ScaleX = -1f;
        }
        #endregion

        #region Animator视觉
        /// <summary>
        /// 动画组件
        /// </summary>
        protected Animator _animator;

        /// <summary>
        /// 播放移动动画
        /// </summary>
        public void PlayMove()
        {
            ResetAnimationStates();
            _animator.SetBool("Move", true);
        }

        /// <summary>
        /// 播放死亡动画
        /// </summary>
        public void PlayDeath()
        {
            ResetAnimationStates();
            _animator.SetBool("Die", true);
        }

        /// <summary>
        /// 播放控制动画
        /// </summary>
        public void PlayHeld()
        {
            ResetAnimationStates();
            _animator.SetBool("Held", true);
        }

        /// <summary>
        /// 重置动画状态
        /// </summary>
        protected virtual void ResetAnimationStates()
        {
            _animator.SetBool("Move", false);
            _animator.SetBool("Die", false);
            _animator.SetBool("Held", false);
        }
        #endregion

        #region 特效视觉
        /// <summary>
        /// 特效管理器
        /// </summary>
        private EffectManager _effectManager => GameServiceLocator.GetFrameworkManager<EffectManager>();

        /// <summary>
        /// 播放死亡特效
        /// </summary>
        public void PlayDeathEffect()
        {
            _effectManager.PlayOneShotAsync(_animalBehaviour.SpecieData.EffectPrefabResourcePath, transform.position, Quaternion.identity).Forget();
        }
        #endregion
    }
}
