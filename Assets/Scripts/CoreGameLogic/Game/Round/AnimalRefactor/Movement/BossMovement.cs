using Hunting.Game.Animal;
using UnityEngine;

public class BossMovement : MonoBehaviour, IMoveable
{
    /// <summary>
    /// 基础速度
    /// </summary>
    private float _baseSpeed;

    /// <summary>
    /// 当前目标变换组件
    /// </summary>
    private Transform _currentTargetTransform;

    /// <summary>
    /// 当前目标方向
    /// </summary>
    private Vector3 _currentTargetDirection;

    /// <summary>
    /// 当前移动方向
    /// </summary>
    private Vector3 _currentMoveDirection;

    /// <summary>
    /// 当前目标位置
    /// </summary>
    private Vector3 _currentTargetPosition;

    /// <summary>
    /// 移动速率倍数
    /// </summary>
    private float _moveRate;

    /// <summary>
    /// 是否正在移动
    /// </summary>
    private bool _isMoving;

    /// <summary>
    /// Boss移动点数组
    /// </summary>
    private BossMovePoint[] _movePoints;

    /// <summary>
    /// 当前速度
    /// </summary>
    public float CurrentSpeed => _isMoving ? _baseSpeed * _moveRate : 0f;

    public Transform CurrentTargetTransform => _currentTargetTransform;

    public Vector3 CurrentTargetDirection => _currentTargetDirection;

    public Vector3 CurrentMoveDirection => _currentMoveDirection;
    
    public IMovePolicy MovePolicy { get; private set; }

    public bool IsMoving => _isMoving;

    public Vector3 CurrentTargetPosition => Vector3.zero;

    public void Init()
    {
        _currentTargetDirection = Vector3.zero;
        _currentMoveDirection = Vector3.zero;
        _isMoving = false;

        // 收集场景上的所有点位
        _movePoints = FindObjectsOfType<BossMovePoint>();
        if (_movePoints.Length == 0)
        {
            Debug.LogError($"[{GetType().Name}] 没有找到任何移动点");
        }
    }

    public void Init(IMovePolicy movePolicy)
    {
        _currentTargetDirection = Vector3.zero;
        _currentMoveDirection = Vector3.zero;
        _isMoving = false;

        // 收集场景上的所有点位
        _movePoints = FindObjectsOfType<BossMovePoint>();
        if (_movePoints.Length == 0)
        {
            Debug.LogError($"[{GetType().Name}] 没有找到任何移动点");
        }
    }

    public void DoUpdate(float dt)
    {
        _currentMoveDirection = _currentTargetDirection;
        if (!_isMoving || _currentTargetDirection == Vector3.zero)
            return;
        
        if (IsNearbyTargetPosition())
        {
            SetRandomTargetPosition();
        }

        float actualSpeed = _baseSpeed * _moveRate;
        Vector3 movement = _currentTargetDirection * actualSpeed * dt;
        transform.position += movement;
    }

    public void SetSpeed(float speed)
    {
        _baseSpeed = speed;
    }

    public void SetMoveRate(float rate)
    {
        _moveRate = rate;
    }

    public void SetDirection(Vector3 direction)
    {
        _currentMoveDirection = direction;
    }

    public void SetTargetPosition(Vector3 target)
    {
        _currentTargetPosition = target;
    }

    public void StartMove()
    {
        _isMoving = true;
        SetRandomTargetPosition();
    }

    public void StopMove()
    {
        _isMoving = false;
    }

    public void ChangeMovePolicy(MovePolicyType policyType)
    {
        
    }

    /// <summary>
    /// 生成随机目标位置
    /// </summary>
    private void SetRandomTargetPosition()
    {
        if (_movePoints.Length == 0)
            return;


        if (_movePoints == null || _movePoints.Length < 2)
        {
            Debug.LogWarning("随机移动点数量不足，无法选择两个不同的点");
            return;
        }

        int index1 = UnityEngine.Random.Range(0, _movePoints.Length);
        int index2 = index1;

        // 确保第二个点与第一个点不同
        while (index2 == index1)
        {
            index2 = UnityEngine.Random.Range(0, _movePoints.Length);
        }

        var randomPoint1 = _movePoints[index1];
        var randomPoint2 = _movePoints[index2];

        float normalized = UnityEngine.Random.value;

        var randomPoint = randomPoint1.Target.position +
                          normalized * (randomPoint2.Target.position - randomPoint1.Target.position);

        _currentTargetPosition = randomPoint;
        _currentTargetDirection = (_currentTargetPosition - transform.position).normalized;
        // return randomPoint1.position + normalized * (randomPoint2.position - randomPoint1.position);
    }

    /// <summary>
    /// 是否靠近目标位置
    /// </summary>
    /// <returns></returns>
    private bool IsNearbyTargetPosition()
    {
        float minDistance = 1f;
        return Vector3.Distance(_currentTargetPosition, transform.position) < minDistance;
    }
}