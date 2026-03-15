using UnityEngine;

/// <summary>
/// 游玩界面组件显隐规则
/// </summary>
public interface IUIGameplayComponentVisibilityRule
{
    /// <summary>
    /// 是否显示时间组件
    /// </summary>
    bool ShowTime { get; }

    /// <summary>
    /// 是否显示肉度条组件
    /// </summary>
    bool ShowMeatBar { get; }

    /// <summary>
    /// 是否显示动物计数器组件
    /// </summary>
    bool ShowAnimalCounter { get; }

    /// <summary>
    /// 是否显示Boss血量组件
    /// </summary>
    bool ShowBossHealth { get; }

    /// <summary>
    /// 是否显示返回按钮组件
    /// </summary>
    bool ShowReturnButton { get; }

    /// <summary>
    /// 是否显示三千盘金币组件
    /// </summary>
    bool ShowThreeKPCoin { get; }

    /// <summary>
    /// 是否显示道具组组件
    /// </summary>
    bool ShowPropGroup { get; }

    /// <summary>
    /// 是否显示子弹组件
    /// </summary>
    bool ShowBullet { get; }

    /// <summary>
    /// 是否显示摇杆组件
    /// </summary>
    bool ShowJoystick { get; }

    /// <summary>
    /// 是否显示技能组件
    /// </summary>
    bool ShowSkill { get; }

    /// <summary>
    /// 是否显示掉落奖励光效组件
    /// </summary>
    bool ShowDropRewardLightEffect { get; }

    /// <summary>
    /// 是否显示提示组件
    /// </summary>
    bool ShowTip { get; }

    /// <summary>
    /// 是否使用隐藏地图背景
    /// </summary>
    bool UseHiddenMapBackground { get; }
}

/// <summary>
/// 主地图游玩界面组件显隐规则
/// </summary>
public class MainMapComponentVisibilityRule : IUIGameplayComponentVisibilityRule
{
    public bool ShowTime => true;
    public bool ShowMeatBar => true;
    public bool ShowAnimalCounter => true;
    public bool ShowBossHealth => false;
    public bool ShowReturnButton => true;
    public bool ShowThreeKPCoin => true;
    public bool ShowPropGroup => true;
    public bool ShowBullet => true;
    public bool ShowJoystick => true;
    public bool ShowSkill => true;
    public bool ShowDropRewardLightEffect => true;
    public bool ShowTip => true;
    public bool UseHiddenMapBackground => false;
}

/// <summary>
/// 隐藏地图游玩界面组件显隐规则
/// </summary>
public class HiddenMapComponentVisibilityRule : IUIGameplayComponentVisibilityRule
{
    public bool ShowTime => true;
    public bool ShowMeatBar => true;
    public bool ShowAnimalCounter => false;
    public bool ShowBossHealth => true;
    public bool ShowReturnButton => false;
    public bool ShowThreeKPCoin => true;
    public bool ShowPropGroup => true;
    public bool ShowBullet => true;
    public bool ShowJoystick => true;
    public bool ShowSkill => true;
    public bool ShowDropRewardLightEffect => true;
    public bool ShowTip => true;
    public bool UseHiddenMapBackground => true;
}
