using GameFramework.Game;
using GameLogic.Game.Round.Boss;

namespace Hunting.Events
{
    /// <summary>
    /// Boss系统事件jian
    /// </summary>
    public static class BossEvents
    {
        /// <summary>
        /// Boss 死亡事件
        /// </summary>
        public static readonly EventKey<BossDiedEventArgs> BossDied = new EventKey<BossDiedEventArgs>();
    }

    /// <summary>
    /// Boss 死亡事件参数
    /// </summary>
    public sealed class BossDiedEventArgs : EventArgs
    {
        /// <summary>
        /// 死亡的Boss实例
        /// </summary>
        public BossBehaviour Boss { get; set; }
    }
}