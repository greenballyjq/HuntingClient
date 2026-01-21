using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using UnityEngine;

public abstract class BaseSpawner : MonoBehaviour, IAnimalSpawner
{
    /// <summary>
    /// 派发间隔（秒）
    /// </summary>
    [SerializeField] protected float spawnInterval;
    /// <summary>
    /// 物种生成线范围
    /// </summary>
    [SerializeField] private Vector2 spawnLineRange = new Vector2(-3f, 3f);

    /// <summary>
    /// 最小派发角度
    /// </summary>
    [SerializeField] private float minSpawnAngle = 0f;

    /// <summary>
    /// 最大派发角度
    /// </summary>
    [SerializeField] private float maxSpawnAngle = 60f;

    /// <summary>
    /// 累积时间
    /// </summary>
    protected float _accumulatedTime;

    /// <summary>
    /// 当前地图数据
    /// </summary>
    protected Map _mapData;

    /// <summary>
    /// 配置管理器
    /// </summary>
    protected HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 动物管理器
    /// </summary>
    protected AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    /// <summary>
    /// 初始化派发器
    /// </summary>
    /// <param name="mapData">地图数据</param>
    public void Init(Map mapData)
    {
        _mapData = mapData;
        _accumulatedTime = 0f;
        // _configManager.GetSpe
    }
    
    // /// <summary>
    // /// 每帧更新
    // /// </summary>
    // /// <param name="dt">时间增量</param>
    // public void DoUpdate(float dt)
    // {
    //     _accumulatedTime += dt;
    //
    //     if (_accumulatedTime >= spawnInterval)
    //     {
    //         SpawnAsync().Forget();
    //         _accumulatedTime -= spawnInterval;
    //     }
    // }

    #region 私有方法
    public abstract UniTask<AnimalBehavior> SpawnAsync();

    /// <summary>
    /// 计算派发的位置
    /// </summary>
    protected Vector3 CalculateSpawnPosition()
    {
        float offset = Random.Range(spawnLineRange.x, spawnLineRange.y);
        Vector3 localOffset = new Vector3(0f, 0f, offset);
        return transform.position + transform.rotation * localOffset;
    }

    /// <summary>
    /// 计算派发方向
    /// </summary>
    protected Vector3 CalculateMoveDirection()
    {
        float angle = Random.Range(minSpawnAngle, maxSpawnAngle);
        return Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
    }

    #endregion

    #region 编辑器可视化
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Vector3 startPoint = origin + transform.rotation * new Vector3(0f, 0f, spawnLineRange.x);
        Vector3 endPoint = origin + transform.rotation * new Vector3(0f, 0f, spawnLineRange.y);

        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawSphere(startPoint, 0.15f);
        Gizmos.DrawSphere(endPoint, 0.15f);
        Gizmos.DrawSphere(origin, 0.1f);

        Gizmos.color = Color.red;
        float arcLength = 2.5f;
        Vector3 minDir = Quaternion.AngleAxis(minSpawnAngle, Vector3.up) * forward;
        Vector3 maxDir = Quaternion.AngleAxis(maxSpawnAngle, Vector3.up) * forward;
        Gizmos.DrawLine(origin, origin + minDir * arcLength);
        Gizmos.DrawLine(origin, origin + maxDir * arcLength);
    }
    #endregion
}