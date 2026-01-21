using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using GameFramework.Core;
using GameFramework.Game;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 道具系统事件键
/// </summary>
public static class PropEvents
{
    /// <summary>
    /// 道具效果开始事件
    /// </summary>
    public static readonly EventKey<PropStartedEventArgs> PropStarted = new EventKey<PropStartedEventArgs>();

    /// <summary>
    /// 道具效果更新事件
    /// </summary>
    public static readonly EventKey<PropUpdatedEventArgs> PropUpdated = new EventKey<PropUpdatedEventArgs>();

    /// <summary>
    /// 道具效果结束事件
    /// </summary>
    public static readonly EventKey<PropEndedEventArgs> PropEnded = new EventKey<PropEndedEventArgs>();

    /// <summary>
    /// 道具使用失败事件
    /// </summary>
    public static readonly EventKey<PropUseFailedEventArgs> PropUseFailed = new EventKey<PropUseFailedEventArgs>();

    /// <summary>
    /// 道具使用成功事件
    /// </summary>
    public static readonly EventKey<PropUseSucceededEventArgs> PropUseSucceeded = new EventKey<PropUseSucceededEventArgs>();

    /// <summary>
    /// 陷阱触发事件
    /// </summary>
    public static readonly EventKey<TrapTriggeredEventArgs> TrapTriggered = new EventKey<TrapTriggeredEventArgs>();
}

/// <summary>
/// 道具效果开始事件参数
/// </summary>
public sealed class PropStartedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 道具效果更新事件参数
/// </summary>
public sealed class PropUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }

    /// <summary>
    /// 剩余时间（秒）
    /// </summary>
    public float RemainingTime { get; set; }
}

/// <summary>
/// 道具效果结束事件参数
/// </summary>
public sealed class PropEndedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 道具使用失败事件参数
/// </summary>
public sealed class PropUseFailedEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 道具使用成功事件参数
/// </summary>
public sealed class PropUseSucceededEventArgs : EventArgs
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 陷阱触发事件参数
/// </summary>
public sealed class TrapTriggeredEventArgs : EventArgs
{
    /// <summary>
    /// 触发的陷阱实例
    /// </summary>
    public TrapBehavior Trap { get; set; }

    /// <summary>
    /// 触发陷阱的动物实例
    /// </summary>
    public AnimalBehavior TriggeredAnimal { get; set; }

    /// <summary>
    /// 触发位置
    /// </summary>
    public Vector3 TriggerPosition { get; set; }
}

