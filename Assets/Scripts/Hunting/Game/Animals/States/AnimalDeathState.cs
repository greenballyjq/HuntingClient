using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Game.Animal.State 
{
    /// <summary>
    /// 动物死亡状态
    /// </summary>
    public class AnimalDeathState : AnimalState
    {
        /// <summary>
        /// 死亡动画持续时间
        /// </summary>
        private float _deathDuration = 1f;

        public AnimalDeathState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
            : base(animal, stateMachine, animationName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // 设置死亡计时器
            stateTimer = _deathDuration;

            // 停止移动并释放RVO
            animal.ReleaseRVO();

            // 禁用碰撞体
            animal.SetColliderEnabled(false);
        }

        public override void Update()
        {
            base.Update();

            // 状态计时器倒计时
            stateTimer -= Time.deltaTime;

            // 触发死亡事件
            if (stateTimer <= 0f)
                animal.TriggerAnimalDied();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
