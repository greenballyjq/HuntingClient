using cfg.HuntingConfig.Skill;
using GameFramework.Core;
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
        /// 当前技能处理器
        /// </summary>
        private ISkillHandler _handler;

        /// <summary>
        /// 当前技能上下文
        /// </summary>
        private SkillContext _currentSkillContext;

        /// <summary>
        /// 技能是否正在运行
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// 技能剩余时间
        /// </summary>
        private float _remainingTime;

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
            _handler?.OnSkillUpdate(_currentSkillContext, Time.deltaTime);

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
        /// 当前是否可以启动技能
        /// </summary>
        public bool CanStartSkill()
        {
            if (_handler == null || _isRunning || !Energy.CanConsume())
                return false;

            return true;
        }

        /// <summary>
        /// 尝试启动技能
        /// </summary>
        public bool TryStartSkill()
        {
            if (!CanStartSkill())
            {
                Debug.LogWarning("[SkillManager] 当前无法启动技能");
                return false;
            }

            // 只有成功消耗能量条后才启动技能
            if (!Energy.TryConsumeOneBar())
            {
                Debug.LogWarning("[SkillManager] 能量消耗失败，取消启动技能");
                return false;
            }

            BeginSkill();
            return true;
        }

        /// <summary>
        /// 当前技能是否正在运行
        /// </summary>
        public bool IsRunning()
        {
            return _isRunning;
        }

        /// <summary>
        /// 获取技能剩余时间
        /// </summary>
        public float GetRemainingTime()
        {
            return _remainingTime;
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
            _handler.OnSkillStart(_currentSkillContext);

            TriggerSkillStarted(new SkillStartedEventArgs
            {
                Sender = this,
                SkillData = _currentSkillContext.SkillData,
                Context = _currentSkillContext.RoundContext,
                Duration = _remainingTime
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
            _handler?.OnSkillEnd(_currentSkillContext);

            _isRunning = false;
            _remainingTime = 0f;

            TriggerSkillEnded(new SkillEndedEventArgs
            {
                Sender = this,
                SkillData = _currentSkillContext.SkillData,
                Context = _currentSkillContext.RoundContext
            });

            Debug.Log("[SkillManager] 技能结束");
        }

        /// <summary>
        /// 重置内部状态
        /// </summary>
        private void ResetState()
        {
            _handler = null;
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
            Skill skill = Config.GetSkill(args.Context.SkillId);
            if (skill == null)
            {
                Debug.LogWarning($"[SkillManager] 未找到技能配置，SkillId:{args.Context.SkillId}");
                return;
            }

            _handler = SkillHandlerFactory.CreateSkillHandler(skill.SkillType);
            if (_handler == null)
            {
                Debug.LogWarning($"[SkillManager] 未实现的技能类型: {skill.SkillType}");
                return;
            }

            _currentSkillContext = new SkillContext
            {
                SkillData = skill,
                RoundContext = args.Context
            };

            Debug.Log($"[SkillManager] 已准备技能处理器: {skill.SkillType}");
        }

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            // 确保技能在单局结束时停止
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
