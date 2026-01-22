using Hunting.Game.Animal;
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

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);
        
        stateTimer += Time.deltaTime;
        if (stateTimer > 2f)
        {
            stateMachine.ChangeState(boss.GetRandomMoveState());
        }
    }
} 