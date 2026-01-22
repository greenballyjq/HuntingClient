namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物移动状态
    /// </summary>
    public class AnimalMoveState : AnimalState, IStayState
    {
        /// <summary>
        /// 移动速度倍率
        /// </summary>
        protected float _moveSpeedRate;

        public AnimalMoveState(StateMachine stateMachine, AnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _moveSpeedRate = 1f;

            animalBehavior.Moveable.SetMoveRate(_moveSpeedRate);
            
            animalBehavior.AnimalAnimator.PlayMove();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            animalBehavior.Moveable.DoUpdate(dt);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Resume()
        {
            base.Resume();

            animalBehavior.Moveable.SetMoveRate(_moveSpeedRate);

            animalBehavior.AnimalAnimator.PlayMove();
        }
    }
}

