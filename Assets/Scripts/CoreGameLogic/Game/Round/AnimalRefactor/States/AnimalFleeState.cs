namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物逃跑状态
    /// </summary>
    public class AnimalFleeState : AnimalState
    {
        /// <summary>
        /// 逃跑速度倍率
        /// </summary>
        protected float _fleeSpeedRate;
        
        /// <summary>
        /// 逃跑持续时间
        /// </summary>
        protected float _fleeDuration;

        public AnimalFleeState(StateMachine stateMachine, AnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _fleeSpeedRate = 2f;

            _fleeDuration = 8f;

            animalBehavior.Moveable.SetMoveRate(_fleeSpeedRate);
            
            animalBehavior.AnimalAnimator.PlayFlee();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            animalBehavior.Moveable.DoUpdate(dt);

            // 检查是否到达逃跑持续时间
            if (stateTimer >= _fleeDuration)
            {
                // 暂时什么都不做
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        public override void Resume()
        {
            base.Resume();
            
            animalBehavior.AnimalAnimator.PlayFlee();
        }
    }
}

