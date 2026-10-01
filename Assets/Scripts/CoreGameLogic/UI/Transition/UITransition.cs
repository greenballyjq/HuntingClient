using GameFramework.UI;
using UnityEngine;

/// <summary>
/// 通用过场遮罩
/// </summary>
[UIForm(UILayer.Overlay, lifetime: UILifetime.Overlay)]
public class UITransition : UIForm
{
    /// <summary>
    /// 加载提示组件
    /// </summary>
    [SerializeField] private UIComponentLoadingHint _loadingHint;

    protected override void OnOpen()
    {
        _loadingHint?.Init();
        UIBootCover.Dismiss();
    }

    public void SetProgress(float progress)
    {
        _loadingHint?.SetProgress(progress);
    }

    protected override void OnClose()
    {
        _loadingHint?.CleanUp();
        base.OnClose();
    }
}
