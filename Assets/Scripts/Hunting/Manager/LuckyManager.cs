using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.Game.Luckys;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 幸运仪式管理器
    /// </summary>
    public class LuckyManager : BaseGameManager
    {
        /// <summary>
        /// 当局幸运仪式处理器
        /// </summary>
        private ILuckyHandler _handler;

        /// <summary>
        /// 当局幸运仪式上下文
        /// </summary>
        private LuckyContext _currentLuckyContext;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        public override void Init()
        {
            RegisterEvents();
            ResetState();
            Debug.Log("[LuckyManager] 初始化完成");
        }

        public override void Update()
        {

        }

        public override void Release()
        {
            UnregisterEvents();
            ResetState();
            Debug.Log("[LuckyManager] 已释放");
        }

        #region 私有方法
        /// <summary>
        /// 重置内部状态
        /// </summary>
        private void ResetState()
        {
            _handler = null;
            _currentLuckyContext = null;
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
            ResetState();

            // 读取本局幸运仪式配置
            ELuckyType luckyType = args.Context.LuckyType;
            if (luckyType == ELuckyType.None)
            {
                Debug.Log("[LuckyManager] 本局未选择幸运仪式");
                return;
            }

            Lucky luckyData = Config.GetLucky(luckyType);
            if (luckyData == null)
            {
                Debug.LogWarning($"[LuckyManager] 未找到幸运仪式配置，LuckyType:{luckyType}");
                return;
            }

            // 创建对应的处理器
            _handler = LuckyHandlerFactory.CreateLuckyHandler(luckyType);
            if (_handler == null)
            {
                Debug.LogWarning($"[LuckyManager] 未实现的幸运仪式类型: {luckyType}");
                return;
            }

            // 构造幸运仪式上下文
            _currentLuckyContext = new LuckyContext
            {
                LuckyData = luckyData
            };

            // 激活幸运仪式效果
            _handler?.OnActivate(_currentLuckyContext);

            // 触发幸运仪式激活事件
            TriggerLuckyActivated(new LuckyActivatedEventArgs
            {
                Sender = this,
                LuckyData = luckyData
            });

            Debug.Log($"[LuckyManager] 幸运仪式已激活: {luckyType}");
        }

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            // 注销幸运仪式效果
            if (_handler != null && _currentLuckyContext != null)
                _handler?.OnDeactivate(_currentLuckyContext);

            ResetState();
        }

        /// <summary>
        /// 触发幸运仪式激活事件
        /// </summary>
        private void TriggerLuckyActivated(LuckyActivatedEventArgs args)
        {
            Event.Trigger(LuckyEvents.LuckyActivated, args);
        }
        #endregion
    }
}

