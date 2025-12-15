using GameFramework.Game;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hunting.Manager
{
    /// <summary>
    /// 输入管理器
    /// </summary>
    public class InputManager : BaseGameManager, GameInputActions.IPlayerActions
    {
        /// <summary>
        /// 输入系统
        /// </summary>
        private GameInputActions _input;

        /// <summary>
        /// 主摄像机
        /// </summary>
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera;

        /// <summary>
        /// 指针屏幕坐标
        /// </summary>P
        public Vector2 PointerScreenPosition { get; private set; }

        /// <summary>
        /// Fire键是否按住
        /// </summary>
        public bool IsFireHeld { get; private set; }

        public override void Init()
        {
            // 初始化输入系统
            _input = new GameInputActions();
            _input.Player.SetCallbacks(this);
            _mainCamera = Camera.main;

            // 默认启用Fire Action，禁用SelectTarget Action
            _input.Player.Fire.Enable();
            _input.Player.SelectTarget.Disable();
            _input.Player.PointerPosition.Enable();

            Debug.Log("[InputManager] 初始化完成");
        }

        public override void Update()
        {
            // InputManager不需要每帧更新逻辑
        }

        public override void Release()
        {
            _input.Disable();
            _input.Dispose();
            Debug.Log("[InputManager] 已释放");
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

        /// <summary>
        /// 将屏幕坐标转换为世界坐标
        /// </summary>
        /// <param name="screenPosition">屏幕坐标</param>
        /// <param name="depth">世界空间深度</param>
        /// <returns>世界坐标</returns>
        public Vector3 ScreenToWorld(Vector2 screenPosition, float depth)
        {
            return _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
        }

        /// <summary>
        /// 将世界坐标转换为屏幕坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>屏幕坐标</returns>
        public Vector3 WorldToScreen(Vector3 worldPosition)
        {
            return _mainCamera.WorldToScreenPoint(worldPosition);
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
                // Fire键按下
                IsFireHeld = true;
                OnFireStarted?.Invoke();
            }
            else if (context.canceled)
            {
                // Fire键松开
                IsFireHeld = false;
                OnFireCanceled?.Invoke();
            }
        }

        /// <summary>
        /// PointerPosition Action回调
        /// </summary>
        public void OnPointerPosition(InputAction.CallbackContext context)
        {
            // 更新指针屏幕坐标
            PointerScreenPosition = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// SelectTarget Action回调
        /// </summary>
        public void OnSelectTarget(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                // 触发选择目标事件
                OnTargetSelected?.Invoke();
            }
        }

        #endregion
    }
}

