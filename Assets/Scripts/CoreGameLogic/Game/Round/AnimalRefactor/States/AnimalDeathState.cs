namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物死亡状态
    /// </summary>
    public class AnimalDeathState : AnimalState
    {
        /// <summary>
        /// 死亡持续时间
        /// </summary>
        private float _deathDuration = 7f;

        /// <summary>
        /// 动物掉落奖励是否触发
        /// </summary>
        private bool _isAnimalDropRewardTriggered;


        public AnimalDeathState(StateMachine stateMachine, AnimalBehavior animal) : base(stateMachine, animal)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animal.AnimalAnimator.PlayDeath();

            animal.Moveable.StopMove();

            animal.Collider.enabled = false;

            animal.TriggerAnimalDying();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if(stateTimer >= 4f && !_isAnimalDropRewardTriggered){
                animal.TriggerAnimalDropReward();
                animal.PlayDeathEffect();
                _isAnimalDropRewardTriggered = true;
            }

            if (stateTimer >= _deathDuration)
                animal.TriggerAnimalDied();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

