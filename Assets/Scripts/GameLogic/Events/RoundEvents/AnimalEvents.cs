using cfg;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using GameFramework.Game;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物系统事件键
/// </summary>
public static class AnimalEvents
{
    /// <summary>
    /// 动物生成事件
    /// </summary>
    public static readonly EventKey<AnimalSpawnedEventArgs> AnimalSpawned = new EventKey<AnimalSpawnedEventArgs>();

    /// <summary>
    /// 动物进入死亡事件
    /// </summary>
    public static readonly EventKey<AnimalDyingEventArgs> AnimalDying = new EventKey<AnimalDyingEventArgs>();

    /// <summary>
    /// 动物死亡事件
    /// </summary>
    public static readonly EventKey<AnimalDiedEventArgs> AnimalDied = new EventKey<AnimalDiedEventArgs>();

    /// <summary>
    /// 动物逃跑事件
    /// </summary>
    public static readonly EventKey<AnimalFledEventArgs> AnimalFled = new EventKey<AnimalFledEventArgs>();

    /// <summary>
    /// 动物掉落奖励事件
    /// </summary>
    public static readonly EventKey<AnimalDropRewardEventArgs> AnimalDropReward = new EventKey<AnimalDropRewardEventArgs>();
}

/// <summary>
/// 动物生成事件参数
/// </summary>
public sealed class AnimalSpawnedEventArgs : EventArgs
{
    /// <summary>
    /// 触发派发的派发器
    /// </summary>
    public Spawner Spawner { get; set; }

    /// <summary>
    /// 生成的动物实例
    /// </summary>
    public AnimalBehavior Animal { get; set; }

    /// <summary>
    /// 物种配置
    /// </summary>
    public Specie SpecieData { get; set; }

    /// <summary>
    /// 生成位置
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// 初始方向
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    /// 驻场时间
    /// </summary>
    public float StayTime { get; set; }
}

/// <summary>
/// 动物进入死亡事件参数
/// </summary>
public sealed class AnimalDyingEventArgs : EventArgs
{
    /// <summary>
    /// 进入死亡的动物实例
    /// </summary>
    public AnimalBehavior Animal { get; set; }

    /// <summary>
    /// 物种配置
    /// </summary>
    public Specie SpecieData { get; set; }
}

/// <summary>
/// 动物死亡事件参数（死亡动画结束后）
/// </summary>
public sealed class AnimalDiedEventArgs : EventArgs
{
    /// <summary>
    /// 死亡的动物实例
    /// </summary>
    public AnimalBehavior Animal { get; set; }

    /// <summary>
    /// 物种配置
    /// </summary>
    public Specie SpecieData { get; set; }
}

/// <summary>
/// 动物逃跑事件参数
/// </summary>
public sealed class AnimalFledEventArgs : EventArgs
{
    /// <summary>
    /// 逃跑的动物实例
    /// </summary>
    public AnimalBehavior Animal { get; set; }

    /// <summary>
    /// 物种配置
    /// </summary>
    public Specie SpecieData { get; set; }
}

/// <summary>
/// 动物掉落奖励事件参数
/// </summary>
public sealed class AnimalDropRewardEventArgs : EventArgs
{
    /// <summary>
    /// 掉落奖励的动物实例
    /// </summary>
    public AnimalBehavior Animal { get; set; }

    /// <summary>
    /// 掉落奖励
    /// </summary>
    public Dictionary<EDropType, int> DropRewards { get; set; }
}


