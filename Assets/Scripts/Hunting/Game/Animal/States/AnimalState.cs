using System.Collections;
using System.Collections.Generic;
using Hunting.Game.Animal;
using UnityEngine;


namespace Hunting.Game.Animal.State
{
    /// <summary>
    /// 动物状态
    /// </summary>
    public class AnimalState : IState
    {
        /// <summary>
        /// 状态机
        /// </summary>
        protected StateMachine stateMachine;

        /// <summary>
        /// 状态动画名称
        /// </summary>
        protected string animName;

        /// <summary>
        /// 动物自身
        /// </summary>
        protected AnimalBehavior animal;

        /// <summary>
        /// 状态计时器
        /// </summary>
        protected float stateTimer;

        /// <summary>
        /// 动画组件
        /// </summary>
        protected Animator animator;

        /// <summary>
        /// 移动避障组件
        /// </summary>
        protected RVOMovement rvo;

        public AnimalState(AnimalBehavior animal, StateMachine stateMachine, string animName)
        {
            this.animal = animal;
            this.stateMachine = stateMachine;
            this.animName = animName;

            // 获取animal上的组件，方便使用
            animator = animal.animator;
            rvo = animal.rvo;
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public virtual void Enter()
        {
            animator.SetBool(animName, true);
        }

        /// <summary>
        /// 退出状态
        /// </summary>

        public virtual void Exit()
        {
            animator.SetBool(animName, false);
        }

        /// <summary>
        /// 状态更新
        /// </summary>

        public virtual void Update()
        {
            // 当前状态倒计时
            stateTimer -= Time.deltaTime;

            // 任何状态都可以进入逃跑状态（除了死亡和逃跑状态本身）
            if (animal.TimeInScene >= animal.StayTime &&
                !(this is AnimalDeathState) &&
                !(this is AnimalFleeState))
            {
                //stateMachine.ChangeState(animal.fleeState);
            }
        }


    }


}