using GameFramework.Core;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 启动占位遮罩。UIManager 还不能开 UITransition 时用同一套加载文案；正式过场打开后即退出。
/// </summary>
public class UIBootCover : MonoBehaviour
{
    private static UIBootCover _instance;

    [SerializeField] private UIComponentLoadingHint _loadingHint;

    private EventManager _eventManager;

    private void Awake()
    {
        _instance = this;
        _loadingHint.Init();
    }

    private async void Start()
    {
        await GameFrameworkManager.Instance.WaitForInitializationAsync();
        _eventManager = GameFrameworkManager.Instance.GetManager<EventManager>();
        _eventManager.AddListener("GameAppStarted", Dismiss);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        if (_eventManager == null)
            return;

        _eventManager.RemoveListener("GameAppStarted", Dismiss);
    }

    public static void Dismiss()
    {
        if (_instance != null)
            _instance.Hide();
    }

    private void Hide()
    {
        _loadingHint?.CleanUp();
        gameObject.SetActive(false);
    }
}
