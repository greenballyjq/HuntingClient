using System.Collections.Generic;
using Hunting.Game.Animal;

namespace CoreGameLogic.Game.Round.Boss.States
{
    public class BossAnimalCallGuardState : AnimalState
    {
        private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();
        
        private List<BaseAnimalBehaviour> _calledAnimals;
        
        public BossAnimalCallGuardState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(
            stateMachine, animalBehavior)
        {
            
        }

        public override void Enter()
        {
            ((BossAnimalVisual)animalBehavior.AnimalVisual).PlayCall();
            stateTimer = 0f;
            
            _calledAnimals = _animalManager.GetCloseAnimalsFromTargetPosition(animalBehavior.transform.position, 10);

            _calledAnimals?.ForEach(animal =>
            {
                // if (animal.Moveable.MovePolicy is GuardMovePolicy) return;
                animal.Moveable.ChangeMovePolicy(MovePolicyType.Guard);
                // bossAnimalBehaviour.CalledGuardCount++;
            });
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (stateTimer > 20f)
            {
                stateMachine.ExitTempState();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _calledAnimals?.ForEach(animal =>
            {
                animal.Moveable.ChangeMovePolicy(MovePolicyType.Linear);
            });
        }
    }
}