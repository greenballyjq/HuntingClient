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

            // 禁用碰撞体
            if (animal.TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }

            // 停止所有物理运动
            if (animal.rb != null)
            {
                animal.rb.velocity = Vector3.zero;
                animal.rb.angularVelocity = Vector3.zero;
                animal.rb.isKinematic = true; // 直接设为运动学，不受物理影响
            }

            // 停止移动
            animal.moveSpeed = 0f;

            // 2秒后销毁
            stateTimer = 2f;

            Debug.Log($"[AnimalDeathState] {animal.specieData.Name} 死亡，停止所有运动");

            // 处理掉落
            animal.HandleDeathDrop();
        }

        public override void Update()
        {
            base.Update();

            // 2秒后销毁
            if (stateTimer <= 0f)
            {
                Object.Destroy(animal.gameObject);
            }
        }
    }
}
