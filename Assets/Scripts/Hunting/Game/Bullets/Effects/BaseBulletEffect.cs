using GameFramework.Core;
using Hunting.Events;
using UnityEditor.PackageManager;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace Hunting.Game.Bullets.Effects
{
    /// <summary>
    /// 基础命中效果
    /// </summary>
    public class BaseBulletEffect : IBulletEffect
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 命中处理
        /// </summary>
        public void OnHit(BulletEffectContext context, BulletHitInfo hitInfo)
        {
            // 对命中目标造成一次基础伤害
            hitInfo.PrimaryTarget.TakeDamage(context.BaseDamage, hitInfo.HitPoint);

            // 触发命中事件
            TriggerBulletHit(new BulletHitEventArgs
            {
                HitPoint = hitInfo.HitPoint,
                Target = hitInfo.PrimaryTarget,
                BulletData = context.BulletData
            });
        }

        /// <summary>
        /// 触发命中事件
        /// </summary>
        private void TriggerBulletHit(BulletHitEventArgs args)
        {
            Event.Trigger(BulletEvents.BulletHit,args);
        }

    }
}



