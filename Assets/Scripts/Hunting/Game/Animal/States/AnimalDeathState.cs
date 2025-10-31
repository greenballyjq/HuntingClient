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

            Collider collider = animal.GetComponent<Collider>();
            if (collider != null)
            {
                GameObject.Destroy(collider);
            }
            
        }

        public override void Update()
        {
            base.Update();

            if (stateTimer <= 0f)
                Object.Destroy(animal.gameObject);
        }
    }
}
