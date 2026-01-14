/// <summary>
/// 玩家控制处理器接口
/// </summary>
public interface IPlayerControlHandler
{
    /// <summary>
    /// 控制逻辑开始
    /// </summary>
    void OnControlStart();

    /// <summary>
    /// 控制逻辑更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    void OnControlUpdate(float dt);

    /// <summary>
    /// 控制逻辑结束
    /// </summary>
    void OnControlEnd();
}