using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Game;
using Hunting.Game.Luckys;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 幸运仪式增益管理器
    /// </summary>
    public class LuckyBuffManager : BaseGameManager
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
        private EventManager Event => GameServiceLocator.Event;

        public override void Init()
        {
            RegisterEvents();
            ResetState();
            Debug.Log("[LuckyBuffManager] 初始化完成");
        }

        public override void Update()
        {

        }

        public override void Release()
        {
            UnregisterEvents();
            ResetState();
            Debug.Log("[LuckyBuffManager] 已释放");
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
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 单局开始回调
        /// </summary>
        private void OnRoundStarted(RoundStartedEventArgs args)
        {
            // 读取本局幸运仪式增益配置
            LuckyBuff buffData = args.Context.LuckyBuffData;

            // 未选择幸运仪式增益
            if (buffData == null)
                return;

            // 创建处理器
            _currentHandler = LuckyBuffHandlerFactory.CreateLuckyBuffHandler(buffData.LuckyBuffType);
            if (_currentHandler == null)
            {
                Debug.LogWarning($"[LuckyBuffManager] 未实现的幸运仪式增益类型: {buffData.LuckyBuffType}");
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

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            // 注销幸运仪式增益效果
            _currentHandler?.OnDeactivate(_currentLuckyBuffContext);

            ResetState();
        }

        /// <summary>
        /// 触发幸运仪式增益激活事件
        /// </summary>
        private void TriggerLuckyBuffActivated(LuckyBuffActivatedEventArgs args)
        {
            Event.Trigger(LuckyBuffEvents.LuckyBuffActivated, args);
        }
        #endregion
    }
}

