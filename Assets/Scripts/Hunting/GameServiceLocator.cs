using Cysharp.Threading.Tasks;
using GameFramework.Core;
using Hunting.Manager;

namespace Hunting
{
    /// <summary>
    /// 游戏服务定位器 - 统一的服务访问点
    /// </summary>
    public static class GameServiceLocator
    {
        #region 常用管理器
        public static EventManager Events => GameLogic.Instance.GetFrameworkManager<EventManager>();
        public static ResourceManager Resources => GameLogic.Instance.GetFrameworkManager<ResourceManager>();
        public static UIManager UI => GameLogic.Instance.GetFrameworkManager<UIManager>();
        public static HuntingGameConfigManager Config => HuntingGameConfigManager.Instance;
        #endregion

        /// <summary>
        /// 获取框架管理器
        /// </summary>
        public static T GetFrameworkManager<T>() where T : class, IManager
        {
            return GameLogic.Instance.GetFrameworkManager<T>();
        }

        /// <summary>
        /// 获取游戏业务管理器
        /// </summary>
        public static T GetGameManager<T>() where T : class, IGameManager
        {
            return GameLogic.Instance.GetGameManager<T>();
        }

        /// <summary>
        /// 异步等待获取框架管理器
        /// </summary>
        public static async UniTask<T> GetFrameworkManagerAsync<T>() where T : class, IManager
        {
            await GameLogic.Instance.WaitForInitialization();
            return GameLogic.Instance.GetFrameworkManager<T>();
        }

        /// <summary>
        /// 异步等待获取游戏业务管理器
        /// </summary>
        public static async UniTask<T> GetGameManagerAsync<T>() where T : class, IGameManager
        {
            await GameLogic.Instance.WaitForInitialization();
            return GameLogic.Instance.GetGameManager<T>();
        }

        /// <summary>
        /// 等待游戏初始化完成
        /// </summary>
        public static async UniTask WaitForInitialization()
        {
            await GameLogic.Instance.WaitForInitialization();
        }
    }
}