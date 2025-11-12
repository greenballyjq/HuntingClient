    using GameFramework.Core;

/// <summary>
/// 丰收能量条系统事件键
/// </summary>
public static class EnergyEvents
{
    /// <summary>
    /// 能量值变化事件
    /// </summary>
    public static readonly EventKey<EnergyProgressChangedEventArgs> EnergyProgressChanged = new EventKey<EnergyProgressChangedEventArgs>();

    /// <summary>
    /// 能量条变化事件
    /// </summary>
    public static readonly EventKey<EnergyBarCountChangedEventArgs> EnergyBarCountChanged = new EventKey<EnergyBarCountChangedEventArgs>();

    /// <summary>
    /// 能量条达到上限事件
    /// </summary>
    public static readonly EventKey EnergyMaxBarsReached = new EventKey();

    /// <summary>
    /// 能量条被消耗事件
    /// </summary>
    public static readonly EventKey<EnergyConsumedEventArgs> EnergyConsumed = new EventKey<EnergyConsumedEventArgs>();
}

/// <summary>
/// 能量值变化事件参数
/// </summary>
public sealed class EnergyProgressChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前能量值
    /// </summary>
    public float CurrentEnergy { get; set; }

    /// <summary>
    /// 当前能量条数
    /// </summary>
    public int CurrentBars { get; set; }
}

/// <summary>
/// 能量条数量变化事件参数
/// </summary>
public sealed class EnergyBarCountChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前能量条数
    /// </summary>
    public int CurrentBars { get; set; }
}

/// <summary>
/// 能量条被消耗事件参数
/// </summary>
public sealed class EnergyConsumedEventArgs : EventArgs
{
    /// <summary>
    /// 剩余能量条数
    /// </summary>
    public int RemainingBars { get; set; }
}

