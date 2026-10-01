using Cysharp.Threading.Tasks;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主图打完后的选择面板：领取回准备页，或进入隐藏关卡。
/// </summary>
[UIForm(UILayer.Popup)]
public class UIPopupEnterHiddenMap : UIPopupNormalSettlement
{
    /// <summary>
    /// 进入隐藏关卡按钮
    /// </summary>
    [SerializeField] private Button _buttonEnterHidden;

    protected override void BindButtons()
    {
        base.BindButtons();
        _buttonEnterHidden.onClick.AddListener(OnEnterHiddenMapClicked);
    }

    protected override void UnbindButtons()
    {
        base.UnbindButtons();
        _buttonEnterHidden.onClick.RemoveListener(OnEnterHiddenMapClicked);
    }

    protected override void SetButtonsInteractable(bool interactable)
    {
        base.SetButtonsInteractable(interactable);
        _buttonEnterHidden.interactable = interactable;
    }

    /// <summary>
    /// 进入隐藏关卡
    /// </summary>
    private void OnEnterHiddenMapClicked()
    {
        SetButtonsInteractable(false);
        HuntingAppFlow.Instance.EnterHiddenMapAsync().Forget();
    }
}
