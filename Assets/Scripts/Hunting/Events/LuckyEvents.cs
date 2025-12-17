using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.Manager;

/// <summary>
/// 幸运仪式增益系统事件键
/// </summary>
public static class LuckyBuffEvents
{
    /// <summary>
    /// 幸运仪式增益已激活事件
    /// </summary>
    public static readonly EventKey<LuckyBuffActivatedEventArgs> LuckyBuffActivated = new EventKey<LuckyBuffActivatedEventArgs>();

    /// <summary>
    /// 礼包开启事件
    /// </summary>
    public static readonly EventKey<GiftOpenedEventArgs> GiftOpened = new EventKey<GiftOpenedEventArgs>();
}

/// <summary>
/// 幸运仪式增益已激活事件参数
/// </summary>
public sealed class LuckyBuffActivatedEventArgs : EventArgs
{
    /// <summary>
    /// 幸运仪式增益配置
    /// </summary>
    public LuckyBuff LuckyBuffData { get; set; }
}

/// <summary>
/// 礼包开启事件参数
/// </summary>
public sealed class GiftOpenedEventArgs : EventArgs
{
    /// <summary>
    /// 礼包类型
    /// </summary>
    public ELuckyGiftType GiftType { get; set; }

    /// <summary>
    /// 幸运仪式增益配置
    /// </summary>
    public LuckyBuff LuckyBuffData { get; set; }
}

