using GameFramework.Core;

/// <summary>
/// 游戏应用流程事件
/// </summary>
public static class GameAppFlowEvents
{
    /// <summary>
    /// 游戏应用启动事件
    /// </summary>
    public static readonly EventKey AppStarted = new EventKey();

    /// <summary>
    /// 游戏应用退出事件
    /// </summary>
    public static readonly EventKey AppExited = new EventKey();
}
