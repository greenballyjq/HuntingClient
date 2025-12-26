using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Game.Animal.State
{
    /// <summary>
    /// 动物逃跑状态
    /// </summary>
    public class AnimalFleeState : AnimalState
    {
        /// <summary>
        /// 逃跑持续时间
        /// </summary>
        private float _fleeDuration = 20f;

        public AnimalFleeState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
            : base(animal, stateMachine, animationName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // 应用逃跑加速倍率
            animal.ApplySpeedMultiplier(animal.FleeSpeedMultiplier);

            // 第一次进入逃跑，设置完整时间；从受击状态返回，使用剩余时间
            if (animal.FleeTimeRemaining <= 0)
            {
                stateTimer = _fleeDuration;
                animal.FleeTimeRemaining = _fleeDuration;
            }
            else
            {
                stateTimer = animal.FleeTimeRemaining;
            }
            
            // 标记为逃跑状态
            animal.SetFleeing(true);
        }

        public override void Update()
        {
            base.Update();

            // 状态计时器倒计时
            stateTimer -= Time.deltaTime;
            
            // 持续更新剩余时间
            animal.FleeTimeRemaining = stateTimer;

            // 受击判断（逃跑中可以被击中）
            if (animal.IsHit)
            {
                stateMachine.ChangeState(animal.GetHitState());
                animal.ResetHitFlag();
                return;
            }

            // 逃跑时间到，触发离场事件
            if (stateTimer <= 0f)
            {
                animal.TriggerAnimalFled();
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            // 如果不是因为离场退出，保留剩余时间供下次恢复
            if (stateTimer <= 0)
            {
                // 逃跑完成，清零并取消逃跑标志
                animal.FleeTimeRemaining = 0;
                animal.SetFleeing(false);
            }
            
            // 恢复正常速度
            animal.ApplySpeedMultiplier(1f);
        }
    }
}