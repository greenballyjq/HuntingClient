using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Game;
using Hunting;
using GameFramework.Manager;
using Hunting.Manager;
using Hunting.Round;
using Hunting.UI;
using UnityEngine;
using GameFramework.Core;

namespace Hunting.App
{
    /// <summary>
    /// 打猎应用流程
    /// </summary>
    public class HuntingAppFlow : GameAppFlow
    {
        private static HuntingAppFlow _instance;
        public static HuntingAppFlow Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// 打猎应用流程状态枚举
        /// </summary>
        private enum HuntingAppFlowState
        {
            /// <summary>
            /// 无状态
            /// </summary>
            None,

            /// <summary>
            /// 准备
            /// </summary>
            Prepare,

            /// <summary>
            /// 单局
            /// </summary>
            Round
        }

        /// <summary>
        /// 打猎应用流程当前状态
        /// </summary>
        private HuntingAppFlowState _currentState = HuntingAppFlowState.None;

        /// <summary>
        /// 当前单局上下文
        /// </summary>
        private RoundContext _currentRoundContext;

        /// <summary>
        /// 当前局级流程实例
        /// </summary>
        private RoundFlow _roundFlow;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager => GameServiceLocator.EventManager;

        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager _uiManager => GameServiceLocator.UIManager;
        #region 公共方法
        /// <summary>
        /// 进入准备阶段
        /// </summary>
        public async UniTask EnterPrepareAsync()
        {
            _currentState = HuntingAppFlowState.Prepare;

            // 结束当前单局流程
            _roundFlow?.EndRound();
            _roundFlow = null;

            // TODO: 当前版本启动应用后即进入准备界面 未来根据需求调整
            await _uiManager.OpenUIAsync<UIPrepare>("UIPrepare");

            TriggerPrepareEntered();
        }

        /// <summary>
        /// 进入单局
        /// </summary>
        /// <param name="context">单局上下文</param>
        public void EnterRound(RoundContext context)
        {
            _currentState = HuntingAppFlowState.Round;

            // 记录下局上下文
            _currentRoundContext = context;

            TriggerPrepareExited(new PrepareExitedEventArgs
            {
                Sender = this,
                Context = _currentRoundContext
            });

            // 创建单局流程并开始
            _roundFlow = new RoundFlow();
            _roundFlow.StartRound(_currentRoundContext);
        }

        /// <summary>
        /// 获取单局流程
        /// </summary>
        /// <returns>单局流程实例</returns>
        public RoundFlow GetRoundFlow()
        {
            if (_currentState != HuntingAppFlowState.Round)
            {
                Debug.LogWarning("[HuntingAppFlow] 当前未进入单局，不允许访问 RoundFlow");
                return null;
            }
            return _roundFlow;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取配置管理器
        /// </summary>
        protected override BaseConfigManager GetConfigManager()
        {
            return HuntingConfigManager.Instance;
        }

        /// <summary>
        /// 注册应用级管理器
        /// </summary>
        protected override void RegisterAppManagers()
        {
            RegisterAppManager(new InputManager());
            RegisterAppManager(new PlayerDataManager());
        }
        #endregion

        #region 钩子方法
        /// <summary>
        /// 应用启动钩子
        /// </summary>
        protected override async UniTask OnAppStartAsync()
        {
            await _uiManager.OpenUIAsync<UITestPanel>("UITestPanel");
            //await EnterPrepareAsync();
        }

        /// <summary>
        /// 应用运行时钩子
        /// </summary>
        /// <param name="dt">时间增量</param>
        protected override void OnAppRunning(float dt)
        {
            if (_currentState != HuntingAppFlowState.Round)
                return;

            _roundFlow.DoUpdate(dt);
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 触发进入准备阶段事件
        /// </summary>
        private void TriggerPrepareEntered()
        {
            _eventManager.Trigger(HuntingAppFlowEvents.PrepareEntered);
        }

        /// <summary>
        /// 触发离开准备阶段事件
        /// </summary>
        private void TriggerPrepareExited(PrepareExitedEventArgs args)
        {
            _eventManager.Trigger(HuntingAppFlowEvents.PrepareExited, args);
        }
        #endregion
    }
}
