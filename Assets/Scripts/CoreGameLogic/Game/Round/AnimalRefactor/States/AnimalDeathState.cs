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
        protected float _deathDuration ;

        /// <summary>
        /// 动物掉落奖励是否触发
        /// </summary>
        protected bool _isAnimalDropRewardTriggered;


        public AnimalDeathState(StateMachine stateMachine, AnimalBehaviour animal) : base(stateMachine, animal)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _deathDuration = 7f;

            _isAnimalDropRewardTriggered = false;

            animalBehavior.AnimalAnimator.PlayDeath();

            animalBehavior.Moveable.StopMove();

            animalBehavior.Collider.enabled = false;

            animalBehavior.TriggerAnimalDying();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if(stateTimer >= 4f && !_isAnimalDropRewardTriggered){
                animalBehavior.TriggerAnimalDropReward();
                animalBehavior.PlayDeathEffect();
                _isAnimalDropRewardTriggered = true;
            }

            if (stateTimer >= _deathDuration)
                animalBehavior.TriggerAnimalDied();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

