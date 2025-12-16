using cfg.HuntingConfig;
using Hunting.Manager;

namespace Hunting.Game.Luckys
{
    /// <summary>
    /// 幸运仪式增益上下文
    /// </summary>
    public class LuckyBuffContext
    {
        /// <summary>
        /// 幸运仪式增益配置
        /// </summary>
        public LuckyBuff LuckyBuffData { get; set; }
    }

    /// <summary>
    /// 幸运仪式增益处理器接口
    /// </summary>
    public interface ILuckyBuffHandler
    {
        /// <summary>
        /// 激活幸运仪式增益效果
        /// </summary>
        /// <param name="context">幸运仪式增益上下文</param>
        void OnActivate(LuckyBuffContext context);

        /// <summary>
        /// 注销幸运仪式增益效果
        /// </summary>
        /// <param name="context">幸运仪式增益上下文</param>
        void OnDeactivate(LuckyBuffContext context);
    }
}

