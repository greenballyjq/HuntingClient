namespace Hunting.Round
{
    /// <summary>
    /// 单局管理器可更新
    /// </summary>
    public interface IRoundUpdatable
    {
        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        void Update(float deltaTime);
    }
}
