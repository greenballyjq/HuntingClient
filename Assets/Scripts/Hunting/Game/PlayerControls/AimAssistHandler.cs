namespace Hunting.Game.PlayerControls
{
    using Hunting.Game.Weapons;
    using Hunting.Manager;
    using UnityEngine;

    /// <summary>
    /// 指哪打哪控制处理器
    /// </summary>
    public class AimAssistHandler : IPlayerControlHandler
    {
        /// <summary>
        /// 主武器
        /// </summary>
        private MainWeapon _weapon;

        /// <summary>
        /// 当前锁定的目标
        /// </summary>
        private Transform _currentTarget;

        /// <summary>
        /// 最大锁定距离
        /// </summary>
        private float _maxLockDistance;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 输入管理器
        /// </summary>
        private InputManager Input => GameServiceLocator.GetGameManager<InputManager>();

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager Weapon => GameServiceLocator.GetGameManager<WeaponManager>();

        public void OnControlStart()
        {
            Input.SwitchToSelectTargetMode();
            RegisterEvents();
        }

        public void OnControlUpdate(float deltaTime)
        {
            // TODO: 待以后实现LoadingManager后移除
            if (_weapon == null)
            {
                _weapon = Weapon.GetMainWeapon();
                if (_weapon == null)
                    return;
            }

            // 检查是否有目标
            if (_currentTarget == null)
                return;

            // 检查目标距离
            float distance = Vector3.Distance(_weapon.transform.position, _currentTarget.position);
            if (distance > _maxLockDistance)
            {
                _currentTarget = null;
                return;
            }

            // 控制武器朝向目标
            _weapon.SetAimTarget(_currentTarget.position);

            // 持续射击
            _weapon.Fire();
        }

        public void OnControlEnd()
        {
            UnregisterEvents();
            _currentTarget = null;
        }

        #region 公共方法
        /// <summary>
        /// 设置最大锁定距离
        /// </summary>
        /// <param name="distance">最大锁定距离</param>
        public void SetMaxLockDistance(float distance)
        {
            _maxLockDistance = distance;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 处理目标选择
        /// </summary>
        private void HandleTargetSelected()
        {
            // 从屏幕坐标发射射线进行射线检测
            Vector2 screenPos = Input.PointerScreenPosition;
            Ray ray = Input.MainCamera.ScreenPointToRay(screenPos);

            int layerMask = LayerMask.GetMask("Animal");
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                _currentTarget = hit.transform;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Input.OnTargetSelected += HandleTargetSelected;
            Event.AddListener(AnimalEvents.AnimalDying, OnAnimalDying);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Input.OnTargetSelected -= HandleTargetSelected;
            Event.RemoveListener(AnimalEvents.AnimalDying, OnAnimalDying);
        }

        /// <summary>
        /// 动物进入死亡事件回调
        /// </summary>
        private void OnAnimalDying(AnimalDyingEventArgs args)
        {
            // 当死亡的动物是当前锁定目标时清除
            if (_currentTarget != null && args.Animal.transform == _currentTarget)
                _currentTarget = null;
        }
        #endregion
    }
}

