namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物逃跑状态
    /// </summary>
    public class AnimalFleeState : AnimalState
    {
        /// <summary>
        /// 逃跑速度倍率
        /// </summary>
        private const float FleeSpeedRate = 2f;
        
        /// <summary>
        /// 逃跑持续时间
        /// </summary>
        private const float FleeDuration = 8f;

        public AnimalFleeState(StateMachine stateMachine, AnimalBehavior animal) : base(stateMachine, animal)
        {
        }

        public override void Enter()
        {
            base.Enter();
           
            // 设置逃跑速度倍率
            animal.Moveable.SetMoveRate(FleeSpeedRate);
            
            // 播放逃跑动画
            animal.AnimalAnimator.PlayFlee();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            
            // 检查是否到达逃跑持续时间
            if (stateTimer >= FleeDuration)
            {
                // 暂时什么都不做
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        public override void Resume()
        {
            base.Resume();
            
            animal.AnimalAnimator.PlayFlee();
        }
    }
}

