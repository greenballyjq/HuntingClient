using UnityEngine;

/// <summary>
/// 动物移动状态
/// </summary>
public class AnimalMoveState : AnimalState
{
    public AnimalMoveState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
        : base(animal, stateMachine, animationName) 
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // 确保正常速度
        animal.ApplySpeedMultiplier(1f);
        
        // 设置移动方向
        animal.SetDirection(animal.CurrentDirection);
    }

    public override void Update()
    {
        base.Update();

        // 受击判断
        if (animal.IsHit)
        {
            animal.ResetHitFlag();
            stateMachine.ChangeState(animal.GetHitState());
            return;
        }

        // 驻场时间到，进入逃跑
        if (animal.TimeInScene >= animal.StayTime)
        {
            stateMachine.ChangeState(animal.GetFleeState());
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}