using UnityEngine;

/// <summary>
/// 动物移动接口
/// </summary>
public interface IAnimalMovement
{
    /// <summary>
    /// 设置移动方向
    /// </summary>
    /// <param name="direction">移动方向</param>
    void SetDirection(Vector3 direction);

    /// <summary>
    /// 设置移动速度
    /// </summary>
    /// <param name="speed">速度值</param>
    void SetSpeed(float speed);

    /// <summary>
    /// 启用移动
    /// </summary>
    void Enable();

    /// <summary>
    /// 禁用移动
    /// </summary>
    void Disable();

    /// <summary>
    /// 初始化移动组件
    /// </summary>
    /// <param name="speed">初始速度</param>
    /// <param name="direction">初始方向</param>
    void Initialize(float speed, Vector3 direction);
}

