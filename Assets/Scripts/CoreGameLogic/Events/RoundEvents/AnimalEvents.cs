using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.Game.Animal;
using System.Collections.Generic;

/// <summary>
/// 动物相关事件
/// </summary>
public static class AnimalEvents
{
    /// <summary>
    /// 动物生成事件
    /// </summary>
    public static readonly EventKey<AnimalGeneratedEventArgs> AnimalGenerated = new EventKey<AnimalGeneratedEventArgs>();

    /// <summary>
    /// 动物进入死亡事件
    /// </summary>
    public static readonly EventKey<AnimalEnteredDeathEventArgs> AnimalEnteredDeath = new EventKey<AnimalEnteredDeathEventArgs>();

    /// <summary>
    /// 动物掉落奖励事件
    /// </summary>
    public static readonly EventKey<AnimalDropRewardEventArgs> AnimalDropReward = new EventKey<AnimalDropRewardEventArgs>();

    /// <summary>
    /// 掉落奖励生效事件
    /// </summary>
    public static readonly EventKey<DropRewardArrivedEventArgs> DropRewardArrived = new EventKey<DropRewardArrivedEventArgs>();

    /// <summary>
    /// 动物死亡事件
    /// </summary>
    public static readonly EventKey<AnimalDiedEventArgs> AnimalDied = new EventKey<AnimalDiedEventArgs>();
    
    /// <summary>
    /// 动物离场事件
    /// </summary>
    public static readonly EventKey<AnimalLeftEventArgs> AnimalLeft = new EventKey<AnimalLeftEventArgs>();

    /// <summary>
    /// Boss受伤事件
    /// </summary>
    public static readonly EventKey<BossDamagedEventArgs> BossDamaged = new EventKey<BossDamagedEventArgs>();
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
/// 掉落奖励生效事件参数
/// </summary>
public sealed class DropRewardArrivedEventArgs : EventArgs
{
    /// <summary>
    /// 掉落类型
    /// </summary>
    public EDropType DropType { get; set; }

    /// <summary>
    /// 掉落数量
    /// </summary>
    public int DropCount { get; set; }
}

/// <summary>
/// 动物离场事件参数
/// </summary>
public sealed class AnimalLeftEventArgs : EventArgs
{
    /// <summary>
    /// 离场的动物实例
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
