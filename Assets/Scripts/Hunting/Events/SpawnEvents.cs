using cfg.HuntingConfig;
using GameFramework.Core;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 派发系统事件键
/// </summary>
public static class SpawnEvents
{
    /// <summary>
    /// 派发管理器完成初始化时触发
    /// </summary>
    public static readonly EventKey<SpawnManagerReadyEventArgs> SpawnManagerReady = new EventKey<SpawnManagerReadyEventArgs>();

    /// <summary>
    /// 管理器调整派发器启用状态时触发
    /// </summary>
    public static readonly EventKey<SpawnerActiveChangedEventArgs> SpawnerActiveChanged = new EventKey<SpawnerActiveChangedEventArgs>();

    /// <summary>
    /// 当派发器发起一次派发时触发
    /// </summary>
    public static readonly EventKey<SpeciesSpawnEventArgs> SpeciesSpawned = new EventKey<SpeciesSpawnEventArgs>();
}

/// <summary>
/// 物种派发事件参数
/// </summary>
public sealed class SpeciesSpawnEventArgs : EventArgs
{
    /// <summary>
    /// 触发派发的派发器
    /// </summary>
    public SpeciesSpawner Spawner { get; set; }

    /// <summary>
    /// 物种配置数据
    /// </summary>
    public Specie SpecieData { get; set; }

    /// <summary>
    /// 派发位置
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// 派发方向
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    /// 驻场时间
    /// </summary>
    public float StayTime { get; set; }

    /// <summary>
    /// 派发时间戳
    /// </summary>
    public float SpawnTime { get; set; }
}

/// <summary>
/// 派发管理器准备完成事件参数
/// </summary>
public sealed class SpawnManagerReadyEventArgs : EventArgs
{
    /// <summary>
    /// 派发器数量
    /// </summary>
    public int SpawnerCount { get; set; }
}

/// <summary>
/// 派发器启用状态变化事件参数
/// </summary>
public sealed class SpawnerActiveChangedEventArgs : EventArgs
{
    /// <summary>
    /// 目标派发器
    /// </summary>
    public SpeciesSpawner Spawner { get; set; }

    /// <summary>
    /// 当前启用状态
    /// </summary>
    public bool IsActive { get; set; }
}