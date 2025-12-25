using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.App;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 玩家数据管理器（占位）
    /// </summary>
    public class PlayerDataManager : IAppManager
    {
        /// <summary>
        /// 3币数量
        /// </summary>
        private int _threeKPCoin = 50;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        public void Init()
        {
            // TODO: 从服务器获取玩家数据
            Debug.Log("[PlayerDataManager] 初始化完成");
        }

        public void Dispose()
        {
            Debug.Log("[PlayerDataManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 获取3币数量
        /// </summary>
        public int GetThreeKPCoin()
        {
            return _threeKPCoin;
        }

        /// <summary>
        /// 更新3币数量
        /// </summary>
        public void UpdateThreeKPCoin(int amount)
        {
            int oldAmount = _threeKPCoin;
            _threeKPCoin += amount;

            int deltaAmount = _threeKPCoin - oldAmount;
            TriggerThreeKPCoinChanged(new ThreeKPCoinChangedEventArgs
            {
                CurrentAmount = _threeKPCoin,
                DeltaAmount = deltaAmount
            });
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 触发3币数量改变事件
        /// </summary>
        private void TriggerThreeKPCoinChanged(ThreeKPCoinChangedEventArgs args)
        {
            Event.Trigger(PlayerDataEvents.ThreeKPCoinChanged, args);
        }
        #endregion
    }
}
