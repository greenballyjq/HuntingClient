using Hunting.Game.Animal;

/// <summary>
/// 动物入场状态
/// </summary>
public class AnimalEnterState : AnimalState
{
    public AnimalEnterState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
