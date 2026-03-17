using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 返回按钮组件
/// </summary>
public class UIComponentReturnButton : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 返回按钮
    /// </summary>
    [SerializeField] private Button _buttonReturn;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = GameServiceLocator.UIManager;

        _buttonReturn.onClick.AddListener(OnReturnButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonReturn.onClick.RemoveListener(OnReturnButtonClicked);
    }

    public void Init(){}

    public void CleanUp() { }

    private async void OnReturnButtonClicked()
    {
        await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
    }
}
