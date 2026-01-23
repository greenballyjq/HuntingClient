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
        protected float _deathDuration = 7f;

        /// <summary>
        /// 动物掉落奖励是否触发
        /// </summary>
        protected bool _isAnimalDropRewardTriggered = false;


        public AnimalDeathState(StateMachine stateMachine, BaseAnimalBehaviour animal) : base(stateMachine, animal)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animalBehavior.Moveable.StopMove();

            animalBehavior.Collider.enabled = false;

            animalBehavior.AnimalVisual.PlayDeath();

            animalBehavior.AnimalEventTrigger.TriggerAnimalEnteredDeath();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            HandleDeathExecution();
        }

        protected virtual void HandleDeathExecution()
        {
            if(stateTimer >= 4f && !_isAnimalDropRewardTriggered){
                animalBehavior.AnimalEventTrigger.TriggerAnimalDropReward();
                animalBehavior.AnimalVisual.PlayDeathEffect();
                _isAnimalDropRewardTriggered = true;
            }

            if (stateTimer >= _deathDuration)
                animalBehavior.AnimalEventTrigger.TriggerAnimalDied();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

