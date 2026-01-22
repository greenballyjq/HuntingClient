using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// Boss 受击状态
/// </summary>
public class BossHitState : BossState
{
    private float _hitDuration;
    
    public BossHitState(BossBehaviour boss, StateMachine stateMachine, string animationName) : base(boss, stateMachine, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _hitDuration = boss.GetAnimationLength("Hit");
        Debug.Log($"[{GetType().Name}] HitDuration: {_hitDuration}");
        stateTimer = 0f;
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);
        
        stateTimer += Time.deltaTime;
        if (stateTimer >= _hitDuration)
        {
            stateMachine.ChangeState(boss.GetRandomMoveState());
        }
    }
}