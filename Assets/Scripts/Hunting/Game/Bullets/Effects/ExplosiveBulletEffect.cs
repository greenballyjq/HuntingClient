using cfg.HuntingConfig;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Game.Bullets.Effects
{
    /// <summary>
    /// 爆炸效果
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
        public void OnHit(BulletEffectContext context, BulletHitInfo hitInfo)
        {
            // 记录已处理的动物，避免重复伤害
            HashSet<Animal.AnimalBehavior> processed = new HashSet<Animal.AnimalBehavior>
            {
                hitInfo.PrimaryTarget
            };

            Collider[] colliders = Physics.OverlapSphere(hitInfo.HitPoint, _radius, LayerMask.GetMask("Animal"));
            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent(out Animal.AnimalBehavior animal))
                    continue;

                if (!processed.Add(animal))
                    continue;

                animal.TakeDamage(context.BaseDamage, hitInfo.HitPoint);
            }
        }
    }
}


