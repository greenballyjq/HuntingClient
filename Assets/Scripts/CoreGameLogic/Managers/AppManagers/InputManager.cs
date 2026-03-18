using System;
using System.Collections.Generic;
using GameFramework.Game;
using GameFramework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 输入管理器
/// </summary>
public class InputManager : IAppManager, GameInputActions.IPlayerActions
{
    /// <summary>
    /// 输入系统
    /// </summary>
    private GameInputActions _input;

    /// <summary>
    /// 指针屏幕坐标
    /// </summary>
    public Vector2 PointerScreenPosition { get; private set; }

    /// <summary>
    /// Fire键按下时的起始屏幕坐标
    /// </summary>
    public Vector2 PointerPressStartPosition { get; private set; }

    /// <summary>
    /// Fire键是否按住
    /// </summary>
    public bool IsFireHeld { get; private set; }

    public void Init()
    {
        // 初始化输入系统
        _input = new GameInputActions();
        _input.Player.SetCallbacks(this);

        // 默认启用Fire Action，禁用SelectTarget Action
        _input.Player.Fire.Enable();
        _input.Player.SelectTarget.Disable();
        _input.Player.PointerPosition.Enable();

        Log.Info("[InputManager] 初始化完成");
    }

    public void Dispose()
    {
        _input.Disable();
        _input.Dispose();
        Log.Info("[InputManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 切换到持续射击模式
    /// </summary>
    public void SwitchToFireMode()
    {
        _input.Player.Fire.Enable();
        _input.Player.SelectTarget.Disable();
    }

    /// <summary>
    /// 切换到选择目标模式
    /// </summary>
    public void SwitchToSelectTargetMode()
    {
        _input.Player.Fire.Disable();
        _input.Player.SelectTarget.Enable();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// Fire键按下事件
    /// </summary>
    public event Action OnFireStarted;

    /// <summary>
    /// Fire键松开事件
    /// </summary>
    public event Action OnFireCanceled;

    /// <summary>
    /// 选择目标事件
    /// </summary>
    public event Action OnTargetSelected;

    /// <summary>
    /// Fire Action回调
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 screenPos = Pointer.current != null ? Pointer.current.position.ReadValue() : PointerScreenPosition;

            if (IsPointerOverUI(screenPos))
            {
                return;
            }

            PointerPressStartPosition = screenPos;
            IsFireHeld = true;
            OnFireStarted?.Invoke();
        }
        else if (context.canceled)
        {
            IsFireHeld = false;
            OnFireCanceled?.Invoke();
        }
    }

    /// <summary>
    /// PointerPosition Action回调
    /// </summary>
    public void OnPointerPosition(InputAction.CallbackContext context)
    {
        PointerScreenPosition = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// SelectTarget Action回调
    /// </summary>
    public void OnSelectTarget(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 screenPos = Pointer.current != null ? Pointer.current.position.ReadValue() : PointerScreenPosition;

            if (IsPointerOverUI(screenPos))
            {
                return;
            }
            
            OnTargetSelected?.Invoke();
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 指定屏幕坐标是否点在UI
    /// </summary>
    private static bool IsPointerOverUI(Vector2 screenPosition)
    {
        var pointerData = new PointerEventData(EventSystem.current) { position = screenPosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
    #endregion
}

