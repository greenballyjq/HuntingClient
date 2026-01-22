using cfg.HuntingConfig;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 逃跑动物行为类
    /// </summary>
    public class FleeAnimalBehaviour : BaseAnimalBehaviour
    {
        /// <summary>
        /// 逃跑状态
        /// </summary>
        public AnimalFleeState FleeState { get; private set; }

        /// <summary>
        /// 驻场时间
        /// </summary>
        private float _stayTime;

        /// <summary>
        /// 驻场计时器
        /// </summary>
        private float _stayTimer;

        /// <summary>
        /// 是否完成驻场
        /// </summary>
        private bool _stayFinished;

        protected override void Awake()
        {
            base.Awake();
            FleeState = new AnimalFleeState(_stateMachine, this);

            AnimalEventTrigger = AnimalEventTrigger as FleeAnimalEventTrigger;
            
            var eventTrigger = AnimalEventTrigger as FleeAnimalEventTrigger;
        }

        #region 公共方法
        public override void Init(Specie data)
        {
            base.Init(data);

            // 设置驻场时间
            _stayTime = data.StayTime;
            _stayTimer = 0f;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            UpdateStayTime(dt);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 驻场时间更新
        /// </summary>
        /// <param name="dt"></param>
        private void UpdateStayTime(float dt)
        {
            if (_stateMachine.CurrentState is AnimalDeathState || _stateMachine.CurrentState is AnimalFleeState)
                return;

            if (_stayFinished)
                return;

            _stayTimer += dt;
            if (_stayTimer >= _stayTime)
            {
                _stayFinished = true;
                _stateMachine.ChangeState(FleeState);
            }
        }
        #endregion
    }
}

