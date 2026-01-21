using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// Boss进入状态
/// </summary>
public class BossEnterState : BossState
{
    private float _enterDuration;

    public BossEnterState(BossBehaviour boss, StateMachine stateMachine, string animationName) : base(boss, stateMachine, animationName)
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
    }
}

