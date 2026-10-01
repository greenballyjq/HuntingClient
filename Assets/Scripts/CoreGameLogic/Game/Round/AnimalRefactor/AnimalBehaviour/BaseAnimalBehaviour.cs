using cfg.HuntingConfig;
using GameFramework.Core.Pool;
using UnityEngine;


namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物行为基类
    /// </summary>
    public class BaseAnimalBehaviour : MonoBehaviour, IPoolItem
    {
        /// <summary>
        /// 状态机
        /// </summary>
        protected StateMachine _stateMachine;

        /// <summary>
        /// 当前状态
        /// </summary>
        public AnimalState CurrentState => _stateMachine.CurrentState as AnimalState;

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
        /// 被控制状态
        /// </summary>
        public AnimalHeldState HeldState { get; private set; }

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
        /// 动物视觉组件
        /// </summary>
        public BaseAnimalVisual AnimalVisual { get; protected set; }

        /// <summary>
        /// 动物事件触发器组件
        /// </summary>
        public BaseAnimalEventTrigger AnimalEventTrigger { get; protected set; }

        private RoundNumericLayer _numeric;

        /// <summary>
        /// 碰撞体组件
        /// </summary>
        public Collider Collider { get; private set; }

        protected virtual void Awake()
        {
            // 创建状态机实例
            _stateMachine = new StateMachine();

            // 获取组件
            Health = GetComponent<IHealth>();
            Moveable = GetComponent<IMoveable>();
            AnimalVisual = GetComponent<BaseAnimalVisual>();
            AnimalEventTrigger = GetComponent<BaseAnimalEventTrigger>();
            Collider = GetComponent<Collider>();
        }

        #region 公共方法
        /// <summary>
        /// 初始化动物行为类
        /// </summary>
        /// <param name="data">配置数据</param>
        public virtual void Init(Specie data)
        {
            SpecieData = data;

            // 初始化血量组件
            if (_numeric == null)
                _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();

            Health.Init(_numeric.EvaluateMaxHealth(data.HP));

            // 初始化移动组件
            Moveable.Init();
            Moveable.SetSpeed(data.MoveSpeed);
            Moveable.StartMove();

            // 初始化视觉组件
            AnimalVisual.Init(this);

            // 初始化事件触发器组件
            AnimalEventTrigger.Init(this);

            // 创建状态实例
            MoveState = new AnimalMoveState(_stateMachine, this);
            HitState = new AnimalHitState(_stateMachine, this);
            DeathState = new AnimalDeathState(_stateMachine, this);
            HeldState = new AnimalHeldState(_stateMachine, this);

            // 初始化状态机
            InitStateMachine();
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        protected virtual void InitStateMachine()
        {
            _stateMachine.Init(MoveState);
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public virtual void DoUpdate(float dt)
        {
            _stateMachine.DoUpdate(dt);
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 动物受伤事件回调
        /// </summary>
        protected virtual void OnDamaged()
        {
            _stateMachine.EnterTempState(HitState,true);
        }

       
        /// <summary>
        /// 动物死亡事件回调
        /// </summary>
        protected virtual void OnDeath()
        {
            _stateMachine.ChangeState(DeathState);
        }

        /// <summary>
        /// 动物被控制事件回调
        /// </summary>
        protected virtual void OnHeld()
        {
            _stateMachine.EnterTempState(HeldState, true, false);
        }


        #endregion

        #region IPoolItem 接口实现
        public virtual void OnSpawned()
        {
            Collider.enabled = true;

            Health.OnDamaged += OnDamaged;
            Health.OnDeath += OnDeath;
            Health.OnHeld += OnHeld;
        }

        public virtual void OnDespawned()
        {
            Health.OnDamaged -= OnDamaged;
            Health.OnDeath -= OnDeath;
            Health.OnHeld -= OnHeld;

            Collider.enabled = false;
        }
        #endregion
    }
}

