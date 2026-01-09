using cfg.HuntingConfig;
using GameFramework.Game;
using UnityEngine;

/// <summary>
/// 派发系统事件键
/// </summary>
public static class SpawnEvents
{
    /// <summary>
    /// 物种派发事件
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
    public Spawner Spawner { get; set; }

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
}