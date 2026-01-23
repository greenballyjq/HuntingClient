using CoreGameLogic.Game.Round.Animals.Spawners;
using Hunting.Game.Animal;

namespace CoreGameLogic.Game.Round.Boss.States
{
    public class BossAnimalCallGuardState : AnimalState
    {
        private GuardAnimalSpawner[] _guardSpawners;
        
        public BossAnimalCallGuardState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(
            stateMachine, animalBehavior)
        {
            
        }

        public override void Enter()
        {
            
        }
    }
}