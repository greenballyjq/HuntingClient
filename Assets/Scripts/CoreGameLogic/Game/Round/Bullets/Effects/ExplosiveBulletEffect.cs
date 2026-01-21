using cfg.HuntingConfig;
using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆炸子弹效果
/// </summary>
public class ExplosiveBulletEffect : IBulletEffect
{
    /// <summary>
    /// 爆炸半径
    /// </summary>
    private float _radius;

    public ExplosiveBulletEffect(Bullet bulletData)
    {
        _radius = bulletData.EffectParamFloat;
    }

    /// <summary>
    /// 命中处理
    /// </summary>
    public List<IDamageable> OnHit(BulletRuntimeContext context, BulletHitInfo hitInfo)
    {
        HashSet<IDamageable> processed = new HashSet<IDamageable>
        {
            hitInfo.PrimaryTarget
        };

        // 对主要目标造成伤害
        hitInfo.PrimaryTarget.TakeDamage(context.FinalDamage, hitInfo.HitPoint, Vector3.zero);

        // 检测爆炸范围内的所有可伤害物体
        List<IDamageable> hitTargets = new List<IDamageable> { hitInfo.PrimaryTarget };
        Collider[] colliders = Physics.OverlapSphere(hitInfo.HitPoint, _radius, LayerMask.GetMask("Animal"));
        
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out IDamageable damageable))
                continue;

            if (!processed.Add(damageable))
                continue;

            // 对范围内的其他可伤害物体造成伤害
            damageable.TakeDamage(context.FinalDamage, hitInfo.HitPoint);
            hitTargets.Add(damageable);
        }

        return hitTargets;
    }
}
