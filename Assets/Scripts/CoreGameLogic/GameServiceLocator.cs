using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.Core;
using GameFramework.Game;
using GameFramework.Manager;
using GameFramework.UI;

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
    /// UI管理器
    /// </summary>
    public static UIManager UIManager => GameFrameworkManager.Instance.GetManager<UIManager>();

    /// <summary>
    /// 资源管理器
    /// </summary>
    public static ResourceManager ResourceManager => GameFrameworkManager.Instance.GetManager<ResourceManager>();

    /// <summary>
    /// 对象池管理器
    /// </summary>
    public static GameObjectPoolManager GameObjectPoolManager => GameFrameworkManager.Instance.GetManager<GameObjectPoolManager>();

    /// <summary>
    /// 特效管理器
    /// </summary>
    public static EffectManager EffectManager => GameFrameworkManager.Instance.GetManager<EffectManager>();

    /// <summary>
    /// 音频管理器
    /// </summary>
    public static AudioManager AudioManager => GameFrameworkManager.Instance.GetManager<AudioManager>();

    /// <summary>
    /// 配置管理器
    /// </summary>
    public static HuntingConfigManager ConfigManager => HuntingAppFlow.Instance.GetAppManager<HuntingConfigManager>();
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
        return HuntingAppFlow.Instance.RoundFlow.GetRoundManager<T>();
    }
}
