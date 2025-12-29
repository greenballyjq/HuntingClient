using GameFramework.Core;

/// <summary>
/// 结算系统事件键
/// </summary>
public static class SettlementEvents
{
    /// <summary>
    /// 结算开始事件
    /// </summary>
    public static readonly EventKey SettlementStarted = new EventKey();

    /// <summary>
    /// 结算数据已计算事件
    /// </summary>
    public static readonly EventKey<SettlementCalculatedEventArgs> SettlementCalculated = new EventKey<SettlementCalculatedEventArgs>();

    /// <summary>
    /// 结算最终确认事件
    /// </summary>
    public static readonly EventKey<SettlementCompletedEventArgs> SettlementCompleted = new EventKey<SettlementCompletedEventArgs>();
}

/// <summary>
/// 结算数据已计算事件参数
/// </summary>
public sealed class SettlementCalculatedEventArgs : EventArgs
{
    /// <summary>
    /// 本局完成的肉度条数量
    /// </summary>
    public int CompletedMeatBars { get; set; }

    /// <summary>
    /// 肉度条基础金币
    /// </summary>
    public int BaseCoin { get; set; }

    /// <summary>
    /// 肉度条基础熟练度
    /// </summary>
    public int BaseMastery { get; set; }

    /// <summary>
    /// 金币物种掉落金币
    /// </summary>
    public int CoinFromSpecie { get; set; }

    /// <summary>
    /// 动态任务奖励金币
    /// </summary>
    public int CoinFromQuest { get; set; }

    /// <summary>
    /// 额外结算倍率
    /// </summary>
    public float ExtraMultiplier { get; set; }

    /// <summary>
    /// 是否已应用广告翻倍
    /// </summary>
    public bool IsDoubleApplied { get; set; }

    /// <summary>
    /// 结算后的总金币
    /// </summary>
    public int TotalCoin { get; set; }

    /// <summary>
    /// 结算后的熟练度
    /// </summary>
    public int TotalMastery { get; set; }
}

/// <summary>
/// 结算最终确认事件参数
/// </summary>
public sealed class SettlementCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 结算后的总金币
    /// </summary>
    public int TotalCoin { get; set; }

    /// <summary>
    /// 结算后的总熟练度
    /// </summary>
    public int TotalMastery { get; set; }
}

