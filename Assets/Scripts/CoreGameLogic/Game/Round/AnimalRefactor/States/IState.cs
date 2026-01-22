namespace Hunting.Game.Animal
{
    /// <summary>
    /// 状态接口
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        void Enter();

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        void DoUpdate(float dt);

        /// <summary>
        /// 退出状态
        /// </summary>
        void Exit();

        /// <summary>
        /// 暂停状态
        /// </summary>
        void Pause(); 

        /// <summary>
        /// 恢复状态
        /// </summary>
        void Resume();
    }
}
