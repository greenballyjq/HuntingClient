using cfg.HuntingConfig;
using CoreGameLogic.Game.Round.Boss.States;
using GameFramework.Manager;
using Hunting.Game.Animal;

public class BossAnimalBehaviour : BaseAnimalBehaviour
{
    private BossAnimalDeathState _bossDeathState;
    private BossAnimalCallGuardState _bossCallGuardState;
    private BossAnimalEnterState _bossEnterState;
    
    private int _callGuardTimerId;
    private const float CALL_GUARD_DURATION = 30f;

    private TimerManager _timerManager => GameServiceLocator.TimerManager;
    
    public int CalledGuardCount { get; set; }
    
    public override void Init(Specie data)
    {
        base.Init(data);
        _bossDeathState = new BossAnimalDeathState(_stateMachine, this);
        _bossEnterState = new BossAnimalEnterState(_stateMachine, this);
        _bossCallGuardState = new BossAnimalCallGuardState(_stateMachine, this);

        _callGuardTimerId = _timerManager.StartTimer(CALL_GUARD_DURATION, CallGuard, repeat: TimerManager.LOOP);

        _stateMachine.Init(_bossEnterState);
    }

    private void CallGuard()
    {
        _stateMachine.EnterTempState(_bossCallGuardState);
    }

    protected override void OnDeath()
    {
        _stateMachine.ChangeState(_bossDeathState);

        var spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();
        var autoSpawners = spawnerManager.GetSpawners<AutoSpawner>("Random");
        autoSpawners.ForEach(spawner => spawner.SetEnabled(false));

        _timerManager.StopTimer(_callGuardTimerId);
    }

    public void EnterCombat()
    {
        _stateMachine.ChangeState(MoveState);
    }
}