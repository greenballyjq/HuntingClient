/// <summary>
/// 控制处理器阶段
/// </summary>
public enum ControlHandlerPhase
{
    /// <summary>
    /// 无阶段
    /// </summary>
    None,

    /// <summary>
    /// 准备阶段
    /// </summary>
    Preparing,

    /// <summary>
    /// 运行阶段
    /// </summary>
    Running
}

/// <summary>
/// 控制处理器接口
/// </summary>
public interface IControlHandler
{
    /// <summary>
    /// 初始化
    /// </summary>
    void Init();

    /// <summary>
    /// 开始控制
    /// </summary>
    void StartControl();

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    void DoUpdate(float dt);

    /// <summary>
    /// 结束控制
    /// </summary>
    void EndControl();
}
