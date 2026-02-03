using System.Diagnostics;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物逃跑状态
    /// </summary>
    public class AnimalFleeState : AnimalState
    {
        /// <summary>
        /// 逃跑移动速率
        /// </summary>
        protected float _fleeMoveSpeedRate = 2f;

        public AnimalFleeState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animalBehavior.Moveable.SetMoveRate(_fleeMoveSpeedRate);
            
            (animalBehavior.AnimalVisual as FleeAnimalVisual).PlayFlee();
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

            animalBehavior.Moveable.SetMoveRate(_fleeMoveSpeedRate);

            (animalBehavior.AnimalVisual as FleeAnimalVisual).PlayFlee();
        }
    }
}

