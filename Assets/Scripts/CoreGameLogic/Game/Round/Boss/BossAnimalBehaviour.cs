using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Events;
using Hunting.Game.Animal;

public class BossAnimalBehaviour : BaseAnimalBehaviour
{
    private BossAnimalDeathState _bossDeathState;
    
    private HiddenMapSpawner[] _spawners;
    
    private int _perAnimalSpawnCount;
    private float _animalSpawnInterval;
    private float _perAnimalSpawnInterval;

    private TimerManager _timerManager => GameServiceLocator.TimerManager;

    private int _spawnedAnimalTimerId;
    
    public override void Init(Specie data)
    {
        base.Init(data);

        _bossDeathState = new BossAnimalDeathState(_stateMachine, this);
        
        SetRandomSpawnParam();
        _perAnimalSpawnInterval = 0.4f;
        
        Health.OnDamaged += OnDamaged;
        Health.OnDeath += OnDeath;

        _spawnedAnimalTimerId = _timerManager.StartTimer(_animalSpawnInterval, SpawnAnimal, repeat: TimerManager.LOOP);

        _spawners = FindObjectsOfType<HiddenMapSpawner>();
    }

    private void SetRandomSpawnParam()
    {
        _animalSpawnInterval = UnityEngine.Random.Range(7f, 10f);
        _perAnimalSpawnCount = UnityEngine.Random.Range(6, 8);
    }

    protected override void OnDeath()
    {
        _stateMachine.ChangeState(_bossDeathState);
        _timerManager.StopTimer(_spawnedAnimalTimerId);
    }

    private async void SpawnAnimal()
    {
        int spawnedCount = 0;
        while (spawnedCount < _perAnimalSpawnCount)
        {
            var spawner = _spawners[UnityEngine.Random.Range(0, _spawners.Length)];
            await spawner.SpawnAsync();
            spawnedCount++;
            await UniTask.Delay((int)(_perAnimalSpawnInterval * 1000));
        }
        
        SetRandomSpawnParam();
    }
}