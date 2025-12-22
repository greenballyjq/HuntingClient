using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Game;
using Hunting.UI;
using UnityEngine;


namespace Hunting.Manager
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public sealed class RoundContext
    {
        /// <summary>
        /// 角色数据
        /// </summary>
        public Role RoleData { get; set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        public Map MapData { get; set; }

        /// <summary>
        /// 技能数据
        /// </summary>
        public Skill SkillData { get; set; }

        /// <summary>
        /// 幸运仪式增益数据
        /// </summary>
        public LuckyBuff LuckyBuffData { get; set; }

        /// <summary>
        /// 是否存在地图联动
        /// </summary>
        public bool HasMapAffinity { get; set; }
    }

    /// <summary>
    /// 单局管理器
    /// </summary>
    public sealed class RoundManager : BaseGameManager
    {
        /// <summary>
        /// 待启动的下一局的上下文
        /// </summary>
        private RoundContext _nextContext;

        /// <summary>
        /// 当前单局上下文
        /// </summary>
        public RoundContext CurrentContext { get; private set; }

        /// <summary>
        /// 是否处于进行中的单局
        /// </summary>
        public bool IsRoundRunning { get; private set; }

        /// <summary>
        /// 是否处于暂停状态
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager UI => GameServiceLocator.UI;

        public override void Init()
        {
            Debug.Log("[RoundManager] 初始化");
        }

        public override void Release()
        {
            Debug.Log("[RoundManager] 释放");
            CloseGameplayUI();
            ResetState();
        }

        public override void DoUpdate()
        {

        }

        #region 公共方法
        /// <summary>
        /// 设置下一局的上下文
        /// </summary>
        public void SetRoundContext(RoundContext context)
        {
            _nextContext = context;
        }

        /// <summary>
        /// 开始单局
        /// </summary>
        public async void StartRound()
        {
            if (_nextContext == null)
            {
                Debug.LogError("[RoundManager] 未找到可用的单局上下文");
                return;
            }

            CurrentContext = _nextContext;
            _nextContext = null;

            IsRoundRunning = true;
            IsPaused = false;

            await OpenGameplayUIAsync();

            // 触发单局开始事件
            TriggerRoundStarted(new RoundStartedEventArgs
            {
                Sender = this,
                Context = CurrentContext
            });
        }

        /// <summary>
        /// 暂停单局
        /// </summary>
        public void PauseRound()
        {
            if (!IsRoundRunning || IsPaused)
                return;

            IsPaused = true;

            // 通知局内管理器暂停运行（停止AI、暂停计时器等）
            Event.Trigger(RoundEvents.RoundPaused);

            Debug.Log("[RoundManager] 单局已暂停");
        }

        /// <summary>
        /// 恢复单局
        /// </summary>
        public void ResumeRound()
        {
            if (!IsRoundRunning || !IsPaused)
                return;

            IsPaused = false;

            // 通知局内管理器恢复运行（恢复AI、恢复计时器等）
            Event.Trigger(RoundEvents.RoundResumed);

            Debug.Log("[RoundManager] 单局已恢复");
        }

        /// <summary>
        /// 结束单局
        /// </summary>
        public void EndRound(bool isCompleted)
        {
            if (!IsRoundRunning)
                return;

            // 通知管理器执行收尾工作（生成结算、清理对象等）
            TriggerRoundEnded(new RoundEndedEventArgs
            {
                Sender = this,
                Context = CurrentContext
            });

            CloseGameplayUI();
            ResetState();

            Debug.Log($"[RoundManager] 单局结束，是否完成:{isCompleted}");
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 触发单局开始事件
        /// </summary>
        private void TriggerRoundStarted(RoundStartedEventArgs args)
        {
            Event.Trigger(RoundEvents.RoundStarted, args);
        }

        /// <summary>
        /// 触发单局结束事件
        /// </summary>
        private void TriggerRoundEnded(RoundEndedEventArgs args)
        {
            Event.Trigger(RoundEvents.RoundEnded, args);
        }

        /// <summary>
        /// 重置单局状态
        /// </summary>
        private void ResetState()
        {
            CurrentContext = null;
            _nextContext = null;
            IsRoundRunning = false;
            IsPaused = false;
        }
        #endregion

        #region TODO: 待转移到专门调度UI的类中
        /// <summary>
        /// 打开游玩界面
        /// </summary>
        private async UniTask OpenGameplayUIAsync()
        {
            await UI.OpenUIAsync<UIGameplay>("UIHuntingGameplay", UIManager.UILayer.Game);
        }

        /// <summary>
        /// 关闭游玩界面
        /// </summary>
        private void CloseGameplayUI()
        {
            if (UI.IsUIOpened("UIHuntingGameplay"))
                UI.CloseUI("UIHuntingGameplay");
        }
        #endregion
    }
}


