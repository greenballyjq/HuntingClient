using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using Hunting.App;
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
        /// 当局幸运仪式增益处理器
        /// </summary>
        private ILuckyBuffHandler _currentHandler;

        /// <summary>
        /// 当局幸运仪式增益上下文
        /// </summary>
        private LuckyBuffContext _currentLuckyBuffContext;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager = GameServiceLocator.Event;

        public void Init(RoundContext context)
        {
            // 读取本局幸运仪式增益配置
            LuckyBuff buffData = context.LuckyBuffData;

            // 未选择幸运仪式增益
            if (buffData == null)
            {
                ResetState();
                return;
            }

            // 创建处理器
            _currentHandler = LuckyBuffHandlerFactory.CreateLuckyBuffHandler(buffData.LuckyBuffType);
            if (_currentHandler == null)
            {
                Debug.LogWarning($"[LuckyBuffManager] 未实现的幸运仪式增益类型: {buffData.LuckyBuffType}");
                ResetState();
                return;
            }

            // 构造上下文
            _currentLuckyBuffContext = new LuckyBuffContext
            {
                LuckyBuffData = buffData
            };

            // 激活幸运仪式效果
            _currentHandler?.OnActivate(_currentLuckyBuffContext);

            // 触发幸运仪式增益激活事件
            TriggerLuckyBuffActivated(new LuckyBuffActivatedEventArgs
            {
                Sender = this,
                LuckyBuffData = buffData
            });
        }

        public void Dispose()
        {
            // 注销幸运仪式增益效果
            _currentHandler?.OnDeactivate(_currentLuckyBuffContext);

            ResetState();
        }

        #region 私有方法
        /// <summary>
        /// 重置内部状态
        /// </summary>
        private void ResetState()
        {
            _currentHandler = null;
            _currentLuckyBuffContext = null;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 触发幸运仪式增益激活事件
        /// </summary>
        private void TriggerLuckyBuffActivated(LuckyBuffActivatedEventArgs args)
        {
            _eventManager.Trigger(LuckyBuffEvents.LuckyBuffActivated, args);
        }
        #endregion
    }
}
