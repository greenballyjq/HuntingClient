using Hunting.Game.Animal;
using UnityEngine;


namespace Hunting.Game.Animal.State
{
    /// <summary>
    /// 动物被击中状态
    /// </summary>
    public class AnimalHitState : AnimalState
    {
        /// <summary>
        /// 受伤状态持续时间
        /// </summary>
        private float hitDuration = 0.5f;

        public AnimalHitState(AnimalBehavior animal, StateMachine stateMachine, string animName) : base(animal, stateMachine, animName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // 设置状态计时器
            stateTimer = hitDuration;
            animal.PlayHitAudio();
        }

        public override void Update()
        {
            base.Update();
            // 受伤状态结束后自动回到移动状态
            if (stateTimer <= 0f)
                stateMachine.ChangeState(animal.moveState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}