using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Manager;

namespace Hunting.Round
{
    /// <summary>
    /// 单局服务集合
    /// </summary>
    public class RoundServices
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        public EventManager Event { get; }

        /// <summary>
        /// UI 管理器
        /// </summary>
        public UIManager UI { get; }

        /// <summary>
        /// 资源管理器
        /// </summary>
        public ResourceManager Resource { get; }

        /// <summary>
        /// 对象池管理器
        /// </summary>
        public GameObjectPoolManager Pool { get; }

        /// <summary>
        /// 配置管理器
        /// </summary>
        public HuntingConfigManager Config { get; }

        /// <summary>
        /// 玩家数据管理器
        /// </summary>
        public PlayerDataManager PlayerData { get; }

        // TODO: 需要暴露的其它应用级管理器在这里补充

        public RoundServices(
            EventManager eventManager,
            UIManager uiManager,
            ResourceManager resourceManager,
            GameObjectPoolManager poolManager,
            HuntingConfigManager configManager,
            PlayerDataManager playerDataManager)
        {
            Event = eventManager;
            UI = uiManager;
            Resource = resourceManager;
            Pool = poolManager;
            Config = configManager;
            PlayerData = playerDataManager;
        }
    }
}
