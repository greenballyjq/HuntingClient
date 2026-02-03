namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物受击状态
    /// </summary>
    public class AnimalHitState : AnimalState
    {
        /// <summary>
        /// 受伤持续时间
        /// </summary>
        protected float _hitDuration = 0.2f;

        /// <summary>
        /// 受伤移动速率
        /// </summary>
        protected float _hitMoveSpeedRate = 0.5f;

        public AnimalHitState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animalBehavior.Moveable.SetMoveRate(_hitMoveSpeedRate);
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            animalBehavior.Moveable.DoUpdate(dt);

            if (stateTimer >= _hitDuration)
                stateMachine.ExitTempState();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

