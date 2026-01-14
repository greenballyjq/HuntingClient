using UnityEngine;

/// <summary>
/// Boss 死亡状态
/// </summary>
public class BossDeathState : BossState
{
    private readonly float _deathDuration = 3f;
    private bool _hasPlayedDeathEffect = false;
    public BossDeathState(BossBehaviour boss, StateMachine stateMachine, string animationName) : base(boss, stateMachine, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        boss.TriggerBossDyingEvent();
        stateTimer = _deathDuration;
        _hasPlayedDeathEffect = false;
    }

    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        if (!_hasPlayedDeathEffect && stateTimer < 2f)
        {
            boss.PlayBossDeathEffect();
            _hasPlayedDeathEffect = true;
        }

        if (stateTimer <= 1f)
        {
            boss.TriggerBossDiedEvent();
        }
    }
}