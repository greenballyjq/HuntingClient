using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 肉度条相关事件
/// </summary>
public static class MeatEvents
{
    /// <summary>
    /// 肉度值变化事件
    /// </summary>
    public static readonly EventKey<MeatValueChangedEventArgs> MeatValueChanged = new EventKey<MeatValueChangedEventArgs>();
}

/// <summary>
/// 肉度值变化事件参数
/// </summary>
public sealed class MeatValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// 当前肉度值
    /// </summary>
    public float CurrentMeatValue { get; set; }
    
    /// <summary>
    /// 已完成的刻度数
    /// </summary>
    public int CompletedScaleCount { get; set; }

    /// <summary>
    /// 总进度比例（0-1之间）
    /// </summary>
    public float TotalProgressRatio { get; set; }
}

