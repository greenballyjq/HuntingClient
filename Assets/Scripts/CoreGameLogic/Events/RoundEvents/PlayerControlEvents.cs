using GameFramework.Core;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 玩家控制相关事件
/// </summary>
public static class PlayerControlEvents
{
    /// <summary>
    /// 目标选中事件
    /// </summary>
    public static readonly EventKey<TargetSelectedEventArgs> TargetSelected = new EventKey<TargetSelectedEventArgs>();

    /// <summary>
    /// 目标丢失事件
    /// </summary>
    public static readonly EventKey<TargetLostEventArgs> TargetLost = new EventKey<TargetLostEventArgs>();

    /// <summary>
    /// 目标切换事件
    /// </summary>
    public static readonly EventKey<TargetChangedEventArgs> TargetChanged = new EventKey<TargetChangedEventArgs>();
}

/// <summary>
/// 目标选中事件参数
/// </summary>
public sealed class TargetSelectedEventArgs : EventArgs
{
    /// <summary>
    /// 选中的目标
    /// </summary>
    public BaseAnimalBehaviour Target { get; set; }
}

/// <summary>
/// 目标丢失事件参数
/// </summary>
public sealed class TargetLostEventArgs : EventArgs
{
    /// <summary>
    /// 丢失的目标
    /// </summary>
    public BaseAnimalBehaviour LostTarget { get; set; }
}

/// <summary>
/// 目标切换事件参数
/// </summary>
public sealed class TargetChangedEventArgs : EventArgs
{
    /// <summary>
    /// 旧目标
    /// </summary>
    public BaseAnimalBehaviour OldTarget { get; set; }

    /// <summary>
    /// 新目标
    /// </summary>
    public BaseAnimalBehaviour NewTarget { get; set; }
}

