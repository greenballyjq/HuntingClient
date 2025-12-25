using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Game;
using Hunting;
using GameFramework.Manager;
using Hunting.Manager;
using Hunting.Round;
using Hunting.UI;
using UnityEngine;

namespace Hunting.App
{
    /// <summary>
    /// 打猎应用流程
    /// </summary>
    public sealed class HuntingAppFlow : GameAppFlow
    {
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
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager UI => GameServiceLocator.UI;

        /// <summary>
        /// 单局管理器
        /// </summary>
        private RoundManager Round => GameServiceLocator.GetAppManager<RoundManager>();

        #region 公共方法
        /// <summary>
        /// 进入准备阶段
        /// </summary>
        public async UniTask EnterPrepareAsync()
        {
            _currentState = HuntingAppFlowState.Prepare;

            // TODO: 当前版本启动应用后即进入准备界面 未来根据需求调整
            await UI.OpenUIAsync<UIPrepare>("UIPrepare");

            TriggerPrepareEntered();
        }

        /// <summary>
        /// 开始单局
        /// </summary>
        /// <param name="context">单局上下文</param>
        public void StartRound(RoundContext context)
        {
            _currentState = HuntingAppFlowState.Round;

            // 记录下局上下文
            _currentRoundContext = context;

            TriggerPrepareExited(new PrepareExitedEventArgs
            {
                Sender = this,
                Context = _currentRoundContext
            });

            // TODO: 现阶段复用 RoundManager，未来可切换到 RoundFlow 统一调度
            Round.SetRoundContext(_currentRoundContext);
            Round.StartRound();
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
            // TODO: 下面方法并非都是真的“应用级”管理器，未来需要整理分类
            RegisterAppManager(new RoundManager());
            RegisterAppManager(new InputManager());
            RegisterAppManager(new PlayerControlManager());
            RegisterAppManager(new PlayerDataManager());
            RegisterAppManager(new LuckyBuffManager());
            RegisterAppManager(new MeatProgressManager());
            RegisterAppManager(new EnergyProgressManager());
            RegisterAppManager(new SettlementRewardManager());
            RegisterAppManager(new WeaponManager());
            RegisterAppManager(new BulletManager());
            RegisterAppManager(new SpawnerManager());
            RegisterAppManager(new TrapManager());
            RegisterAppManager(new PropManager());
            RegisterAppManager(new SkillManager());
            RegisterAppManager(new QuestManager());
        }
        #endregion

        #region 钩子方法
        /// <summary>
        /// 应用启动钩子
        /// </summary>
        protected override async UniTask OnAppStartAsync()
        {
            await EnterPrepareAsync();
        }

        /// <summary>
        /// 应用运行时钩子
        /// </summary>
        /// <param name="dt">时间增量</param>
        protected override void OnAppRunning(float dt)
        {
            if (_currentState != HuntingAppFlowState.Round)
                return;

            // TODO: 未来接入 RoundFlow 的更新调度
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 触发离开准备阶段事件
        /// </summary>
        private void TriggerPrepareExited(PrepareExitedEventArgs args)
        {
            Event.Trigger(HuntingAppFlowEvents.PrepareExited, args);
        }

        private void TriggerPrepareEntered()
        {
            Event.Trigger(HuntingAppFlowEvents.PrepareEntered);
        }
        #endregion
    }
}
