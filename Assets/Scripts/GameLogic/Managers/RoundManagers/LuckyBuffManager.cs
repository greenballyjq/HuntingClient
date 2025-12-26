using cfg.HuntingConfig;
using Hunting.Game.Luckys;
using Hunting.Round;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 幸运仪式增益管理器
    /// </summary>
    public class LuckyBuffManager : IRoundManager
    {
        /// <summary>
        /// 幸运仪式增益处理器
        /// </summary>
        private ILuckyBuffHandler _luckyBuffhandler;

        /// <summary>
        /// 幸运仪式增益上下文
        /// </summary>
        private LuckyBuffContext _luckyBuffContext;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager = GameServiceLocator.EventManager;

        public void Init(RoundContext context)
        {
            // 获取本局幸运仪式增益配置
            LuckyBuff buffData = context.LuckyBuffData;

            // 未选择幸运仪式增益
            if (buffData == null)
                return;

            // 创建幸运仪式增益处理器
            _luckyBuffhandler = LuckyBuffHandlerFactory.CreateLuckyBuffHandler(buffData.LuckyBuffType);

            // 构造幸运仪式增益上下文
            _luckyBuffContext = new LuckyBuffContext
            {
                LuckyBuffData = buffData
            };

            // 启用幸运仪式效果
            _luckyBuffhandler?.OnActivate(_luckyBuffContext);

            // 触发幸运仪式增益激活事件
            TriggerLuckyBuffActivated(new LuckyBuffActivatedEventArgs
            {
                Sender = this,
                LuckyBuffData = buffData
            });
        }

        public void Dispose()
        {
            // 停用幸运仪式增益效果
            _luckyBuffhandler?.OnDeactivate(_luckyBuffContext);
        }

        #region 事件相关
        /// <summary>
        /// 触发幸运仪式增益激活事件
        /// </summary>
        private void TriggerLuckyBuffActivated(LuckyBuffActivatedEventArgs args)
        {
            _eventManager.Trigger(LuckyEvents.LuckyBuffActivated, args);
        }
        #endregion
    }
}
