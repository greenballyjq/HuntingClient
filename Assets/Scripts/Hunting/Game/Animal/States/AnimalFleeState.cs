using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Game.Animal.State
{
    /// <summary>
    /// 动物逃跑状态
    /// </summary>
    public class AnimalFleeState : AnimalState
    {
        public AnimalFleeState(AnimalBehavior animal, StateMachine stateMachine, string animName) : base(animal, stateMachine, animName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // 速度翻倍
            animal.currentMoveSpeed *= 2f;

            // 10秒后销毁
            stateTimer = 20f;

            Debug.LogWarning($"[AnimalFleeState] {animal.specieData.Name} 逃跑，速度: {animal.currentMoveSpeed}");
        }

        public override void Update()
        {
            base.Update();

            // 逃跑状态下移动
            animal.transform.Translate(animal.transform.forward * animal.currentMoveSpeed * Time.deltaTime, Space.World);

            // 20秒后销毁
            if (stateTimer <= 0f)
            {
                Object.Destroy(animal.gameObject);
            }
        }
    }
}