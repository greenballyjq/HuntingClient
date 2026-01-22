using UnityEngine;
using Spine.Unity;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物视觉组件
    /// </summary>
    public class AnimalVisual : MonoBehaviour
    {
        /// <summary>
        /// 骨骼动画组件
        /// </summary>
        private SkeletonMecanim _skeletonMecanim;
        
        /// <summary>
        /// 动物基类
        /// </summary>
        private AnimalBehaviour _animalBehavior;
        
        /// <summary>
        /// 移动组件
        /// </summary>
        private IMoveable _moveable;
        
        /// <summary>
        /// 上一次的X方向
        /// </summary>
        private float _lastDirectionX;

        private void Awake()
        {
            _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
        }

        #region 公共方法
        /// <summary>
        /// 初始化视觉组件
        /// </summary>
        /// <param name="animalBehavior">动物基类</param>
        public void Init(AnimalBehaviour animalBehavior)
        {
            _animalBehavior = animalBehavior;
            _moveable = animalBehavior.Moveable;
            
            float currentDirX = _moveable.CurrentMoveDirection.x;

            _lastDirectionX = currentDirX;
            UpdateFlip(currentDirX);
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            float currentDirX = _moveable.CurrentMoveDirection.x;
            
            if (currentDirX != _lastDirectionX)
            {
                UpdateFlip(currentDirX);
                _lastDirectionX = currentDirX;
            }
        }
        #endregion

        #region 私有方法
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
    }
}
