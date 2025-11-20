using cfg.HuntingConfig;
using Hunting.Manager;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 幸运仪式上下文
    /// </summary>
    public class LuckyContext
    {
        /// <summary>
        /// 幸运仪式数据
        /// </summary>
        public Lucky LuckyData { get; set; }
    }

    /// <summary>
    /// 幸运仪式处理器接口
    /// </summary>
    public interface ILuckyHandler
    {
        /// <summary>
        /// 激活幸运仪式效果
        /// </summary>
        /// <param name="context">幸运仪式上下文</param>
        void OnActivate(LuckyContext context);

        /// <summary>
        /// 注销幸运仪式效果
        /// </summary>
        /// <param name="context">幸运仪式上下文</param>
        void OnDeactivate(LuckyContext context);
    }
}

