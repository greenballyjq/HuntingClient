/// <summary>
/// 单局管理器基础接口
/// </summary>
public interface IRoundManager
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="context">单局上下文</param>
    void Init(RoundContext context);

    /// <summary>
    /// 释放
    /// </summary>
    void Dispose();
}

/// <summary>
/// 可更新的单局管理器
/// </summary>
public interface IRoundUpdatable
{
    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    void DoUpdate(float dt);
}

/// <summary>
/// 可暂停的单局管理器
/// </summary>
public interface IRoundPausable
{
    /// <summary>
    /// 暂停
    /// </summary>
    void Pause();

    /// <summary>
    /// 恢复
    /// </summary>
    void Resume();
}
