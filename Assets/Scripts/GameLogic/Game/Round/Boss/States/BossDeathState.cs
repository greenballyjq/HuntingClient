namespace GameLogic.Game.Round.Boss.States
{
    /// <summary>
    /// Boss 死亡状态
    /// </summary>
    public class BossDeathState : BossState
    {
        public BossDeathState(BossBehaviour boss, StateMachine stateMachine, string animationName) : base(boss, stateMachine, animationName)
        {
            
        }
        
        
    }
}