namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物受击状态
    /// </summary>
    public class AnimalHitState : AnimalState, IStayState
    {
        /// <summary>
        /// 受伤持续时间
        /// </summary>
        protected float _hitDuration;

        public AnimalHitState(StateMachine stateMachine, AnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _hitDuration = 3f;

            animalBehavior.AnimalAnimator.PlayHit();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if (stateTimer >= _hitDuration)
                stateMachine.ExitTempState();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

