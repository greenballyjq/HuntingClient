using Hunting.Manager;

namespace Hunting.Round
{
    /// <summary>
    /// 单局流程控制器（占位，后续补全）
    /// </summary>
    public class RoundController
    {
        /// <summary>
        /// 当前单局上下文
        /// </summary>
        private RoundContext _context;

        #region 公共方法
        /// <summary>
        /// 开始单局
        /// </summary>
        /// <param name="context">单局上下文</param>
        public void StartRound(RoundContext context)
        {
            _context = context;
            // TODO: 接入单局生命周期（Init 系统、广播事件等）
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            // TODO: 调度单局系统更新
        }

        /// <summary>
        /// 结束单局
        /// </summary>
        public void EndRound()
        {
            // TODO: 收尾并清理单局资源
            _context = null;
        }
        #endregion
    }
}
