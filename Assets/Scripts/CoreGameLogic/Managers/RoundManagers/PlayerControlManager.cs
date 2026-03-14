using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// 玩家控制管理器
/// </summary>
public class PlayerControlManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 当前玩家控制处理器
    /// </summary>
    private BaseControlHandler _currentPlayerControlHandler;

    public void Init(RoundContext context)
    {
        SwitchToJoystick();
        Log.Info("[PlayerControlManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        if (_currentPlayerControlHandler != null && 
            _currentPlayerControlHandler.ControlPhase == ControlHandlerPhase.Running)
        {
            _currentPlayerControlHandler.DoUpdate(deltaTime);
        }
    }

    public void Dispose()
    {
        EndCurrentControl();
        Log.Info("[PlayerControlManager] 已释放");
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
        BaseControlHandler handler = ControlHandlerFactory.CreatePlayerControlHandler(EControlType.DefaultShooting);
        SwitchHandler(handler);
    }

    /// <summary>
    /// 切换到摇杆控制模式
    /// </summary>
    public void SwitchToJoystick()
    {
        BaseControlHandler handler = ControlHandlerFactory.CreatePlayerControlHandler(EControlType.Joystick);
        SwitchHandler(handler);
    }

    /// <summary>
    /// 切换到指哪打哪模式
    /// </summary>
    public void SwitchToAimAssist()
    {
        BaseControlHandler handler = ControlHandlerFactory.CreatePlayerControlHandler(EControlType.AimAssist);
        SwitchHandler(handler);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 切换控制处理器
    /// </summary>
    private void SwitchHandler(BaseControlHandler newHandler)
    {
        EndCurrentControl();

        _currentPlayerControlHandler = newHandler;
        _currentPlayerControlHandler.Init();
        _currentPlayerControlHandler.StartControl();
    }

    private void EndCurrentControl()
    {
        _currentPlayerControlHandler?.EndControl();
        _currentPlayerControlHandler = null;
    }
    #endregion
}
