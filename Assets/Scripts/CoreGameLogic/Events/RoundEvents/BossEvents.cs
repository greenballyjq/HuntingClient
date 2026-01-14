using GameFramework.Core;


/// <summary>
/// Boss系统事件
/// </summary>
public static class BossEvents
{
    /// <summary>
    /// Boss 死亡事件
    /// </summary>
    public static readonly EventKey<BossDiedEventArgs> BossDied = new EventKey<BossDiedEventArgs>();
    
    /// <summary>
    /// Boss 死亡中事件
    /// </summary>
    public static readonly EventKey<BossDyingEventArgs> BossDying = new EventKey<BossDyingEventArgs>();
    
    /// <summary>
    /// Boss 召唤小怪事件
    /// </summary>
    public static readonly EventKey<BossCallEventArgs> BossCall = new EventKey<BossCallEventArgs>();
}

/// <summary>
/// Boss 死亡事件参数
/// </summary>
public sealed class BossDiedEventArgs : EventArgs
{
    /// <summary>
    /// 死亡的Boss实例
    /// </summary>
    public BossBehaviour Boss { get; set; }
}

/// <summary>
/// Boss 死亡事件参数
/// </summary>
public sealed class BossDyingEventArgs : EventArgs
{
    /// <summary>
    /// 死亡的Boss实例
    /// </summary>
    public BossBehaviour Boss { get; set; }
}

/// <summary>
/// Boss 召唤小怪事件
/// </summary>
public sealed class BossCallEventArgs : EventArgs
{
    /// <summary>
    /// Boss实例
    /// </summary>
    public BossBehaviour Boss { get; set; }
    
    /// <summary>
    /// 召唤小怪的保护时长
    /// </summary>
    public float GuardDuration { get; set; }
}