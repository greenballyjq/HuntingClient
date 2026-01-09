namespace GameLogic.Game.Round.Boss.States
{
    /// <summary>
    /// Boss状态基类
    /// </summary>
    public class BossState : IState
    {
        /// <summary>
        /// 状态机引用
        /// </summary>
        protected StateMachine stateMachine;

        /// <summary>
        /// 动画名称
        /// </summary>
        protected string animationName;

        /// <summary>
        /// Boos Owner 引用
        /// </summary>
        protected BossBehaviour boss;

        /// <summary>
        /// 状态计时器（子类按需使用） 
        /// </summary>
        protected float stateTimer;

        public BossState(BossBehaviour boss, StateMachine stateMachine, string animationName)
        {
            this.boss = boss;
            this.stateMachine = stateMachine;
            this.animationName = animationName;
        }
        
        public virtual void Enter()
        {
            stateTimer = 0f;
            boss.PlayAnimationBool(animationName);
        }

        public virtual void Update()
        {
            if (boss.CurrentHP <= 0 && !(this is BossDeathState))
            {
                stateMachine.ChangeState(boss.GetDeathState());
            }
        }

        public virtual void Exit()
        {
            boss.StopAnimation(animationName);
        }
    }
}