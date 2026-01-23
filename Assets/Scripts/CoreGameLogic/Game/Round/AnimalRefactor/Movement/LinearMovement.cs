using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 线性移动组件
    /// </summary>
    public class LinearMovement : MonoBehaviour, IMoveable
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
        /// 移动策略
        /// </summary>
        private IMovePolicy _movePolicy;

        /// <summary>
        /// 当前速度
        /// </summary>
        public float CurrentSpeed => _isMoving ? _baseSpeed * _moveRate : 0f;
        
        /// <summary>
        /// 当前目标变换组件
        /// </summary>
        public Transform CurrentTargetTransform => null;
        
        /// <summary>
        /// 当前目标方向
        /// </summary>
        public Vector3 CurrentTargetDirection => _currentTargetDirection;
        
        /// <summary>
        /// 当前移动方向
        /// </summary>
        public Vector3 CurrentMoveDirection => _currentMoveDirection;
        
        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool IsMoving => _isMoving;

        #region 公共方法 
        /// <summary>
        /// 初始化移动组件
        /// </summary>
        public void Init()
        {
            _baseSpeed = 0f;
            _moveRate = 1f;
            _currentTargetDirection = Vector3.zero;
            _currentMoveDirection = Vector3.zero;
            _isMoving = false;

            _movePolicy = new LinearMovePolicy(_baseSpeed, _moveRate);
        }

        public void Init(IMovePolicy movePolicy)
        {
            _baseSpeed = 0f;
            _moveRate = 1f;
            _currentTargetDirection = Vector3.zero;
            _currentMoveDirection = Vector3.zero;
            _isMoving = false;
            
            _movePolicy = movePolicy;
            _movePolicy.BaseSpeed = _baseSpeed;
            _movePolicy.MoveRate = _moveRate;
        }
        
        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            // 更新当前移动方向
            _currentMoveDirection = _currentTargetDirection;
            
            if (!_isMoving || _currentTargetDirection == Vector3.zero)
                return;
            
            // 更新位置
            float actualSpeed = _baseSpeed * _moveRate;
            Vector3 movement = _currentTargetDirection * actualSpeed * dt;
            transform.position += movement;
            // Debug.Log($"[{GetType().Name}] DoUpdate");
            // _movePolicy.DoMove(transform, dt);
        }
        
        /// <summary>
        /// 设置移动速度
        /// </summary>
        /// <param name="speed">速度值</param>
        public void SetSpeed(float speed)
        {
            _baseSpeed = speed;
            _movePolicy.BaseSpeed = speed;
        }
        
        /// <summary>
        /// 设置移动速率
        /// </summary>
        /// <param name="rate">速率倍数</param>
        public void SetMoveRate(float rate)
        {
            _moveRate = rate;
            _movePolicy.MoveRate = rate;
        }
        
        /// <summary>
        /// 设置移动方向
        /// </summary>
        /// <param name="direction">移动方向</param>
        public void SetDirection(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                _currentTargetDirection = Vector3.zero;
                return;
            }
            
            _currentTargetDirection = direction.normalized;
            _movePolicy.TargetDirection = _currentTargetDirection;
        }
        
        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="target">目标</param>
        public void SetTarget(Vector3 target) { }
        
        /// <summary>
        /// 开始移动
        /// </summary>
        public void StartMove()
        {
            _isMoving = true;
        }
        
        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopMove()
        {
            _isMoving = false;
        }
        #endregion
    }
}
