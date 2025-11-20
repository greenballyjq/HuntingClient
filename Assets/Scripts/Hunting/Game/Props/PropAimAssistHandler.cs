using Hunting.Manager;

namespace Hunting.Game.Props
{
    /// <summary>
    /// 指哪打哪道具处理器
    /// </summary>
    public class PropAimAssistHandler : IPropHandler
    {
        /// <summary>
        /// 道具效果开始
        /// </summary>
        public void OnPropStart(PropContext context)
        {
            // TODO: 实现指哪打哪逻辑
        }

        /// <summary>
        /// 道具效果更新
        /// </summary>
        public void OnPropUpdate(PropContext context, float deltaTime)
        {
            // TODO: 实现指哪打哪更新逻辑
        }

        /// <summary>
        /// 道具效果结束
        /// </summary>
        public void OnPropEnd(PropContext context)
        {
            // TODO: 实现指哪打哪结束逻辑
        }
    }
}

