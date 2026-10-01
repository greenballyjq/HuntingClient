using cfg.HuntingConfig;
using CoreGameLogic.Game.Round.Boss.States;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using System;
using System.Threading;

public class BossAnimalBehaviour : BaseAnimalBehaviour
{
    private BossAnimalDeathState _bossDeathState;
    private BossAnimalCallGuardState _bossCallGuardState;
    private BossAnimalEnterState _bossEnterState;
    
    private CancellationTokenSource _callGuardCts;
    private const float CALL_GUARD_DURATION = 30f;

    private const float MAX_SPAWN_ANIMAL = 30;

    private EventManager _eventManager;
    private AnimalManager _animalManager;
    private SpawnerManager _spawnerManager;
    
    public int CalledGuardCount { get; set; }
    
    public override void Init(Specie data)
    {
        _eventManager = GameServiceLocator.EventManager;
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        _spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();

        _bossDeathState = new BossAnimalDeathState(_stateMachine, this);
        _bossEnterState = new BossAnimalEnterState(_stateMachine, this);
        _bossCallGuardState = new BossAnimalCallGuardState(_stateMachine, this);

        _eventManager.AddListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);

        _callGuardCts = new CancellationTokenSource();
        RunCallGuardLoop(_callGuardCts.Token).Forget();

        base.Init(data);
        
    }

    protected override void InitStateMachine()
    {
        _stateMachine.ChangeState(_bossEnterState);
    }

    public void EnterCombat()
    {
        _stateMachine.ChangeState(MoveState);
    }
    
    private void OnAnimalGenerated(AnimalGeneratedEventArgs args)
    {
        if (_animalManager.GetAliveAnimalCount() >= MAX_SPAWN_ANIMAL)
        {
            DisableAnimalSpawners();
        }
    }
    
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (_animalManager.GetAliveAnimalCount() < MAX_SPAWN_ANIMAL)
        {
            EnableAnimalSpawners();
        }
    }

    

    private async UniTaskVoid RunCallGuardLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(CALL_GUARD_DURATION), cancellationToken: token);
                if (!token.IsCancellationRequested)
                    CallGuard();
            }
        }
        catch (OperationCanceledException)
        {
        }
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

        if (_callGuardCts != null)
        {
            _callGuardCts.Cancel();
            _callGuardCts.Dispose();
            _callGuardCts = null;
        }
        
        _eventManager.RemoveListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }
    
    private void EnableAnimalSpawners()
    {
        var autoSpawners = _spawnerManager.GetSpawners<AutoSpawner>("Random");
        autoSpawners.ForEach(spawner => spawner.SetEnabled(true));
    }

    private void DisableAnimalSpawners()
    {
        var autoSpawners = _spawnerManager.GetSpawners<AutoSpawner>("Random");
        autoSpawners.ForEach(spawner => spawner.SetEnabled(false));
    }
}