using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HiddenMapSpawner : BaseSpawner
{
    /// <summary>
    /// 每批派发数量
    /// </summary>
    [SerializeField] private int perSpawnCount;

    /// <summary>
    /// 单批派发间隔
    /// </summary>
    [SerializeField] private float perSpawnInterval;

    // public void DoUpdate(float dt)
    // {
    //     _accumulatedTime += dt;
    //
    //     if (_accumulatedTime >= spawnInterval)
    //     {
    //         var (specie, stayTime) = _configManager.GetRandomSpecieForMap(_mapData.ID);
    //         SpawnAsync(specie, stayTime).Forget();
    //         _accumulatedTime -= spawnInterval;
    //     }
    // }

    public override async UniTask<AnimalBehavior> SpawnAsync()
    {
        // 从配置按地图与体型策略选出物种与驻场时间
        // var (specie, stayTime) = _configManager.GetRandomSpecieForMap(_mapData.ID);

        var specie = _configManager.GetRandomBossFollow();

        // 计算生成位置与移动方向
        Vector3 spawnPosition = CalculateSpawnPosition();
        Vector3 moveDirection = CalculateMoveDirection();

        // 调用动物管理器生成动物
        _animalManager.GenerateAnimalAsync(specie, spawnPosition, moveDirection, 10f, true).Forget();
        // await UniTask.Delay((int)(perSpawnInterval * 1000));
        
        return null;
    }
}