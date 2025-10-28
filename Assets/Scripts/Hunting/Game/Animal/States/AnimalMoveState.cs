namespace Hunting.Game.Animal.State
{
    /// <summary>
    /// 动物移动状态
    /// </summary>
    public class AnimalMoveState : AnimalState
    {
        public AnimalMoveState(AnimalBehavior animal, StateMachine stateMachine, string animName) : base(animal, stateMachine, animName) { }

        public override void Enter()
        {
            base.Enter();            
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
 
    }
}