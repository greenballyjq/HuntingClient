using cfg.HuntingConfig;
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

