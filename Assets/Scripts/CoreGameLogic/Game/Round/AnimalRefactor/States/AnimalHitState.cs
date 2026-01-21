namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物受击状态
    /// </summary>
    public class AnimalHitState : AnimalState, IStayState
    {
        /// <summary>
        /// 受伤持续时间
        /// </summary>
        private float _hitDuration = 3f;
        
        /// <summary>
        /// 受伤速度倍率
        /// </summary>
        private const float HitSpeedRate = 0f;

        public AnimalHitState(StateMachine stateMachine, AnimalBehavior animal) : base(stateMachine, animal)
        {
        }

        public override void Enter()
        {
            base.Enter();       
            
            // 设置受伤速度倍率
            animal.Moveable.SetMoveRate(HitSpeedRate);
            
            // 播放受伤动画
            animal.AnimalAnimator.PlayHit();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if (stateTimer >= _hitDuration)
                stateMachine.ExitTempState();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

