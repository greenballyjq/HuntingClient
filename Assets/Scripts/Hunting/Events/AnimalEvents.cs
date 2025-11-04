using cfg;
using cfg.HuntingConfig;
using GameFramework.Core;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 动物系统事件键
/// </summary>
public static class AnimalEvents
{
    /// <summary>
    /// 当动物生成完成时触发
    /// </summary>
    public static readonly EventKey<AnimalSpawnedEventArgs> AnimalSpawned = new EventKey<AnimalSpawnedEventArgs>();

    /// <summary>
    /// 当动物死亡时触发
    /// </summary>
    public static readonly EventKey<AnimalDiedEventArgs> AnimalDied = new EventKey<AnimalDiedEventArgs>();

    /// <summary>
    /// 当动物逃跑离场时触发
    /// </summary>
    public static readonly EventKey<AnimalFledEventArgs> AnimalFled = new EventKey<AnimalFledEventArgs>();

    /// <summary>
    /// 当动物掉落奖励时触发
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
    public SpecieSpawner Spawner { get; set; }

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
/// 动物死亡事件参数
/// </summary>
public sealed class AnimalDiedEventArgs : EventArgs
{
    public AnimalBehavior Animal { get; set; }
    public Specie SpecieData { get; set; }
    public EDropType DropType { get; set; }
    public int DropAmount { get; set; }
}

/// <summary>
/// 动物逃跑事件参数
/// </summary>
public sealed class AnimalFledEventArgs : EventArgs
{
    public AnimalBehavior Animal { get; set; }
    public Specie SpecieData { get; set; }
}

/// <summary>
/// 动物掉落奖励事件参数
/// </summary>
public sealed class AnimalDropRewardEventArgs : EventArgs
{
    public AnimalBehavior Animal { get; set; }
    public EDropType DropType { get; set; }
    public int Amount { get; set; }
}


