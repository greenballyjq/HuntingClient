using cfg.HuntingConfig;
using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Game.Bullets.Effects
{
    /// <summary>
    /// 子弹效果接口
    /// </summary>
    public interface IBulletEffect
    {
        /// <summary>
        /// 命中处理
        /// </summary>
        /// <param name="context">子弹上下文</param>
        /// <param name="hitInfo">命中信息</param>
        void OnHit(BulletEffectContext context, BulletHitInfo hitInfo);
    }

    /// <summary>
    /// 子弹效果上下文
    /// </summary>
    public class BulletEffectContext
    {
        /// <summary>
        /// 子弹数据
        /// </summary>
        public Bullet BulletData { get; set; }

        /// <summary>
        /// 基础伤害
        /// </summary>
        public float BaseDamage { get; set; }

        /// <summary>
        /// 发射者
        /// </summary>
        public GameObject Owner { get; set; }
    }

    /// <summary>
    /// 子弹命中信息
    /// </summary>
    public class BulletHitInfo
    {
        /// <summary>
        /// 命中位置
        /// </summary>
        public Vector3 HitPoint { get; set; }

        /// <summary>
        /// 命中的动物
        /// </summary>
        public AnimalBehavior PrimaryTarget { get; set; }
    }
}


