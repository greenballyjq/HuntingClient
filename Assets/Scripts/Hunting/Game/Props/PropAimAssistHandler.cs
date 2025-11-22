using cfg.HuntingConfig.Prop;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Props
{
    /// <summary>
    /// 指哪打哪道具处理器
    /// </summary>
    public class PropAimAssistHandler : IPropHandler
    {
        /// <summary>
        /// 最大锁定距离
        /// </summary>
        private float _maxLockDistance;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 玩家控制管理器
        /// </summary>
        private PlayerControlManager PlayerControl => GameServiceLocator.GetGameManager<PlayerControlManager>();

        /// <summary>
        /// 道具效果开始
        /// </summary>
        public void OnPropStart(PropContext context)
        {
            // 读取配置参数
            PropAimAssist parameter = Config.GetPropAimAssist(context.PropData.ParamTableID);
            _maxLockDistance = parameter.MaxLockDistance;

            // 切换到指哪打哪控制模式
            PlayerControl.SwitchToAimAssist(_maxLockDistance);
        }

        /// <summary>
        /// 道具效果更新
        /// </summary>
        public void OnPropUpdate(PropContext context, float deltaTime)
        {
 
        }

        /// <summary>
        /// 道具效果结束
        /// </summary>
        public void OnPropEnd(PropContext context)
        {
            // 切换回默认控制模式
            PlayerControl.SwitchToDefaultShooting();
        }
    }
}

