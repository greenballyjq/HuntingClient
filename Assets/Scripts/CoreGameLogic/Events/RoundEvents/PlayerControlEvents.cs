using GameFramework.Core;
using GameFramework.Game;
using UnityEngine;

/// <summary>
/// 目标丢失原因枚举
/// </summary>
public enum ETargetLostReason
{
    /// <summary>
    /// 距离超出
    /// </summary>
    DistanceExceeded,

    /// <summary>
    /// 目标死亡
    /// </summary>
    TargetDied,

    /// <summary>
    /// 控制结束
    /// </summary>
    ControlEnded
}

/// <summary>
/// 玩家控制系统事件键
/// </summary>
public static class PlayerControlEvents
{
    /// <summary>
    /// 目标已选中事件
    /// </summary>
    public static readonly EventKey<TargetSelectedEventArgs> TargetSelected = new EventKey<TargetSelectedEventArgs>();

    /// <summary>
    /// 目标已丢失事件
    /// </summary>
    public static readonly EventKey<TargetLostEventArgs> TargetLost = new EventKey<TargetLostEventArgs>();

    /// <summary>
    /// 目标已切换事件
    /// </summary>
    public static readonly EventKey<TargetChangedEventArgs> TargetChanged = new EventKey<TargetChangedEventArgs>();
}

/// <summary>
/// 目标已选中事件参数
/// </summary>
public sealed class TargetSelectedEventArgs : EventArgs
{
    /// <summary>
    /// 选中的目标
    /// </summary>
    public Transform Target { get; set; }

    /// <summary>
    /// 目标动物
    /// </summary>
    public AnimalBehavior Animal { get; set; }
}

/// <summary>
/// 目标已丢失事件参数
/// </summary>
public sealed class TargetLostEventArgs : EventArgs
{
    /// <summary>
    /// 丢失的目标
    /// </summary>
    public Transform LostTarget { get; set; }

    /// <summary>
    /// 丢失原因
    /// </summary>
    public ETargetLostReason Reason { get; set; }
}

/// <summary>
/// 目标已切换事件参数
/// </summary>
public sealed class TargetChangedEventArgs : EventArgs
{
    /// <summary>
    /// 旧目标
    /// </summary>
    public Transform OldTarget { get; set; }

    /// <summary>
    /// 新目标
    /// </summary>
    public Transform NewTarget { get; set; }
}

