using Hunting.App;
using Hunting.Game.PlayerControls;
using Hunting.Round;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 玩家控制管理器
    /// </summary>
    public class PlayerControlManager : IRoundManager, IRoundUpdatable
    {
        /// <summary>
        /// 当前控制处理器
        /// </summary>
        private IPlayerControlHandler _currentHandler;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager = GameServiceLocator.Event;

        public void Init(RoundContext context)
        {
            ResetState();
            SwitchToDefaultShooting();
            Debug.Log("[PlayerControlManager] 初始化完成");
        }

        public void DoUpdate(float deltaTime)
        {
            _currentHandler?.OnControlUpdate(deltaTime);
        }

        public void Dispose()
        {
            EndCurrentControl();
            ResetState();
            Debug.Log("[PlayerControlManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 切换到默认射击模式
        /// </summary>
        public void SwitchToDefaultShooting()
        {
            IPlayerControlHandler handler = PlayerControlHandlerFactory.CreatePlayerControlHandler(EControlType.DefaultShooting);
            SwitchHandler(handler);
        }

        /// <summary>
        /// 切换到指哪打哪模式
        /// </summary>
        /// <param name="minLockDistance">最小锁定距离</param>
        /// <param name="maxLockDistance">最大锁定距离</param>
        public void SwitchToAimAssist(float minLockDistance, float maxLockDistance)
        {
            IPlayerControlHandler handler = PlayerControlHandlerFactory.CreatePlayerControlHandler(EControlType.AimAssist);
            
            // 设置指哪打哪控制器的参数
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
        /// <param name="newHandler">新的控制处理器</param>
        private void SwitchHandler(IPlayerControlHandler newHandler)
        {
            // 结束当前处理器
            EndCurrentControl();

            // 切换到新处理器
            _currentHandler = newHandler;

            // 启动新处理器
            _currentHandler?.OnControlStart();
        }

        /// <summary>
        /// 结束当前控制
        /// </summary>
        private void EndCurrentControl()
        {
            _currentHandler?.OnControlEnd();
            _currentHandler = null;
        }

        /// <summary>
        /// 重置内部状态
        /// </summary>
        private void ResetState()
        {
            _currentHandler = null;
        }
        #endregion
    }
}
