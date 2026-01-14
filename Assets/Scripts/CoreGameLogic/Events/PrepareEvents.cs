using GameFramework.Core;
using GameFramework.Game;

/// <summary>
/// 准备界面事件键
/// </summary>
public static class PrepareEvents
{
    /// <summary>
    /// 角色选中事件
    /// </summary>
    public static readonly EventKey<RoleSelectedEventArgs> RoleSelected = new EventKey<RoleSelectedEventArgs>();

    /// <summary>
    /// 骰子动画开始事件
    /// </summary>
    public static readonly EventKey DiceAnimationStarted = new EventKey();

    /// <summary>
    /// 骰子动画结束事件
    /// </summary>
    public static readonly EventKey DiceAnimationEnded = new EventKey();

    /// <summary>
    /// 走格子动画开始事件
    /// </summary>
    public static readonly EventKey SlotAnimationStarted = new EventKey();

    /// <summary>
    /// 走格子动画结束事件
    /// </summary>
    public static readonly EventKey SlotAnimationEnded = new EventKey();
}

/// <summary>
/// 角色选中事件参数
/// </summary>
public sealed class RoleSelectedEventArgs : EventArgs
{
    /// <summary>
    /// 起始格子索引
    /// </summary>
    public int FromSlotIndex { get; set; }

    /// <summary>
    /// 目标格子索引
    /// </summary>
    public int TargetSlotIndex { get; set; }

    /// <summary>
    /// 被选中的角色ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 骰子点数
    /// </summary>
    public int DiceValue { get; set; }

    /// <summary>
    /// 角色格子列表
    /// </summary>
    public UIComponentRoleSlot[] RoleSlots { get; set; }
}
