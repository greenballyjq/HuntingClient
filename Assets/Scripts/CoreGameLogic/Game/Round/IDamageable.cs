using UnityEngine;

/// <summary>
/// 可攻击目标接口
/// 所有能被子弹命中的目标都应该实现此接口
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="hitPoint">命中点</param>
    /// <param name="hitNormal">命中法线</param>
    void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}