using cfg.HuntingConfig;
using CoreGameLogic.Game.Round.Boss.States;
using GameFramework.Manager;
using Hunting.Game.Animal;

public class BossAnimalBehaviour : BaseAnimalBehaviour
{
    private BossAnimalDeathState _bossDeathState;
    private BossAnimalCallGuardState _bossCallGuardState;
    
    private int _callGuardTimerId;
    private const float CALL_GUARD_DURATION = 30f;

    private TimerManager _timerManager => GameServiceLocator.TimerManager;
    
    public int CalledGuardCount { get; set; }
    
    public override void Init(Specie data)
    {
        _bossCallGuardState = new BossAnimalCallGuardState(_stateMachine, this);
        
        base.Init(data);
        _bossDeathState = new BossAnimalDeathState(_stateMachine, this);
        
        Health.OnDamaged += OnDamaged;
        Health.OnDeath += OnDeath;

        _callGuardTimerId = _timerManager.StartTimer(CALL_GUARD_DURATION, CallGuard, repeat: TimerManager.LOOP);
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
    }
}