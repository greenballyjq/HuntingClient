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

    private UIManager _uiManager;

    private void Awake()
    {
        RegisterServers();
        _buttonReturn.onClick.AddListener(OnReturnButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonReturn.onClick.RemoveListener(OnReturnButtonClicked);
    }

    public void Init(){}

    public void CleanUp() { }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _uiManager = GameServiceLocator.UIManager;
    }
    #endregion

    #region 事件相关
    private async void OnReturnButtonClicked()
    {
        await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
    }
    #endregion
}
