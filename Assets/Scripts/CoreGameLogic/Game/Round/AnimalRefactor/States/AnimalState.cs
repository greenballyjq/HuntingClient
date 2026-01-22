namespace Hunting.Game.Animal
{
    public class AnimalState : IState
    {
        /// <summary>
        /// 状态机
        /// </summary>
        protected StateMachine stateMachine;

        /// <summary>
        /// 动物基类
        /// </summary>
        protected AnimalBehaviour animalBehavior;

        /// <summary>
        /// 状态计时器
        /// </summary>
        protected float stateTimer;
        
        /// <summary>
        /// 是否暂停
        /// </summary>
        protected bool _isPaused;

        public AnimalState(StateMachine stateMachine,AnimalBehaviour animalBehavior)
        {
            this.animalBehavior = animalBehavior;
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

            animalBehavior.AnimalVisual.DoUpdate(dt);
        }

        public virtual void Exit(){}

        public virtual void Pause()
        {
            _isPaused = true;
        }

        public virtual void Resume()
        {
            _isPaused = false;
        }

        protected void ChangeState(IState next)
        {
            stateMachine.ChangeState(next);
        }
    }
}