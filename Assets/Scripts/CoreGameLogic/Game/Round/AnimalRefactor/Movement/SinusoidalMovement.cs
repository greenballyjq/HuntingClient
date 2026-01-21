using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 三角函数式移动组件（用于测试）
    /// </summary>
    public class SinusoidalMovement : MonoBehaviour, IMoveable
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
        /// 摆动频率（每秒周期数）
        /// </summary>
        [SerializeField] private float _frequency = 1f;
        
        /// <summary>
        /// 摆动幅度（单位：米）
        /// </summary>
        [SerializeField] private float _amplitude = 1f;
        
        /// <summary>
        /// 时间累积器
        /// </summary>
        private float _time;

        /// <summary>
        /// 当前速度
        /// </summary>
        public float CurrentSpeed => _isMoving ? _baseSpeed * _moveRate : 0f;
        
        /// <summary>
        /// 当前目标位置
        /// </summary>
        public Vector3 CurrentTargetPosition => Vector3.zero;
        
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
            _time = 0f;
        }
        
        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            // 累积时间
            _time += dt;
            
            // 如果未移动或目标方向为零，则当前移动方向也为零
            if (!_isMoving || _currentTargetDirection == Vector3.zero)
            {
                _currentMoveDirection = Vector3.zero;
                return;
            }
            
            // 计算垂直于目标方向的向量（用于摆动）
            Vector3 perpendicular = new Vector3(-_currentTargetDirection.z, 0f, _currentTargetDirection.x);
            
            // 计算摆动量（使用 Sin 函数）
            float swingAmount = Mathf.Sin(_time * _frequency * 2f * Mathf.PI) * _amplitude;
            
            // 计算实际移动方向：目标方向 + 摆动分量
            Vector3 swingDirection = perpendicular * swingAmount;
            _currentMoveDirection = (_currentTargetDirection + swingDirection).normalized;
            
            // 更新位置
            float actualSpeed = _baseSpeed * _moveRate;
            Vector3 movement = _currentMoveDirection * actualSpeed * dt;
            transform.position += movement;
        }
        
        /// <summary>
        /// 设置移动速度
        /// </summary>
        /// <param name="speed">速度值</param>
        public void SetSpeed(float speed)
        {
            _baseSpeed = speed;
        }
        
        /// <summary>
        /// 设置移动速率
        /// </summary>
        /// <param name="rate">速率倍数</param>
        public void SetMoveRate(float rate)
        {
            _moveRate = rate;
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
