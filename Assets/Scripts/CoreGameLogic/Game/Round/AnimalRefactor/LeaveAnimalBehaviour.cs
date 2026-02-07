using cfg.HuntingConfig;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 离场动物行为类
    /// </summary>
    public class LeaveAnimalBehaviour : BaseAnimalBehaviour
    {
        /// <summary>
        /// 离场状态
        /// </summary>
        public AnimalLeaveState LeaveState { get; private set; }

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

        #region 公共方法
        public override void Init(Specie data)
        {
            _stayTime = data.StayTime;
            _stayTimer = 0f;
            _stayFinished = false;

            LeaveState = new AnimalLeaveState(_stateMachine, this);

            base.Init(data);
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
            if (_stateMachine.CurrentState is AnimalDeathState || _stateMachine.CurrentState is AnimalLeaveState)
                return;

            if (_stayFinished)
                return;

            _stayTimer += dt;
            if (_stayTimer >= _stayTime)
            {
                _stayFinished = true;
                _stateMachine.ChangeState(LeaveState);
            }
        }
        #endregion
    }
}

