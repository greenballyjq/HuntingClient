namespace CoreGameLogic.Game.Round.Animals.States
{
    public class AnimalIdleState : AnimalState
    {
        public AnimalIdleState(AnimalBehavior animal, StateMachine stateMachine, string animationName) : base(animal, stateMachine, animationName)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            animal.RVO.SetMaxSpeed(0f);
        }
    }
}