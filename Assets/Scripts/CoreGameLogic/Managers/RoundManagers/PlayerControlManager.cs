using System;
using System.Collections.Generic;
using cfg.HuntingConfig;
using GameFramework.Utility;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 玩家控制管理器
/// </summary>
public class PlayerControlManager : IMapWorld, IRoundUpdatable, IRoundPausable
{
    /// <summary>
    /// 当前玩家控制处理器
    /// </summary>
    private BaseControlHandler _currentPlayerControlHandler;

    /// <summary>
    /// 控制处理器缓存
    /// </summary>
    private readonly Dictionary<EControlType, BaseControlHandler> _controlHandlers = new Dictionary<EControlType, BaseControlHandler>();
    private bool _paused;

    public UniTask InitAsync(RoundContext context)
    {
        CacheHandlers();
        Log.Info("[PlayerControlManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void DoUpdate(float deltaTime)
    {
        if (_paused)
            return;

        if (_currentPlayerControlHandler != null && 
            _currentPlayerControlHandler.ControlPhase == ControlHandlerPhase.Running)
        {
            _currentPlayerControlHandler.DoUpdate(deltaTime);
        }
    }

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }

    public void Dispose()
    {
        EndCurrentControl();
        Log.Info("[PlayerControlManager] 已释放");
    }

    public void Unbind()
    {
        EndCurrentControl();
    }

    public void Bind(Map mapData)
    {
        SwitchToJoystick();
    }

    #region 公共方法
    /// <summary>
    /// 切换到默认射击模式
    /// </summary>
    public void SwitchToDefaultShooting()
    {
        SwitchHandler(_controlHandlers[EControlType.DefaultShooting]);
    }

    /// <summary>
    /// 切换到摇杆控制模式
    /// </summary>
    public void SwitchToJoystick()
    {
        SwitchHandler(_controlHandlers[EControlType.Joystick]);
    }

    /// <summary>
    /// 切换到指哪打哪模式
    /// </summary>
    public void SwitchToAimAssist()
    {
        SwitchHandler(_controlHandlers[EControlType.AimAssist]);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 缓存处理器
    /// </summary>
    private void CacheHandlers()
    {
        foreach (EControlType controlType in Enum.GetValues(typeof(EControlType)))
        {
            BaseControlHandler handler = ControlHandlerFactory.CreatePlayerControlHandler(controlType);
            _controlHandlers[controlType] = handler;
        }
    }

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
