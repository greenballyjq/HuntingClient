using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物相关事件
/// </summary>
public static class AnimalEvents
{
    /// <summary>
    /// 动物进入死亡事件
    /// </summary>
    public static readonly EventKey<AnimalEnteredDeathEventArgs> AnimalEnteredDeath = new EventKey<AnimalEnteredDeathEventArgs>();

    /// <summary>
    /// 动物死亡事件
    /// </summary>
    public static readonly EventKey<AnimalDiedEventArgs> AnimalDied = new EventKey<AnimalDiedEventArgs>();
    
    /// <summary>
    /// 动物移除事件（这是所有动物失活的最后一步，此时动物已被AnimalManager移除引用，但还未被销毁或归池）
    /// </summary>
    public static readonly EventKey<AnimalRemovedEventArgs> AnimalRemoved = new EventKey<AnimalRemovedEventArgs>();

    /// <summary>
    /// 动物逃跑事件
    /// </summary>
    public static readonly EventKey<AnimalFledEventArgs> AnimalFled = new EventKey<AnimalFledEventArgs>();

    /// <summary>
    /// 动物掉落奖励事件
    /// </summary>
    public static readonly EventKey<AnimalDropRewardEventArgs> AnimalDropReward = new EventKey<AnimalDropRewardEventArgs>();

    /// <summary>
    /// 动物生成事件
    /// </summary>
    public static readonly EventKey<AnimalGeneratedEventArgs> AnimalGenerated = new EventKey<AnimalGeneratedEventArgs>();

    /// <summary>
    /// 动物到达边界事件
    /// </summary>
    public static readonly EventKey<AnimalReachedWallEventArgs> AnimalReachedWall = new EventKey<AnimalReachedWallEventArgs>();

    /// <summary>
    /// Boss受伤事件
    /// </summary>
    public static readonly EventKey<BossDamagedEventArgs> BossDamaged = new EventKey<BossDamagedEventArgs>();
}

/// <summary>
/// 动物进入死亡事件参数
/// </summary>
public sealed class AnimalEnteredDeathEventArgs : EventArgs
{
    /// <summary>
    /// 进入死亡的动物实例
    /// </summary>
    public BaseAnimalBehaviour Animal { get; set; }

    /// <summary>
    /// 物种配置
    /// </summary>
    public Specie SpecieData { get; set; }
}

/// <summary>
/// 动物死亡事件参数
/// </summary>
public sealed class AnimalDiedEventArgs : EventArgs
{
    /// <summary>
    /// 死亡的动物实例
    /// </summary>
    public BaseAnimalBehaviour Animal { get; set; }

    /// <summary>
    /// 物种配置
    /// </summary>
    public Specie SpecieData { get; set; }
}

/// <summary>
/// 动物死亡事件参数
/// </summary>
public sealed class AnimalRemovedEventArgs : EventArgs
{
    /// <summary>
    /// 移除的动物实例
    /// </summary>
    public BaseAnimalBehaviour Animal { get; set; }
}

/// <summary>
/// 动物逃跑事件参数
/// </summary>
public sealed class AnimalFledEventArgs : EventArgs
{
    /// <summary>
    /// 逃跑的动物实例
    /// </summary>
    public BaseAnimalBehaviour Animal { get; set; }

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
    public BaseAnimalBehaviour Animal { get; set; }

    /// <summary>
    /// 掉落奖励
    /// </summary>
    public Dictionary<EDropType, int> DropRewards { get; set; }
}

/// <summary>
/// 动物生成事件参数
/// </summary>
public sealed class AnimalGeneratedEventArgs : EventArgs
{
    /// <summary>
    /// 生成的动物实例
    /// </summary>
    public BaseAnimalBehaviour Animal { get; set; }
}

/// <summary>
/// 动物到达边界事件参数
/// </summary>
public sealed class AnimalReachedWallEventArgs : EventArgs
{
    /// <summary>
    /// 到达边界的动物实例
    /// </summary>
    public BaseAnimalBehaviour Animal { get; set; }
}

/// <summary>
/// Boss受伤事件参数
/// </summary>
public sealed class BossDamagedEventArgs : EventArgs
{
    /// <summary>
    /// 最大血量
    /// </summary>
    public float MaxHealth { get; set; }

    /// <summary>
    /// 当前血量
    /// </summary>
    public float CurrentHealth { get; set; }
}
