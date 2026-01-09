using UnityEngine;

/// <summary>
/// Boss Idle状态
/// </summary>
public class BossIdleState : BossState
{
    public BossIdleState(BossBehaviour boss, StateMachine stateMachine, string animationName) : base(boss, stateMachine, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0f;
    }

    public override void Update()
    {
        base.Update();
        
        stateTimer += Time.deltaTime;
        if (stateTimer > boss.StayTime)
        {
            stateMachine.ChangeState(boss.GetRandomMoveState());
        }
    }
} 