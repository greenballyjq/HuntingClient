using UnityEngine;

public class AnimalIdleState : AnimalState
{
    public AnimalIdleState(AnimalBehavior animal, StateMachine stateMachine, string animationName) : base(animal,
        stateMachine, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //animal.RVO.SetMaxSpeed(0f);
        // 设置速度为0（待机不动）
        animal.ApplySpeedMultiplier(0f);
        stateTimer = 0f;
    }

    public override void Update()
    {
        base.Update();
        stateTimer += Time.deltaTime;
        if (stateTimer > 1f)
        {
            stateMachine.ChangeState(animal.GetGuardState());
        }
    }
}