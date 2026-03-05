/// <summary>
/// 玩家控制处理器阶段
/// </summary>
public enum PlayerControlHandlerPhase
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
/// 玩家控制处理器接口
/// </summary>
public interface IPlayerControlHandler
{
    /// <summary>
    /// 开始控制
    /// </summary>
    void StartControl();

    /// <summary>
    /// 更新控制
    /// </summary>
    /// <param name="dt">时间增量</param>
    void UpdateControl(float dt);

    /// <summary>
    /// 结束控制
    /// </summary>
    void EndControl();
}

/// <summary>
/// 玩家控制处理器基类
/// </summary>
public abstract class BasePlayerControlHandler : IPlayerControlHandler
{
    /// <summary>
    /// 控制阶段
    /// </summary>
    public PlayerControlHandlerPhase ControlPhase { get; private set; }

    public void StartControl()
    {
        ControlPhase = PlayerControlHandlerPhase.Preparing;

        OnControlStart();

        ControlPhase = PlayerControlHandlerPhase.Running;
    }

    public void UpdateControl(float dt)
    {
        if (ControlPhase != PlayerControlHandlerPhase.Running)
            return;

        OnControlUpdate(dt);
    }

    public void EndControl()
    {
        OnControlEnd();

        ControlPhase = PlayerControlHandlerPhase.None;
    }

    /// <summary>
    /// 控制开始钩子
    /// </summary>
    protected abstract void OnControlStart();

    /// <summary>
    /// 控制更新钩子
    /// </summary>
    /// <param name="dt">时间增量</param>
    protected abstract void OnControlUpdate(float dt);

    /// <summary>
    /// 控制结束钩子
    /// </summary>
    protected abstract void OnControlEnd();
}