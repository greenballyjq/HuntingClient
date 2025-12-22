namespace Hunting.Round
{
    /// <summary>
    /// 单局管理器可暂停
    /// </summary>
    public interface IRoundPausable
    {
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复
        /// </summary>
        void Resume();
    }
}
