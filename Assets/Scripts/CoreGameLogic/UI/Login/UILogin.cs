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

    private async void OnEnable()
    {
        _uiComponentLoginLoading.Init();
        _uiComponentLoginButton.Init();

        _uiComponentLoginLoading.gameObject.SetActive(true);
        _uiComponentLoginButton.gameObject.SetActive(false);

        await GameFrameLauncher.Instance.WaitForInitializationAsync();

        GameServiceLocator.EventManager.AddListener("GameAppStarted", OnGameAppStarted);
    }

    private void OnDisable()
    {
        GameServiceLocator.EventManager.RemoveListener("GameAppStarted", OnGameAppStarted);

        _uiComponentLoginLoading.CleanUp();
        _uiComponentLoginButton.CleanUp();
    }

    /// <summary>
    /// 游戏应用启动回调
    /// </summary>
    private void OnGameAppStarted()
    {
        _uiComponentLoginLoading.gameObject.SetActive(false);
        _uiComponentLoginButton.gameObject.SetActive(true);
    }
}
