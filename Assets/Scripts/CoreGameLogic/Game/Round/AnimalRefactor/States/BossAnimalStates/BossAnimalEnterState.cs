using Hunting.Game.Animal;

namespace CoreGameLogic.Game.Round.Boss.States
{
    public class BossAnimalEnterState : AnimalState
    {
        public BossAnimalEnterState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            ((BossAnimalVisual)animalBehavior.AnimalVisual).PlayEnter();
        }
    }
}