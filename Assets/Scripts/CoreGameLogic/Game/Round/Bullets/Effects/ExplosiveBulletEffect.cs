using cfg.HuntingConfig;
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
        // 记录已处理的动物，避免重复伤害
        HashSet<IDamageable> processed = new HashSet<IDamageable>
        {
            hitInfo.PrimaryTarget
        };

        // 对主要目标造成伤害
        hitInfo.PrimaryTarget.TakeDamage(context.FinalDamage, hitInfo.HitPoint, Vector3.zero);

        // 检测爆炸范围内的所有动物
        List<IDamageable> hitTargets = new List<IDamageable> { hitInfo.PrimaryTarget };
        Collider[] colliders = Physics.OverlapSphere(hitInfo.HitPoint, _radius, LayerMask.GetMask("Animal"));
        
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out AnimalBehavior animal))
                continue;

            if (!processed.Add(animal))
                continue;

            // 对范围内的其他动物造成伤害
            animal.TakeDamage(context.FinalDamage, hitInfo.HitPoint);
            hitTargets.Add(animal);
        }

        return hitTargets;
    }
}
