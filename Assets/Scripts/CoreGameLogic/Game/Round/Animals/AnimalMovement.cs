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
    /// 速度倍率
    /// </summary>
    private float _speedMultiplier = 1f;

    /// <summary>
    /// 是否启用移动
    /// </summary>
    private bool _isEnabled = true;

    private void Awake()
    {
        _direction = Vector3.zero;
    }

    /// <summary>
    /// 设置基础速度
    /// </summary>
    public void SetBaseSpeed(float speed)
    {
        _baseSpeed = speed;
    }

    /// <summary>
    /// 设置移动方向
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;
            
        _direction = direction.normalized;
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
        if (!_isEnabled || _direction == Vector3.zero)
            return;

        if (_direction.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (_direction.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

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

