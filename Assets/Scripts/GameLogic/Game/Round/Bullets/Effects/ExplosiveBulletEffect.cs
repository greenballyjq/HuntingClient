using cfg.HuntingConfig;
using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Game.Bullets.Effects
{
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
        public List<AnimalBehavior> OnHit(BulletRuntimeContext context, BulletHitInfo hitInfo)
        {
            // 记录已处理的动物，避免重复伤害
            HashSet<AnimalBehavior> processed = new HashSet<AnimalBehavior>
            {
                hitInfo.PrimaryTarget
            };

            // 对主要目标造成伤害
            hitInfo.PrimaryTarget.TakeDamage(context.FinalDamage, hitInfo.HitPoint);

            // 检测爆炸范围内的所有动物
            List<AnimalBehavior> hitAnimals = new List<AnimalBehavior> { hitInfo.PrimaryTarget };
            Collider[] colliders = Physics.OverlapSphere(hitInfo.HitPoint, _radius, LayerMask.GetMask("Animal"));
            
            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent(out AnimalBehavior animal))
                    continue;

                if (!processed.Add(animal))
                    continue;

                // 对范围内的其他动物造成伤害
                animal.TakeDamage(context.FinalDamage, hitInfo.HitPoint);
                hitAnimals.Add(animal);
            }

            return hitAnimals;
        }
    }
}


