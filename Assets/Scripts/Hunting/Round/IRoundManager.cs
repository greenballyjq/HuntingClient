using Hunting.Manager;

namespace Hunting.Round
{
    /// <summary>
    /// 单局管理器接口
    /// </summary>
    public interface IRoundManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">单局上下文</param>
        void Init(RoundContext context);

        /// <summary>
        /// 开始
        /// </summary>
        void Start();

        /// <summary>
        /// 结束
        /// </summary>
        void End();

        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
    }
}
