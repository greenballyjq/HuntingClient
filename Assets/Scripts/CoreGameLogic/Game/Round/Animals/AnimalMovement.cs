using UnityEngine;

/// <summary>
/// 动物移动组件
/// </summary>
public class AnimalMovement : MonoBehaviour, IAnimalMovement
{
    /// <summary>
    /// 基础移动速度
    /// </summary>
    private float _baseSpeed;

    /// <summary>
    /// 移动方向
    /// </summary>
    private Vector3 _direction;

    /// <summary>
    /// 目标朝向（用于插值旋转）
    /// </summary>
    private Vector3 _targetDirection;

    /// <summary>
    /// 速度倍率
    /// </summary>
    private float _speedMultiplier = 1f;

    /// <summary>
    /// 是否启用移动
    /// </summary>
    private bool _isEnabled = true;

    /// <summary>
    /// 旋转速度（度/秒）
    /// </summary>
    [SerializeField] private float _rotationSpeed = 360f;

    /// <summary>
    /// 初始化时设置目标方向为当前朝向
    /// </summary>
    private void Awake()
    {
        _targetDirection = transform.forward;
        _direction = transform.forward;
    }

    /// <summary>
    /// 设置基础速度
    /// </summary>
    public void SetBaseSpeed(float speed)
    {
        _baseSpeed = speed;
    }

    /// <summary>
    /// 设置移动方向（会平滑旋转到目标方向）
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;
            
        _targetDirection = direction.normalized;
    }

    /// <summary>
    /// 设置速度倍率
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }

    /// <summary>
    /// 启用/禁用移动
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        _isEnabled = enabled;
    }

    /// <summary>
    /// 更新移动
    /// </summary>
    public void UpdateMovement(float deltaTime)
    {
        if (!_isEnabled)
            return;

        // 平滑旋转到目标方向
        if (_targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
            float maxRotationDelta = _rotationSpeed * deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationDelta);
            
            // 更新当前移动方向为当前朝向
            _direction = transform.forward;
        }

        // 移动
        float finalSpeed = _baseSpeed * _speedMultiplier;
        Vector3 movement = _direction * finalSpeed * deltaTime;
        transform.position += movement;
    }

    #region IAnimalMovement 实现

    void IAnimalMovement.SetDirection(Vector3 direction)
    {
        SetDirection(direction);
    }

    public void SetSpeed(float speed)
    {
        // AnimalMovement 需要同时设置基础速度和倍率
        // 这里假设直接设置最终速度，将倍率设为1
        _speedMultiplier = 1f;
        _baseSpeed = speed;
    }

    void IAnimalMovement.Enable()
    {
        SetEnabled(true);
    }

    void IAnimalMovement.Disable()
    {
        SetEnabled(false);
    }

    public void Initialize(float speed, Vector3 direction)
    {
        SetBaseSpeed(speed);
        SetDirection(direction);
        SetEnabled(true);
    }

    #endregion
}

