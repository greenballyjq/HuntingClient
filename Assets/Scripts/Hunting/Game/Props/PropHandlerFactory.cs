using cfg.HuntingConfig.Enum;

namespace Hunting.Game.Props
{
    /// <summary>
    /// 道具处理器工厂
    /// </summary>
    public static class PropHandlerFactory
    {
        /// <summary>
        /// 创建道具处理器
        /// </summary>
        /// <param name="propType">道具类型</param>
        /// <returns>道具处理器实例</returns>
        public static IPropHandler CreatePropHandler(EPropType propType)
        {
            switch (propType)
            {
                case EPropType.Bombardment:
                    //return new PropBombardmentHandler();
                case EPropType.AimAssist:
                    //return new PropAimAssistHandler();
                case EPropType.Trap:
                    //return new PropTrapHandler();
                default:
                    return null;
            }
        }
    }
}

