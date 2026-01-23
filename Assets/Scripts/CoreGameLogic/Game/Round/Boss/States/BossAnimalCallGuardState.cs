using System.Collections.Generic;
using CoreGameLogic.Game.Round.Animals.Spawners;
using Hunting.Game.Animal;

namespace CoreGameLogic.Game.Round.Boss.States
{
    public class BossAnimalCallGuardState : AnimalState
    {
        private List<ManualSpawner> _manualSpawners;
        
        public BossAnimalCallGuardState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior, List<ManualSpawner> manualSpawners) : base(
            stateMachine, animalBehavior)
        {
            _manualSpawners = manualSpawners;
        }

        public override void Enter()
        {
            _manualSpawners.ForEach(spawner => spawner.SetMovePolicyType(MovePolicyType.Guard));
            _manualSpawners.ForEach(spawner => spawner.Spawn());
        }

        public override void Exit()
        {
            base.Exit();
            _manualSpawners.ForEach(spawner => spawner.SetMovePolicyType(MovePolicyType.Linear));
        }
    }
}