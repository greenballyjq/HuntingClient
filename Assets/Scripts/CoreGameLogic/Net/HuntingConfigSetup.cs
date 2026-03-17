using GameFramework.Network.Proxy;
using LitJson;

namespace CoreGameLogic.Net
{
    /// <summary>
    /// 打猎游戏配置注册 - 向框架层 GameConfigProxy 注册游戏特有的配置项
    /// 提供静态属性供 UI 代码访问已拉取的配置数据
    /// </summary>
    public class HuntingConfigSetup
    {
        /// <summary>
        /// 7日连续签到奖励配置
        /// </summary>
        public static JsonData DailyConfigData { get; private set; }
        
        /// <summary>
        /// 用户等级配置
        /// </summary>
        public static JsonData UserLevelConfigData { get; private set; }

        /// <summary>
        /// 游戏积分称号阶梯表
        /// </summary>
        public static JsonData ScoreRankGooseConfigData { get; private set; }
        
        private static bool _initialized = false;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            
            GameConfigProxy proxy = GameConfigProxy.Instance;
            // todo 拉取后端配置
        }
    }
}