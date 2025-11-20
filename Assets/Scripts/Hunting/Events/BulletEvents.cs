
using cfg.HuntingConfig;
using GameFramework.Core;
using Hunting.Game.Animal;
using Hunting.Game.Bullets;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Events
{
    /// <summary>
    /// 子弹系统事件键
    /// </summary>
    public static class BulletEvents
    {
        /// <summary>
        /// 子弹生成事件
        /// </summary>
        public static readonly EventKey<BulletSpawnedEventArgs> BulletSpawned = new EventKey<BulletSpawnedEventArgs>();

        /// <summary>
        /// 子弹命中事件
        /// </summary>
        public static readonly EventKey<BulletHitEventArgs> BulletHit = new EventKey<BulletHitEventArgs>();

        /// <summary>
        /// 子弹销毁事件
        /// </summary>
        public static readonly EventKey<BulletDestroyedEventArgs> BulletDestroyed = new EventKey<BulletDestroyedEventArgs>();

        /// <summary>
        /// 子弹切换事件
        /// </summary>
        public static readonly EventKey<BulletChangedEventArgs> BulletChanged = new EventKey<BulletChangedEventArgs>();

        /// <summary>
        /// 特殊子弹倒计时事件
        /// </summary>
        public static readonly EventKey<SpecialBulletCountdownEventArgs> SpecialBulletCountdown = new EventKey<SpecialBulletCountdownEventArgs>();

        /// <summary>
        /// 特殊子弹效果结束事件
        /// </summary>
        public static readonly EventKey<SpecialBulletEffectEndedEventArgs> SpecialBulletEffectEnded = new EventKey<SpecialBulletEffectEndedEventArgs>();
    }

    /// <summary>
    /// 子弹生成事件参数
    /// </summary>
    public class BulletSpawnedEventArgs : EventArgs
    {
        /// <summary>
        /// 生成的子弹实例
        /// </summary>
        public BulletBehavior Bullet { get; set; }

        /// <summary>
        /// 子弹配置
        /// </summary>
        public Bullet BulletData { get; set; }

        /// <summary>
        /// 子弹生成位置
        /// </summary>
        public Vector3 SpawnPosition { get; set; }

        /// <summary>
        /// 子弹移动方向
        /// </summary>
        public Vector3 Direction { get; set; }
    }

    /// <summary>
    /// 子弹命中事件参数
    /// </summary>
    public class BulletHitEventArgs : EventArgs
    {
        /// <summary>
        /// 命中的子弹实例
        /// </summary>
        public BulletBehavior Bullet { get; set; }

        /// <summary>
        /// 子弹配置
        /// </summary>
        public Bullet BulletData { get; set; }

        /// <summary>
        /// 命中位置
        /// </summary>
        public Vector3 HitPoint { get; set; }

        /// <summary>
        /// 命中的所有动物实例列表
        /// </summary>
        public List<AnimalBehavior> Targets { get; set; }
    }

    /// <summary>
    /// 子弹销毁事件参数
    /// </summary>
    public class BulletDestroyedEventArgs : EventArgs
    {
        /// <summary>
        /// 销毁的子弹实例
        /// </summary>
        public BulletBehavior Bullet { get; set; }

        /// <summary>
        /// 子弹配置
        /// </summary>
        public Bullet BulletData { get; set; }
    }

    /// <summary>
    /// 子弹切换事件参数
    /// </summary>
    public sealed class BulletChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 旧子弹配置
        /// </summary>
        public Bullet OldBulletData { get; set; }

        /// <summary>
        /// 新子弹配置
        /// </summary>
        public Bullet NewBulletData { get; set; }

        /// <summary>
        /// 是否是新特殊子弹
        /// </summary>
        public bool IsSpecialBullet { get; set; }

        /// <summary>
        /// 剩余时间（仅特殊子弹有效）
        /// </summary>
        public float RemainingTime { get; set; }
    }

    /// <summary>
    /// 特殊子弹倒计时事件参数
    /// </summary>
    public sealed class SpecialBulletCountdownEventArgs : EventArgs
    {
        /// <summary>
        /// 当前子弹配置
        /// </summary>
        public Bullet BulletData { get; set; }

        /// <summary>
        /// 剩余时间
        /// </summary>
        public float RemainingTime { get; set; }
    }

    /// <summary>
    /// 特殊子弹效果结束事件参数
    /// </summary>
    public sealed class SpecialBulletEffectEndedEventArgs : EventArgs
    {
        /// <summary>
        /// 结束的特殊子弹配置
        /// </summary>
        public Bullet BulletData { get; set; }
    }
}

