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
        private ISkillHandler _currentHandler;

        /// <summary>
        /// 技能上下文
        /// </summary>
        private SkillContext _skillContext;

        /// <summary>
        /// 剩余持续时间
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

        #region 生命周期
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

            _remainingTime -= Time.deltaTime;
            _currentHandler?.OnSkillUpdate(_skillContext, Time.deltaTime);

            if (_remainingTime <= 0f)
            {
                EndSkill();
            }
        }

        public override void Release()
        {
            UnregisterEvents();

            if (_isRunning)
            {
                EndSkill();
            }

            ResetState();
            Debug.Log("[SkillManager] 已释放");
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);
            Event.AddListener(EnergyEvents.EnergyConsumed, OnEnergyConsumed);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
            Event.RemoveListener(EnergyEvents.EnergyConsumed, OnEnergyConsumed);
        }

        /// <summary>
        /// 重置内部状态
        /// </summary>
        private void ResetState()
        {
            _currentHandler = null;
            _skillContext = null;
            _remainingTime = 0f;
            _isRunning = false;
        }

        /// <summary>
        /// 启动技能
        /// </summary>
        private void BeginSkill()
        {
            _remainingTime = Mathf.Max(0f, _skillContext.SkillData.Duration);
            _isRunning = true;

            _currentHandler.OnSkillStart(_skillContext);

            TriggerSkillStarted(new SkillStartedEventArgs
            {
                Sender = this,
                SkillData = _skillContext.SkillData,
                Context = _skillContext.RoundContext,
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

            _currentHandler?.OnSkillEnd(_skillContext);

            TriggerSkillEnded(new SkillEndedEventArgs
            {
                Sender = this,
                SkillData = _skillContext.SkillData,
                Context = _skillContext.RoundContext
            });

            Debug.Log("[SkillManager] 技能结束");
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

        /// <summary>
        /// 单局开始回调
        /// </summary>
        private void OnRoundStarted(RoundStartedEventArgs args)
        {
            ResetState();

            var skillData = Config.GetSkill(args.Context.SkillId);

            if (skillData == null)
            {
                Debug.LogWarning($"[SkillManager] 无法找到技能数据，SkillId: {args.Context.SkillId}");
                return;
            }

            _currentHandler = SkillHandlerFactory.CreateSkillHandler(skillData.SkillType);

            if (_currentHandler == null)
            {
                Debug.LogWarning($"[SkillManager] 未实现的技能类型: {skillData.SkillType}");
                return;
            }

            _skillContext = new SkillContext
            {
                SkillData = skillData,
                RoundContext = args.Context
            };

            Debug.Log($"[SkillManager] 已准备技能处理器: {skillData.SkillType}");
        }

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            if (_isRunning)
            {
                EndSkill();
            }

            ResetState();
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 能量消耗回调
        /// </summary>
        private void OnEnergyConsumed(EnergyConsumedEventArgs args)
        {
            BeginSkill();
        }
        #endregion
    }
}


