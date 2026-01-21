using UnityEngine;

namespace Hunting.Game.Animal
{
    public class AnimalState : IState
    {
        /// <summary>
        /// 状态机引用
        /// </summary>
        protected StateMachine stateMachine;

        /// <summary>
        /// 动物引用
        /// </summary>
        protected AnimalBehavior animal;

        /// <summary>
        /// 状态计时器
        /// </summary>
        protected float stateTimer;
        
        /// <summary>
        /// 是否暂停
        /// </summary>
        protected bool _isPaused;

        public AnimalState(StateMachine stateMachine,AnimalBehavior animal)
        {
            this.animal = animal;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter()
        {
            stateTimer = 0;
            _isPaused = false;
        }

        public virtual void DoUpdate(float dt)
        {
            if (!_isPaused)
                stateTimer += dt;
        }

        public virtual void Exit(){}

        protected void ChangeState(IState next)
        {
            stateMachine.ChangeState(next);
        }

        public virtual void Pause()
        {
            _isPaused = true;
        }

        public virtual void Resume()
        {
            _isPaused = false;
        }
    }
}