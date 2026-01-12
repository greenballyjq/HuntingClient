using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 玩家数据系统事件键
/// </summary>
public static class PlayerDataEvents
{
    /// <summary>
    /// 3币数量改变事件
    /// </summary>
    public static readonly EventKey<ThreeKPCoinChangedEventArgs> ThreeKPCoinChanged = new EventKey<ThreeKPCoinChangedEventArgs>();
}

/// <summary>
/// 3币数量改变事件参数
/// </summary>
public sealed class ThreeKPCoinChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前3币数量
    /// </summary>
    public int CurrentAmount { get; set; }

    /// <summary>
    /// 变化量
    /// </summary>
    public int DeltaAmount { get; set; }
}

