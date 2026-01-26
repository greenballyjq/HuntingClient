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

        public AnimalHitState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            animalBehavior.AnimalVisual.PlayHit();
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

