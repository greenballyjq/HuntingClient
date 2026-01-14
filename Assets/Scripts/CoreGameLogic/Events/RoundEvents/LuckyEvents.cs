using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 幸运仪式事件
/// </summary>
public static class LuckyEvents
{
    /// <summary>
    /// 幸运仪式增益激活事件
    /// </summary>
    public static readonly EventKey<LuckyBuffActivatedEventArgs> LuckyBuffActivated = new EventKey<LuckyBuffActivatedEventArgs>();

    /// <summary>
    /// 礼包开启事件
    /// </summary>
    public static readonly EventKey<GiftOpenedEventArgs> GiftOpened = new EventKey<GiftOpenedEventArgs>();

    /// <summary>
    /// 礼包开启动画开始事件
    /// </summary>
    public static readonly EventKey GiftOpenAnimationStarted = new EventKey();

    /// <summary>
    /// 礼包开启动画结束事件
    /// </summary>
    public static readonly EventKey<GiftOpenAnimationEndedEventArgs> GiftOpenAnimationEnded = new EventKey<GiftOpenAnimationEndedEventArgs>();

    /// <summary>
    /// 幸运增益确认点击事件
    /// </summary>
    public static readonly EventKey LuckyBuffConfirmClicked = new EventKey();
}

/// <summary>
/// 幸运仪式增益激活事件参数
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

/// <summary>
/// 礼包开启动画结束事件参数
/// </summary>
public sealed class GiftOpenAnimationEndedEventArgs : EventArgs
{
    /// <summary>
    /// 幸运仪式增益配置
    /// </summary>
    public LuckyBuff LuckyBuffData { get; set; }
}

