using GameFramework.Core;
using GameFramework.Core.UI;
using GameFramework.Game;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 登录界面
/// </summary>
public class UILogin : MonoBehaviour
{
    /// <summary>
    /// 登录按钮组件
    /// </summary>
    [SerializeField] private UIComponentLoginButton _uiComponentLoginButton;

    /// <summary>
    /// 登录加载组件
    /// </summary>
    [SerializeField] private UIComponentLoginLoading _uiComponentLoginLoading;

    private void Awake()
    {
        GameServiceLocator.EventManager.AddListener("GameAppStarted", OnGameAppStarted);

        _uiComponentLoginLoading.Init();
    }

    private void OnDestroy()
    {
        GameServiceLocator.EventManager.RemoveListener("GameAppStarted", OnGameAppStarted);
    }

    /// <summary>
    /// 游戏应用启动回调
    /// </summary>
    private void OnGameAppStarted()
    {
        _uiComponentLoginLoading.CleanUp();
        _uiComponentLoginLoading.gameObject.SetActive(false);

        _uiComponentLoginButton.Init();
        _uiComponentLoginButton.gameObject.SetActive(true);
    }
}
