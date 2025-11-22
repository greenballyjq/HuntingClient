using cfg.HuntingConfig.Skill;
using Hunting.Game.Skills;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 技能管理器
    /// </summary>
    public class SkillManager : BaseGameManager
    {
        /// <summary>
        /// 当局技能处理器
        /// </summary>
        private ISkillHandler _currentHandler;

        /// <summary>
        /// 当局技能上下文
        /// </summary>
        private SkillContext _currentSkillContext;

        /// <summary>
        /// 技能剩余时间
        /// </summary>
        private float _remainingTime;

        /// <summary>
        /// 技能是否正在运行
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 能量条管理器
        /// </summary>
        private EnergyProgressManager Energy => GameServiceLocator.GetGameManager<EnergyProgressManager>();

        public override void Init()
        {
            RegisterEvents();
            ResetState();
            Debug.Log("[SkillManager] 初始化完成");
        }

        public override void Update()
        {
            if (!_isRunning)
                return;

            // 逐帧递减持续时间并调用处理器更新
            _remainingTime -= Time.deltaTime;
            _currentHandler?.OnSkillUpdate(_currentSkillContext, Time.deltaTime);

            if (_remainingTime <= 0f)
                EndSkill();
        }

        public override void Release()
        {
            UnregisterEvents();
            EndSkill();
            ResetState();
            Debug.Log("[SkillManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 尝试启动技能
        /// </summary>
        public bool TryStartSkill()
        {
            if (_currentHandler == null || _isRunning || !Energy.TryConsumeOneBar())
                return false;

            BeginSkill();
            return true;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 启动技能
        /// </summary>
        private void BeginSkill()
        {
            _remainingTime =  _currentSkillContext.SkillData.Duration;
            _isRunning = true;

            // 通知处理器执行开始逻辑
            _currentHandler?.OnSkillStart(_currentSkillContext);

            TriggerSkillStarted(new SkillStartedEventArgs
            {
                Sender = this,
                SkillData = _currentSkillContext.SkillData
            });

            Debug.Log($"[SkillManager] 技能开始，持续 {_remainingTime:F2} 秒");
        }

        /// <summary>
        /// 结束技能
        /// </summary>
        private void EndSkill()
        {
            if (!_isRunning)
                return;

            // 通知处理器执行结束逻辑
            _currentHandler?.OnSkillEnd(_currentSkillContext);

            _isRunning = false;
            _remainingTime = 0f;

            TriggerSkillEnded(new SkillEndedEventArgs
            {
                Sender = this,
                SkillData = _currentSkillContext.SkillData
            });

            Debug.Log("[SkillManager] 技能结束");
        }

        /// <summary>
        /// 重置内部状态
        /// </summary>
        private void ResetState()
        {
            _currentHandler = null;
            _currentSkillContext = null;
            _isRunning = false;
            _remainingTime = 0f;
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

            // 读取本局技能配置
            Skill skillData = Config.GetSkill(args.Context.SkillId);

            // 创建处理器
            _currentHandler = SkillHandlerFactory.CreateSkillHandler(skillData.SkillType);

            // 构造上下文
            _currentSkillContext = new SkillContext
            {
                SkillData = skillData,
                RoundContext = args.Context
            };
        }

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            EndSkill();
            ResetState();
        }

        /// <summary>
        /// 触发技能开始事件
        /// </summary>
        private void TriggerSkillStarted(SkillStartedEventArgs args)
        {
            Event.Trigger(SkillEvents.SkillStarted, args);
        }

        /// <summary>
        /// 触发技能结束事件
        /// </summary>
        private void TriggerSkillEnded(SkillEndedEventArgs args)
        {
            Event.Trigger(SkillEvents.SkillEnded, args);
        }
        #endregion
    }
}
