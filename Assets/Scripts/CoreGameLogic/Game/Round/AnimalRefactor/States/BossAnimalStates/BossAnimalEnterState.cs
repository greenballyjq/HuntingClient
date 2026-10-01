using Hunting.Game.Animal;
using UnityEngine;

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
            animalBehavior.Moveable.StopMove();
            ((BossAnimalVisual)animalBehavior.AnimalVisual).PlayEnter();
        }

        public override void Exit()
        {
            animalBehavior.Moveable.StartMove();
            base.Exit();
        }
    }
}