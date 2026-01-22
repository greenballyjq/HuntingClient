using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using UnityEngine;
using Hunting.Game.Animal;

/// <summary>
/// 物种派发器
/// </summary>
public class Spawner : BaseSpawner
{
    // /// <summary>
    // /// 派发间隔（秒）
    // /// </summary>
    // [SerializeField] private float spawnInterval = 2f;
    //
    // /// <summary>
    // /// 物种生成线范围
    // /// </summary>
    // [SerializeField] private Vector2 spawnLineRange = new Vector2(-3f, 3f);
    //
    // /// <summary>
    // /// 最小派发角度
    // /// </summary>
    // [SerializeField] private float minSpawnAngle = 0f;
    //
    // /// <summary>
    // /// 最大派发角度
    // /// </summary>
    // [SerializeField] private float maxSpawnAngle = 60f;
    //
    // /// <summary>
    // /// 累积时间
    // /// </summary>
    // private float _accumulatedTime;
    //
    // /// <summary>
    // /// 当前地图数据
    // /// </summary>
    // private Map _mapData;
    //
    // /// <summary>
    // /// 配置管理器
    // /// </summary>
    // private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;
    //
    // /// <summary>
    // /// 动物管理器
    // /// </summary>
    // private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    // /// <summary>
    // /// 初始化派发器
    // /// </summary>
    // /// <param name="mapData">地图数据</param>
    // public void Init(Map mapData)
    // {
    //     _mapData = mapData;
    //     _accumulatedTime = 0f;
    // }
    //
    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void DoUpdate(float dt)
    {
        _accumulatedTime += dt;
    
        if (_accumulatedTime >= spawnInterval)
        {
            SpawnAsync().Forget();
            _accumulatedTime -= spawnInterval;
        }
    }

    #region 重写方法
    /// <summary>
    /// 派发物种
    /// </summary>
    public override async UniTask<BaseAnimalBehaviour> SpawnAsync()
    {
        

        // 从配置按地图与体型策略选出物种
        var specie = _configManager.GetRandomSpecieForMap(_mapData.ID);

        // 计算生成位置与移动方向
        Vector3 spawnPosition = CalculateSpawnPosition();
        Vector3 moveDirection = CalculateMoveDirection();

        // 调用动物管理器生成动物
        return await _animalManager.GenerateAnimalAsync(specie, spawnPosition, moveDirection);
    }
    #endregion

    // #region 编辑器可视化
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.blue;
    //
    //     Vector3 origin = transform.position;
    //     Vector3 forward = transform.forward;
    //
    //     Vector3 startPoint = origin + transform.rotation * new Vector3(0f, 0f, spawnLineRange.x);
    //     Vector3 endPoint = origin + transform.rotation * new Vector3(0f, 0f, spawnLineRange.y);
    //
    //     Gizmos.DrawLine(startPoint, endPoint);
    //     Gizmos.DrawSphere(startPoint, 0.15f);
    //     Gizmos.DrawSphere(endPoint, 0.15f);
    //     Gizmos.DrawSphere(origin, 0.1f);
    //
    //     Gizmos.color = Color.red;
    //     float arcLength = 2.5f;
    //     Vector3 minDir = Quaternion.AngleAxis(minSpawnAngle, Vector3.up) * forward;
    //     Vector3 maxDir = Quaternion.AngleAxis(maxSpawnAngle, Vector3.up) * forward;
    //     Gizmos.DrawLine(origin, origin + minDir * arcLength);
    //     Gizmos.DrawLine(origin, origin + maxDir * arcLength);
    // }
    // #endregion
}