using GameFramework.UI;
using UnityEngine;

/// <summary>
/// 道具UI组组件
/// </summary>
public class UIComponentPropGroup : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 炮火轰炸道具组件
    /// </summary>
    [SerializeField] private UIComponentPropItem _uiComponentPropBombardment;

    /// <summary>
    /// 指哪打哪道具组件
    /// </summary>
    [SerializeField] private UIComponentPropItem _uiComponentPropAimAssist;

    /// <summary>
    /// 智能诱捕陷阱道具组件
    /// </summary>
    [SerializeField] private UIComponentPropItem _uiComponentPropTrap;

    public void Init()
    {
        _uiComponentPropBombardment.Init();
        _uiComponentPropAimAssist.Init();
        _uiComponentPropTrap.Init();
    }

    public void CleanUp()
    {
        _uiComponentPropBombardment.CleanUp();
        _uiComponentPropAimAssist.CleanUp();
        _uiComponentPropTrap.CleanUp();
    }
}