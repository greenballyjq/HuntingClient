using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 肉度条系统事件键
/// </summary>
public static class MeatEvents
{
    /// <summary>
    /// 肉度条进度变化事件
    /// </summary>
    public static readonly EventKey<MeatProgressChangedEventArgs> MeatProgressChanged = new EventKey<MeatProgressChangedEventArgs>();

    /// <summary>
    /// 肉度条数量变化事件
    /// </summary>
    public static readonly EventKey<MeatBarCountChangedEventArgs> MeatBarCountChanged = new EventKey<MeatBarCountChangedEventArgs>();

    /// <summary>
    /// 肉度条达到上限事件
    /// </summary>
    public static readonly EventKey MeatMaxBarsReached = new EventKey();
}

/// <summary>
/// 肉度条进度变化事件参数
/// </summary>
public sealed class MeatProgressChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前肉度值
    /// </summary>
    public float CurrentMeat { get; set; }

    /// <summary>
    /// 当前肉度条条数
    /// </summary>
    public int CurrentBars { get; set; }
}

/// <summary>
/// 肉度条数量变化事件参数
/// </summary>
public sealed class MeatBarCountChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前肉度条条数
    /// </summary>
    public int CurrentBars { get; set; }
}
