using System;
using System.Collections.Generic;
using GameFramework.Game;
using GameFramework.UI;
using GameFramework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 输入管理器
/// </summary>
public class InputManager : IAppManager, IAppUpdatable, GameInputActions.IPlayerActions
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

    private bool _selectTargetMode;
    private bool _wasPressed;
    private bool _gameplayInputPaused;
    private UIManager _uiManager;

    public UniTask InitAsync()
    {
        BindServices();

#if UNITY_EDITOR
        InputSystem.settings.editorInputBehaviorInPlayMode =
            InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        TouchSimulation.Disable();
        if (Mouse.current != null)
            InputSystem.EnableDevice(Mouse.current);
#endif

        _input = new GameInputActions();
        _input.Player.SetCallbacks(this);

        _input.Player.Fire.Enable();
        _input.Player.SelectTarget.Disable();
        _input.Player.PointerPosition.Enable();
        BindUiInputModule(_input.asset);

        Log.Info("[InputManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        _input.Disable();
        _input.Dispose();
        Log.Info("[InputManager] 已释放");
    }

    public void DoUpdate(float dt)
    {
        if (_gameplayInputPaused)
            return;

        bool pressed = TryReadPointer(out Vector2 pos);
        if (pressed)
            PointerScreenPosition = pos;

        if (_selectTargetMode)
        {
            if (pressed && !_wasPressed && !IsPointerOverUI(pos))
                OnTargetSelected?.Invoke();
        }
        else
        {
            if (pressed && !_wasPressed && !IsPointerOverUI(pos))
            {
                PointerPressStartPosition = pos;
                IsFireHeld = true;
                OnFireStarted?.Invoke();
            }
            else if (!pressed && _wasPressed && IsFireHeld)
            {
                IsFireHeld = false;
                OnFireCanceled?.Invoke();
            }
        }

        _wasPressed = pressed;
    }

    #region 公共方法
    public void PauseGameplayInput()
    {
        _gameplayInputPaused = true;
        if (IsFireHeld)
        {
            IsFireHeld = false;
            OnFireCanceled?.Invoke();
        }
    }

    public void ResumeGameplayInput()
    {
        _gameplayInputPaused = false;
    }

    /// <summary>
    /// 切换到持续射击模式
    /// </summary>
    public void SwitchToFireMode()
    {
        _selectTargetMode = false;
        _input.Player.Fire.Enable();
        _input.Player.SelectTarget.Disable();
    }

    /// <summary>
    /// 切换到选择目标模式
    /// </summary>
    public void SwitchToSelectTargetMode()
    {
        _selectTargetMode = true;
        IsFireHeld = false;
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
    }

    /// <summary>
    /// PointerPosition Action回调
    /// </summary>
    public void OnPointerPosition(InputAction.CallbackContext context)
    {
        if (!_wasPressed)
            PointerScreenPosition = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// SelectTarget Action回调
    /// </summary>
    public void OnSelectTarget(InputAction.CallbackContext context)
    {
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 读取当前按下的指针坐标
    /// </summary>
    private static bool TryReadPointer(out Vector2 pos)
    {
        var touch = Touchscreen.current;
        if (touch != null && touch.primaryTouch.press.isPressed)
        {
            pos = touch.primaryTouch.position.ReadValue();
            return true;
        }

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.isPressed)
        {
            pos = mouse.position.ReadValue();
            return true;
        }

        pos = default;
        return false;
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
    }

    /// <summary>
    /// 把 UI Action Map 绑到框架 EventSystem
    /// </summary>
    private void BindUiInputModule(InputActionAsset actionsAsset)
    {
        InputSystemUIInputModule module = _uiManager.InputModule;
        if (actionsAsset == null || module == null)
            return;

        var uiMap = actionsAsset.FindActionMap("UI");
        if (uiMap == null)
            return;

        module.actionsAsset = actionsAsset;
        module.point = InputActionReference.Create(uiMap.FindAction("Point"));
        module.leftClick = InputActionReference.Create(uiMap.FindAction("Click"));
        module.rightClick = InputActionReference.Create(uiMap.FindAction("RightClick"));
        module.middleClick = InputActionReference.Create(uiMap.FindAction("MiddleClick"));
        module.scrollWheel = InputActionReference.Create(uiMap.FindAction("ScrollWheel"));
        module.move = InputActionReference.Create(uiMap.FindAction("Navigate"));
        module.submit = InputActionReference.Create(uiMap.FindAction("Submit"));
        module.cancel = InputActionReference.Create(uiMap.FindAction("Cancel"));
        module.trackedDevicePosition = InputActionReference.Create(uiMap.FindAction("TrackedDevicePosition"));
        module.trackedDeviceOrientation = InputActionReference.Create(uiMap.FindAction("TrackedDeviceOrientation"));
        module.pointerBehavior = UIPointerBehavior.AllPointersAsIs;
        uiMap.Enable();
    }

    /// <summary>
    /// 指定屏幕坐标是否点在UI
    /// </summary>
    private static bool IsPointerOverUI(Vector2 screenPosition)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;

        var pointerData = new PointerEventData(eventSystem) { position = screenPosition };
        var results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
    #endregion
}
