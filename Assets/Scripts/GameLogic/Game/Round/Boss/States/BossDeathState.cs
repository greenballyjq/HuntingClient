/// <summary>
/// Boss 死亡状态
/// </summary>
public class BossDeathState : BossState
{
    public BossDeathState(BossBehaviour boss, StateMachine stateMachine, string animationName) : base(boss, stateMachine, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        boss.TriggerBossDiedEvent();
    }
}