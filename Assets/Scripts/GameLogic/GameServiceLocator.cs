using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Game;
using GameFramework.Manager;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏服务定位器
/// </summary>
public static class GameServiceLocator
{
    #region 常用管理器
    /// <summary>
    /// 事件管理器
    /// </summary>
    public static EventManager EventManager => GameFrameworkManager.Instance.GetManager<EventManager>();

    /// <summary>
    /// 游戏场景管理器
    /// </summary>
    public static GameSceneManager SceneManager => GameFrameworkManager.Instance.GetManager<GameSceneManager>();

    /// <summary>
    /// UI管理器
    /// </summary>
    public static UIManager UIManager => GameFrameworkManager.Instance.GetManager<UIManager>();

    /// <summary>
    /// 资源加载管理器
    /// </summary>
    public static ResourceManager ResourceManager => GameFrameworkManager.Instance.GetManager<ResourceManager>();

    /// <summary>
    /// 对象池管理器
    /// </summary>
    public static GameObjectPoolManager GameObjectPoolManager => GameFrameworkManager.Instance.GetManager<GameObjectPoolManager>();

    /// <summary>
    /// 配置管理器
    /// </summary>
    public static HuntingConfigManager ConfigManager => HuntingConfigManager.Instance;
    #endregion

    /// <summary>
    /// 获取框架管理器
    /// </summary>
    public static T GetFrameworkManager<T>() where T : class, IManager
    {
        return GameFrameworkManager.Instance.GetManager<T>();
    }

    /// <summary>
    /// 获取应用级管理器
    /// </summary>
    public static T GetAppManager<T>() where T : class, IAppManager
    {
        return HuntingAppFlow.Instance.GetAppManager<T>();
    }

    /// <summary>
    /// 获取单局管理器
    /// </summary>
    public static T GetRoundManager<T>() where T : class, IRoundManager
    {
        return HuntingAppFlow.Instance.GetRoundFlow().GetRoundManager<T>();
    }

    /// <summary>
    /// 等待初始化完成
    /// </summary>
    public static async UniTask WaitForInitializationAsync()
    {
        await HuntingAppFlow.Instance.WaitForAppStartedAsync();
    }
}
