using Hunting.Game.Animal;
using UnityEngine;

namespace CoreGameLogic.Game.Round.Boss
{
    public class BossMovement : MonoBehaviour, IMoveable
    {
        /// <summary>
        /// 基础速度
        /// </summary>
        private float _baseSpeed;

        /// <summary>
        /// 当前目标方向
        /// </summary>
        private Vector3 _currentTargetDirection;

        /// <summary>
        /// 当前移动方向
        /// </summary>
        private Vector3 _currentMoveDirection;

        /// <summary>
        /// 移动速率倍数
        /// </summary>
        private float _moveRate;

        /// <summary>
        /// 是否正在移动
        /// </summary>
        private bool _isMoving;
        
        /// <summary>
        /// 当前速度
        /// </summary>
        public float CurrentSpeed => _isMoving ? _baseSpeed * _moveRate : 0f;
        
        
        public Vector3 CurrentTargetPosition { get; }


        public Vector3 CurrentTargetDirection => _currentTargetDirection;


        public Vector3 CurrentMoveDirection => _currentMoveDirection;


        public bool IsMoving => _isMoving;
        
        
        public void Init()
        {
            
        }

        public void DoUpdate(float dt)
        {
            
        }

        public void SetSpeed(float speed)
        {
            
        }

        public void SetMoveRate(float rate)
        {
            
        }

        public void SetDirection(Vector3 direction)
        {
            
        }

        public void SetTarget(Vector3 target)
        {
            
        }

        public void StartMove()
        {
            
        }

        public void StopMove()
        {
            
        }
    }
}