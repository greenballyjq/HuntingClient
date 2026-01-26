using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游玩界面主面板
/// </summary>
public class UIGameplay : UIBase
{
    /// <summary>
    /// 返回/结算按钮组件
    /// </summary>
    [SerializeField] private UIComponentButtonReturnOrSettlement _uiComponentReturnOrSettlement;

    /// <summary>
    /// 肉度条组件
    /// </summary>
    [SerializeField] private UIComponentMeatProgress _uiComponentMeatProgress;

    /// <summary>
    /// 能量条组件
    /// </summary>
    [SerializeField] private UIComponentSkill _uiComponentSkill;

    /// <summary>
    /// 子弹状态组件
    /// </summary>
    [SerializeField] private UIComponentBullet _uiComponentBulletStatus;

    /// <summary>
    /// 道具组组件
    /// </summary>
    [SerializeField] private UIComponentPropGroup _uiComponentPropGroup;
    
    /// <summary>
    /// 动态任务组件
    /// </summary>
    //[SerializeField] private UIComponentQuest uiComponentQuest;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _uiComponentMeatProgress.Init();
        _uiComponentSkill.Init();
        _uiComponentBulletStatus.Init();
        _uiComponentPropGroup.Init();
        _uiComponentReturnOrSettlement.Init();
        //uiComponentQuest.Init();
    }

    public override void OnClose()
    {
        _uiComponentPropGroup.CleanUp();
        _uiComponentBulletStatus.CleanUp();
        _uiComponentSkill.CleanUp();
        _uiComponentMeatProgress.CleanUp();
        _uiComponentReturnOrSettlement.CleanUp();
        //uiComponentQuest.CleanUp();
        base.OnClose();
    }
}
