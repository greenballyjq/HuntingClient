using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 线性移动组件
    /// </summary>
    public class AnimalMovement : MonoBehaviour, IMoveable
    {
        [SerializeField] private MovePolicyType _currentMovePolicyType;
        
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
        
        private Transform _guardTargetTransform;

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

            _currentMovePolicyType = MovePolicyType.Linear;
        }
        
        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            // 更新当前移动方向
            
            if (!_isMoving || _currentTargetDirection == Vector3.zero)
                return;
            
            // 更新位置
            // float actualSpeed = _baseSpeed * _moveRate;
            // Vector3 movement = _currentTargetDirection * actualSpeed * dt;
            // transform.position += movement;
            // Debug.Log($"[{GetType().Name}] DoUpdate");
            if (_currentMovePolicyType == MovePolicyType.Linear)
            {
                _currentMoveDirection = _currentTargetDirection;
                float actualSpeed = _baseSpeed * _moveRate;
                Vector3 movement = _currentTargetDirection * actualSpeed * dt;
                transform.position += movement;
            }
            else
            {
                if (Vector3.Distance(transform.position, _guardTargetTransform.position) > 5f)
                {
                    var dir = (_guardTargetTransform.position - transform.position).normalized;
                    _currentMoveDirection = dir;
                    float actualSpeed = _baseSpeed * _moveRate;
                    Vector3 movement = dir * actualSpeed * dt;
                    transform.position += movement;
                }
                else
                {
                    transform.RotateAround(_guardTargetTransform.position, Vector3.up, 36f * dt);
                    var angle = Vector3.SignedAngle(_guardTargetTransform.right, transform.position - _guardTargetTransform.position, Vector3.up);
                    if (angle > 0f)
                    {
                        _currentMoveDirection = Vector3.left;
                    }
                    else
                    {
                        _currentMoveDirection = Vector3.right;
                    }
                }
            }
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

        public void ChangeMovePolicy(MovePolicyType policyType)
        {
            _currentMovePolicyType = policyType;
            switch (policyType)
            {
                case MovePolicyType.Linear:
                    // MovePolicy = new LinearMovePolicy(_baseSpeed, _moveRate);
                    Debug.Log("[LinearMovement] 切换到线性策略");
                    break;
                case MovePolicyType.Guard:
                    _guardTargetTransform = Object.FindObjectOfType<BossAnimalBehaviour>().transform;
                    if (_guardTargetTransform == null)
                    {
                        Debug.LogError("[GuardMovePolicy] Could not find BossAnimalBehaviour");
                    }
                    // MovePolicy = new GuardMovePolicy(_baseSpeed, _moveRate);
                    Debug.Log("[LinearMovement] 切换到守卫策略");
                    break;
            }
        }

        #endregion
    }
}
