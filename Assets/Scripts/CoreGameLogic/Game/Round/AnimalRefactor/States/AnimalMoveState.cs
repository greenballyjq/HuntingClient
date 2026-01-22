namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物移动状态
    /// </summary>
    public class AnimalMoveState : AnimalState
    {
        /// <summary>
        /// 移动速度倍率
        /// </summary>
        protected float _moveSpeedRate = 1f;

        public AnimalMoveState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animalBehavior.Moveable.SetMoveRate(_moveSpeedRate);
            
            animalBehavior.AnimalVisual.PlayMove();
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

            animalBehavior.AnimalVisual.PlayMove();
        }
    }
}

