

using cfg.HuntingConfig;
using GameFramework.Core;
using Hunting.Game.Animal;
using Hunting.Game.Bullets;
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
        /// 命中的动物实例
        /// </summary>
        public AnimalBehavior Target { get; set; }
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
}

