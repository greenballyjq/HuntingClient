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
        protected float _fleeSpeedRate = 2f;
        
        /// <summary>
        /// 逃跑持续时间
        /// </summary>
        protected float _fleeDuration = 8f;

        public AnimalFleeState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animalBehavior.Moveable.SetMoveRate(_fleeSpeedRate);
            
            (animalBehavior.AnimalVisual as FleeAnimalVisual).PlayFlee();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            animalBehavior.Moveable.DoUpdate(dt);

            // 检查是否到达逃跑持续时间
            if (stateTimer >= _fleeDuration)
            {
                //(animalBehavior.AnimalEventTrigger as FleeAnimalEventTrigger).TriggerAnimalFled();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        public override void Resume()
        {
            base.Resume();
            
            (animalBehavior.AnimalVisual as FleeAnimalVisual).PlayFlee();
        }
    }
}

