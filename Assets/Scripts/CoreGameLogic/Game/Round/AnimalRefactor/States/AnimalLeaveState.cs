
namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物离场状态
    /// </summary>
    public class AnimalLeaveState : AnimalState
    {
        /// <summary>
        /// 离场移动速率
        /// </summary>
        protected float _leaveMoveSpeedRate = 2f;

        public AnimalLeaveState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
        {
        }

        public override void Enter()
        {
            base.Enter();

            animalBehavior.Moveable.SetMoveRate(_leaveMoveSpeedRate);
            
            (animalBehavior.AnimalVisual as LeaveAnimalVisual).PlayLeave();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            animalBehavior.Moveable.DoUpdate(dt);
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        public override void Resume()
        {
            base.Resume();

            animalBehavior.Moveable.SetMoveRate(_leaveMoveSpeedRate);

            (animalBehavior.AnimalVisual as LeaveAnimalVisual).PlayLeave();
        }
    }
}

