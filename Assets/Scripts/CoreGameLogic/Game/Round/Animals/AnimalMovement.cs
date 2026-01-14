using UnityEngine;

/// <summary>
/// 动物移动组件
/// </summary>
public class AnimalMovement : MonoBehaviour
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
        if (!_isEnabled)
            return;

        float finalSpeed = _baseSpeed * _speedMultiplier;
        Vector3 movement = _direction * finalSpeed * deltaTime;
        transform.position += movement;
    }
}

