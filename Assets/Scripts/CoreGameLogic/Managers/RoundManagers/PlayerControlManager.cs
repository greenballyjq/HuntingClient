using UnityEngine;

/// <summary>
/// 玩家控制管理器
/// </summary>
public class PlayerControlManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 当前玩家控制处理器
    /// </summary>
    private BasePlayerControlHandler _currentPlayerControlHandler;

    public void Init(RoundContext context)
    {
        Debug.Log("[PlayerControlManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        if (_currentPlayerControlHandler != null && 
            _currentPlayerControlHandler.ControlPhase == PlayerControlHandlerPhase.Running)
        {
            _currentPlayerControlHandler.UpdateControl(deltaTime);
        }
    }

    public void Dispose()
    {
        EndCurrentControl();
        Debug.Log("[PlayerControlManager] 已释放");
    }

    public void Cleanup()
    {
        EndCurrentControl();
    }

    public void ReInit(RoundContext context)
    {
        SwitchToJoystick();
    }

    #region 公共方法
    /// <summary>
    /// 切换到默认射击模式
    /// </summary>
    public void SwitchToDefaultShooting()
    {
        BasePlayerControlHandler handler = PlayerControlHandlerFactory.CreatePlayerControlHandler(EControlType.DefaultShooting);
        SwitchHandler(handler);
    }

    /// <summary>
    /// 切换到摇杆控制模式
    /// </summary>
    public void SwitchToJoystick()
    {
        BasePlayerControlHandler handler = PlayerControlHandlerFactory.CreatePlayerControlHandler(EControlType.Joystick);
        SwitchHandler(handler);
    }

    /// <summary>
    /// 切换到指哪打哪模式
    /// </summary>
    /// <param name="minLockDistance">最小锁定距离</param>
    /// <param name="maxLockDistance">最大锁定距离</param>
    public void SwitchToAimAssist(float minLockDistance, float maxLockDistance)
    {
        BasePlayerControlHandler handler = PlayerControlHandlerFactory.CreatePlayerControlHandler(EControlType.AimAssist);
            
        if (handler is AimAssistHandler aimAssistHandler)
        {
            aimAssistHandler.SetMinLockDistance(minLockDistance);
            aimAssistHandler.SetMaxLockDistance(maxLockDistance);
        }
            
        SwitchHandler(handler);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 切换控制处理器
    /// </summary>
    private void SwitchHandler(BasePlayerControlHandler newHandler)
    {
        EndCurrentControl();

        _currentPlayerControlHandler = newHandler;
        _currentPlayerControlHandler.StartControl();
    }

    private void EndCurrentControl()
    {
        _currentPlayerControlHandler?.EndControl();
        _currentPlayerControlHandler = null;
    }
    #endregion
}
