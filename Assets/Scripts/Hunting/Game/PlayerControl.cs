using Cysharp.Threading.Tasks;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hunting.Game
{
    public class PlayerControl : MonoSingleton<PlayerControl>
    {     
        #region 输入系统相关
        /// <summary>
        /// 输入系统
        /// </summary>
        private PlayerInputSet input;
        
        public bool ShootingPerformed { get; private set; }

        /// <summary>
        /// 获取屏幕坐标
        /// </summary>
        private void OnAimPositionPerformed(InputAction.CallbackContext context)
        {
            // 获取屏幕坐标
            _aimPosition = context.ReadValue<Vector2>();
            AimWorldPosition = _mainCamera.ScreenToWorldPoint(
                new Vector3(_aimPosition.x, _aimPosition.y, aimDistance));
        }

        private void OnEnable()
        {
            // 启用输入系统并绑定事件
            input.Enable();
            input.Player.AimPosition.performed += OnAimPositionPerformed;
            input.Player.Shoot.performed += OnShootPerformed;
            input.Player.Shoot.canceled += OnShootCanceled;
        }

        private void OnShootPerformed(InputAction.CallbackContext obj)
        {
            if (!CheckValidArea()) return;
            ShootingPerformed = true;
        }
        
        private void OnShootCanceled(InputAction.CallbackContext obj)
        { 
            ShootingPerformed = false;
        }

        private void OnDisable()
        {
            // 禁用输入系统并解绑事件
            input.Disable();
            input.Player.AimPosition.performed -= OnAimPositionPerformed;
            input.Player.Shoot.performed -= OnShootPerformed;
            input.Player.Shoot.canceled -= OnShootCanceled;
        }
        #endregion

        #region 瞄准相关字段
        /// <summary>
        /// 触摸屏的瞄准位置（屏幕坐标）
        /// </summary>
        private Vector2 _aimPosition;

        /// <summary>
        /// 触摸屏的的瞄准位置（世界坐标）
        /// </summary>
        public Vector3 AimWorldPosition { get; private set; }
        
        /// <summary>
        /// 主摄像机
        /// </summary>
        private Camera _mainCamera;
        #endregion
        

        #region 编辑器可编辑字段
        
        [Header("瞄准设置")]
        [SerializeField] private float aimDistance = 10f; // 瞄准目标点的世界空间深度
        
        #endregion
        
        protected override async void Awake()
        {
            base.Awake();
            // 初始化输入系统
            input = new PlayerInputSet();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            // 检查操作区域
            CheckValidArea();
        }

        /// <summary>
        /// 检查输入位置是否在有效操作区域内
        /// </summary>
        private bool CheckValidArea()
        {
            float screenHeight = Screen.height;
            float minY = screenHeight * 0.2f; // 底部20%为无效区域
            float maxY = screenHeight * 0.8f; // 顶部20%为无效区域
            bool inValidArea = _aimPosition.y >= minY && _aimPosition.y <= maxY;
            return inValidArea;
        }
    }

}