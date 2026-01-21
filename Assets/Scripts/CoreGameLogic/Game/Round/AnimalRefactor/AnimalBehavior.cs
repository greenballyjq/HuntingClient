using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Pool;
using GameFramework.Manager;
using System.Collections.Generic;
using UnityEngine;


namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物基类
    /// </summary>
    public class AnimalBehavior : MonoBehaviour, IPoolItem
    {
        /// <summary>
        /// 状态机
        /// </summary>
        protected StateMachine _stateMachine;

        /// <summary>
        /// 移动状态
        /// </summary>
        public AnimalMoveState MoveState { get; private set; }

        /// <summary>
        /// 受击状态
        /// </summary>
        public AnimalHitState HitState { get; private set; }

        /// <summary>
        /// 死亡状态
        /// </summary>
        public AnimalDeathState DeathState { get; private set; }

        /// <summary>
        /// 逃跑状态
        /// </summary>
        public AnimalFleeState FleeState { get; private set; }

        /// <summary>
        /// 动物配置
        /// </summary>
        public Specie SpecieData { get; private set; }

        /// <summary>
        /// 血量组件
        /// </summary>
        public IHealth Health { get; private set; }

        /// <summary>
        /// 移动组件
        /// </summary>
        public IMoveable Moveable { get; private set; }

        /// <summary>
        /// 动画组件
        /// </summary>
        public AnimalAnimator AnimalAnimator { get; private set; }

        /// <summary>
        /// 视觉组件
        /// </summary>
        public AnimalVisual AnimalVisual { get; private set; }

        /// <summary>
        /// 碰撞体组件
        /// </summary>
        public Collider Collider { get; private set; }

        /// <summary>
        /// 驻场时间
        /// </summary>
        protected float _stayTime;

        /// <summary>
        /// 驻场计时器
        /// </summary>
        protected float _stayTimer;

        /// <summary>
        /// 是否完成驻场
        /// </summary>
        protected bool _stayFinished;

        protected virtual void Awake()
        {
            // 创建状态机实例
            _stateMachine = new StateMachine();

            // 获取组件
            Health = GetComponent<IHealth>();
            Moveable = GetComponent<IMoveable>();
            AnimalAnimator = GetComponent<AnimalAnimator>();
            AnimalVisual = GetComponent<AnimalVisual>();
            Collider = GetComponent<Collider>();
            
            // 创建状态实例
            MoveState = new AnimalMoveState(_stateMachine, this);
            HitState = new AnimalHitState(_stateMachine, this);
            DeathState = new AnimalDeathState(_stateMachine, this);
            FleeState = new AnimalFleeState(_stateMachine, this);
        }

        #region 公共方法
        /// <summary>
        /// 初始化动物
        /// </summary>
        /// <param name="data">配置数据</param>
        /// <param name="stayTime">驻场时间</param>
        public virtual void Init(Specie data, float stayTime = -1f)
        {
            SpecieData = data;

            // 初始化血量组件
            Health.Init(data.HP);

            // 初始化移动组件
            Moveable.Init();
            Moveable.SetSpeed(data.MoveSpeed);
            Moveable.StartMove();

            // 初始化动画组件
            AnimalAnimator.Init();

            // 初始化视觉组件
            AnimalVisual.Init(this);

            // 设置驻场时间
            _stayTime = stayTime;
            _stayTimer = 0f;

            // 初始化状态机
            _stateMachine.Init(MoveState);
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public virtual void DoUpdate(float dt)
        {
            _stateMachine.DoUpdate(dt);

            Moveable.DoUpdate(dt);

            AnimalVisual.DoUpdate(dt);

            UpdateStayTime(dt);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 驻场时间更新
        /// </summary>
        /// <param name="dt"></param>
        protected virtual void UpdateStayTime(float dt)
        {
            if (_stayTime == -1)
                return;

            if (_stayFinished)
                return;

            if (_stateMachine.CurrentState is IStayState)
                return;

            _stayTimer += dt;
            if(_stayTimer >= _stayTime)
            {
                _stayFinished = true;
                _stateMachine.ChangeState(FleeState);
            }
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 动物受伤事件回调
        /// </summary>
        private void OnHealthDamaged()
        {
            _stateMachine.EnterTempState(HitState);
        }

        /// <summary>
        /// 动物死亡事件回调
        /// </summary>
        private void OnHealthDeath()
        {
            _stateMachine.ChangeState(DeathState);
        }
        #endregion

        #region IPoolItem 接口实现
        public virtual void OnSpawned()
        {
            Collider.enabled = true;

            Health.OnDamaged += OnHealthDamaged;
            Health.OnDeath += OnHealthDeath;
        }

        public virtual void OnDespawned()
        {
            Health.OnDamaged -= OnHealthDamaged;
            Health.OnDeath -= OnHealthDeath;

            Collider.enabled = false;
        }
        #endregion

        #region 测试
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager => GameServiceLocator.EventManager;

        /// <summary>
        /// 特效管理器
        /// </summary>
        private EffectManager _effectManager => GameServiceLocator.GetFrameworkManager<EffectManager>();

        /// <summary>
        /// 触发动物进入死亡事件
        /// </summary>
        public void TriggerAnimalDying()
        {
            _eventManager.Trigger(AnimalEvents.AnimalDying, new AnimalDyingEventArgs
            {
                Animal = this,
                SpecieData = SpecieData,
            });
        }

        /// <summary>
        /// 触发动物死亡事件
        /// </summary>
        public void TriggerAnimalDied()
        {
            // 触发死亡事件
            _eventManager.Trigger(AnimalEvents.AnimalDied, new AnimalDiedEventArgs
            {
                Sender = this,
                Animal = this,
                SpecieData = SpecieData,
            });
        }

        /// <summary>
        /// 触发动物掉落奖励事件
        /// </summary>
        public void TriggerAnimalDropReward()
        {
            // 触发掉落奖励事件
            _eventManager.Trigger(AnimalEvents.AnimalDropReward, new AnimalDropRewardEventArgs
            {
                Sender = this,
                Animal = this,
                DropRewards = new Dictionary<EDropType, int>(SpecieData.DropRewards)
            });
        }

        /// <summary>
        /// 触发动物逃跑事件
        /// </summary>
        public void TriggerAnimalFled()
        {
            _eventManager.Trigger(AnimalEvents.AnimalFled, new AnimalFledEventArgs
            {
                Sender = this,
                Animal = this,
                SpecieData = SpecieData
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Border"))
            {
                _eventManager.Trigger(AnimalEvents.AnimalReachedWall,
                    new AnimalReachedWallEventArgs { Sender = this, Animal = this });
            }
        }

        /// <summary>
        /// 播放死亡特效
        /// </summary>
        public void PlayDeathEffect()
        {
            _effectManager.PlayOneShotAsync(SpecieData.EffectPrefabPath, transform.position, Quaternion.identity).Forget();
        }
        #endregion
    }
}

