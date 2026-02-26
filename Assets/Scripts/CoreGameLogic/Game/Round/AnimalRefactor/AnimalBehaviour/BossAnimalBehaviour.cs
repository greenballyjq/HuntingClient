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

    private const float MAX_SPAWN_ANIMAL = 30;

    private TimerManager _timerManager => GameServiceLocator.TimerManager;
    
    private EventManager _eventManager => GameServiceLocator.EventManager;
    
    public int CalledGuardCount { get; set; }
    
    public override void Init(Specie data)
    {
        base.Init(data);
        _bossDeathState = new BossAnimalDeathState(_stateMachine, this);
        _bossEnterState = new BossAnimalEnterState(_stateMachine, this);
        _bossCallGuardState = new BossAnimalCallGuardState(_stateMachine, this);
        
        _eventManager.AddListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
        
        Health.SetHealth(1000);

        _callGuardTimerId = _timerManager.StartTimer(CALL_GUARD_DURATION, CallGuard, repeat: TimerManager.LOOP);
    }
    
    public void EnterCombat()
    {
        _stateMachine.ChangeState(MoveState);
    }
    
    private void OnAnimalGenerated(AnimalGeneratedEventArgs args)
    {
        AnimalManager animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        if (animalManager.GetActiveAnimalCount() >= MAX_SPAWN_ANIMAL)
        {
            DisableAnimalSpawners();
        }
    }
    
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        AnimalManager animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        if (animalManager.GetActiveAnimalCount() < MAX_SPAWN_ANIMAL)
        {
            EnableAnimalSpawners();
        }
    }

    protected override void InitStateMachine()
    {
        _stateMachine.ChangeState(_bossEnterState);
    }

    private void CallGuard()
    {
        _stateMachine.EnterTempState(_bossCallGuardState);
    }

    protected override void OnDamaged()
    {
        (AnimalEventTrigger as  BossAnimalEventTrigger).TriggerBossDamaged(Health.MaxHealth, Health.CurrentHealth);

        if (_stateMachine.CurrentState == _bossCallGuardState)
        {
            return;
        }
        
        base.OnDamaged();
    }

    protected override void OnDeath()
    {
        _stateMachine.ChangeState(_bossDeathState);

        DisableAnimalSpawners();

        _timerManager.StopTimer(_callGuardTimerId);
        
        _eventManager.RemoveListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
    }
    
    private void EnableAnimalSpawners()
    {
        var spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();
        var autoSpawners = spawnerManager.GetSpawners<AutoSpawner>("Random");
        autoSpawners.ForEach(spawner => spawner.SetEnabled(true));
    }

    private void DisableAnimalSpawners()
    {
        var spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();
        var autoSpawners = spawnerManager.GetSpawners<AutoSpawner>("Random");
        autoSpawners.ForEach(spawner => spawner.SetEnabled(false));
    }
}