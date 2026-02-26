using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 玩家数据相关事件
/// </summary>
public static class PlayerDataEvents
{
    /// <summary>
    /// 三千盘金币数量改变事件
    /// </summary>
    public static readonly EventKey<ThreeKPCoinAmountChangedEventArgs> ThreeKPCoinAmountChanged = new EventKey<ThreeKPCoinAmountChangedEventArgs>();

    /// <summary>
    /// 道具数量改变事件
    /// </summary>
    public static readonly EventKey<PropCountChangedEventArgs> PropCountChanged = new EventKey<PropCountChangedEventArgs>();
}

/// <summary>
/// 三千盘金币数量改变事件参数
/// </summary>
public sealed class ThreeKPCoinAmountChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前三千盘金币数量
    /// </summary>
    public int CurrentAmount { get; set; }

    /// <summary>
    /// 变化量
    /// </summary>
    public int DeltaAmount { get; set; }
}

/// <summary>
/// 道具数量改变事件参数
/// </summary>
public sealed class PropCountChangedEventArgs : EventArgs
{
    /// <summary>
    /// 道具类型
    /// </summary>
    public EPropType PropType { get; set; }

    /// <summary>
    /// 当前数量
    /// </summary>
    public int CurrentAmount { get; set; }

    /// <summary>
    /// 变化量
    /// </summary>
    public int DeltaAmount { get; set; }
}

