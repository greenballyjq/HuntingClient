using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Game.Animal.State 
{
    /// <summary>
    /// 动物死亡状态
    /// </summary>
    public class AnimalDeathState : AnimalState
    {
        public AnimalDeathState(AnimalBehavior animal, StateMachine stateMachine, string animName) : base(animal, stateMachine, animName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            stateTimer = 1f;

            animal.HandleDeathDrop();
        }

        public override void Update()
        {
            base.Update();

            if (stateTimer <= 0f)
                Object.Destroy(animal.gameObject);
        }
    }
}
