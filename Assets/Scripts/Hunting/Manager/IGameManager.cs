namespace Hunting.Manager
{
    /// <summary>
    /// 游戏管理器接口，定义了游戏管理器的基本功能和方法
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        void Init();

        /// <summary>
        /// 开始游戏
        /// </summary>
        void StartGame();

        /// <summary>
        /// 更新游戏逻辑，每帧调用
        /// </summary>
        void DoUpdate();

        /// <summary>
        /// 释放资源，清理游戏管理器
        /// </summary>
        void Release();
    }
}