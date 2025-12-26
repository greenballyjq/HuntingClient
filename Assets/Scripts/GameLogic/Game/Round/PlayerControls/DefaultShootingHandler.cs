namespace Hunting.Game.PlayerControls
{
    using Hunting.App;
    using Hunting.Game.Weapons;
    using Hunting.Manager;
    using UnityEngine;

    /// <summary>
    /// 默认射击控制处理器
    /// </summary>
    public class DefaultShootingHandler : IPlayerControlHandler
    {
        /// <summary>
        /// 主武器
        /// </summary>
        private MainWeapon _weapon;

        /// <summary>
        /// 输入管理器
        /// </summary>
        private InputManager Input => GameServiceLocator.GetAppManager<InputManager>();

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

        /// <summary>
        /// 瞄准深度
        /// </summary>
        private const float AimDepth = 10f;

        public void OnControlStart()
        {
            Input.SwitchToFireMode();
        }

        public void OnControlUpdate(float deltaTime)
        {
            // TODO: 待以后实现LoadingManager后移除
            if (_weapon == null)
            {
                _weapon = _weaponManager.GetMainWeapon();
                if (_weapon == null)
                    return;  
            }

            // 获取输入
            Vector2 screenPos = Input.PointerScreenPosition;
            bool isFiring = Input.IsFireHeld;

            // 转换为世界坐标
            Vector3 worldPos = Input.ScreenToWorld(screenPos, AimDepth);

            // 控制武器瞄准
            _weapon.SetAimTarget(worldPos);

            // 控制武器开火
            if (isFiring)
                _weapon.Fire();
        }

        public void OnControlEnd()
        {

        }
    }
}

