using System.Collections.Generic;
using cfg.HuntingConfig;
using CoreGameLogic.Game.Round.Boss.States;
using Hunting.Game.Animal;

public class BossAnimalBehaviour : BaseAnimalBehaviour
{
    private BossAnimalDeathState _bossDeathState;
    private BossAnimalCallGuardState _bossCallGuardState;
    
    private int _spawnedAnimalTimerId;

    private List<ManualSpawner> _manualSpawners;
    
    private SpawnerManager _spawnerManager => GameServiceLocator.GetRoundManager<SpawnerManager>();
    
    public override void Init(Specie data)
    {
        _bossCallGuardState = new BossAnimalCallGuardState(_stateMachine, this, _manualSpawners);
        
        base.Init(data);
        _bossDeathState = new BossAnimalDeathState(_stateMachine, this);
        
        Health.OnDamaged += OnDamaged;
        Health.OnDeath += OnDeath;

        _manualSpawners = _spawnerManager.GetSpawners<ManualSpawner>("Random");
    }

    // protected override void InitState()
    // {
    //     // _stateMachine.Init(_bossCallGuardState);
    // }

    protected override void OnDeath()
    {
        _stateMachine.ChangeState(_bossDeathState);
    }
}