using Hunting.Manager;
using UnityEngine;

namespace GooseCatcher.Manager
{
    /// <summary>
    /// 游戏管理器的基类，继承自MonoBehaviour并实现IGameManager接口
    /// 提供了游戏管理器的基本框架，包含初始化、开始游戏、更新和释放等核心功能
    /// </summary>
    public abstract class BaseGameManager : MonoBehaviour, IGameManager
    {
        /// <summary>
        /// 初始化游戏管理器
        /// 由派生类实现具体的初始化逻辑
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 开始游戏
        /// 由派生类实现具体的游戏开始逻辑
        /// </summary>
        public abstract void StartGame();

        /// <summary>
        /// 游戏更新逻辑
        /// 由派生类实现具体的游戏更新逻辑
        /// </summary>
        public abstract void DoUpdate();

        /// <summary>
        /// 释放资源
        /// 由派生类实现具体的资源释放逻辑
        /// </summary>
        public abstract void Release();
    }
}