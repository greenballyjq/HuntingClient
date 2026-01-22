using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通子弹效果
/// </summary>
public class NormalBulletEffect : IBulletEffect
{
    /// <summary>
    /// 命中处理
    /// </summary>
    public List<IDamageable> OnHit(BulletRuntimeContext context, BulletHitInfo hitInfo)
    {
        // 对命中目标造成伤害
        hitInfo.PrimaryTarget.TakeDamage(context.FinalDamage, hitInfo.HitPoint, Vector3.zero);

        // 返回命中的动物列表
        return new List<IDamageable> { hitInfo.PrimaryTarget };
    }
}