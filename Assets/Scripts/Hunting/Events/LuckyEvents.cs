using cfg.HuntingConfig;
using GameFramework.Core;
using Hunting.Manager;

/// <summary>
/// 幸运仪式系统事件键
/// </summary>
public static class LuckyEvents
{
    /// <summary>
    /// 幸运仪式已激活事件
    /// </summary>
    public static readonly EventKey<LuckyActivatedEventArgs> LuckyActivated = new EventKey<LuckyActivatedEventArgs>();
}

/// <summary>
/// 幸运仪式已激活事件参数
/// </summary>
public sealed class LuckyActivatedEventArgs : EventArgs
{
    /// <summary>
    /// 幸运仪式配置
    /// </summary>
    public Lucky LuckyData { get; set; }
}

