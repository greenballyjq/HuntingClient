using GameFramework.Core;

/// <summary>
/// 应用流程事件
/// </summary>
public static class AppFlowEvents
{
    /// <summary>
    /// 应用启动完成事件
    /// </summary>
    public static readonly EventKey AppStarted = new EventKey();

    /// <summary>
    /// 应用退出完成事件
    /// </summary>
    public static readonly EventKey AppExited = new EventKey();
}
