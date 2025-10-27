using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using Hunting.Manager;
using UnityEngine;

/// <summary>
/// 物种派发器
/// </summary>
public class SpeciesSpawner : MonoBehaviour
{
    /// <summary>
    /// 物种派发方向
    /// </summary>
    public enum SpawnDirection
    {
        /// <summary>
        /// 从左侧生成，向右移动
        /// </summary>
        Left,

        /// <summary>
        /// 从右侧生成，向左移动
        /// </summary>
        Right
    }

    #region 编辑器参数
    /// <summary>
    /// 派发方向
    /// </summary>
    [SerializeField] private SpawnDirection spawnDirection;

    /// <summary>
    /// 当前地图ID
    /// </summary>
    [SerializeField] private int currentMapId = 1;

    /// <summary>
    /// 生成间隔（秒）
    /// </summary>
    [SerializeField] private float spawnInterval = 2f;

    /// <summary>
    /// 生成位置Z坐标范围（前后位置）
    /// </summary>
    [SerializeField] private Vector2 spawnZRange = new Vector2(-3f, 3f);

    /// <summary>
    /// 生成位置X偏移（根据方向固定）
    /// </summary>
    [SerializeField] private float spawnXOffset = 2f;

    [Header("移动方向设置")]
    [Tooltip("最小角度（度）")]
    [SerializeField] private float minAngle = 0f;

    [Tooltip("最大角度（度）")]
    [SerializeField] private float maxAngle = 60f;
    #endregion

    #region 运行时变量
    /// <summary>
    /// 上次生成时间
    /// </summary>
    private float lastSpawnTime;

    /// <summary>
    /// 动物预制体基础路径
    /// </summary>
    private const string ANIMAL_PREFAB_BASE_PATH = "Animals/";
    #endregion

    #region 初始化相关
    private bool initialized;

    /// <summary>
    /// 异步初始化
    /// </summary>
    private async UniTask Init()
    {
        // 等待 HuntingGameConfigManager 初始化完成
        while (HuntingGameConfigManager.Instance == null || !HuntingGameConfigManager.Instance.Initialized)
        {
            await UniTask.Delay(10);
        }
        initialized = true;

        Debug.Log($"[SpeciesSpawner] {spawnDirection}方向派发器初始化完成，地图ID: {currentMapId}");
    }
    #endregion

    private async void Start()
    {
        await Init();
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        if (!initialized) return;

        // 检查生成间隔
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            TrySpawnAnimal();
            lastSpawnTime = Time.time;
        }
    }

    /// <summary>
    /// 尝试生成动物
    /// </summary>
    private void TrySpawnAnimal()
    {
        // 从配置管理器获取随机物种
        var (specie, stayTime) = HuntingGameConfigManager.Instance.GetRandomSpecieForMap(currentMapId);

        if (specie == null)
        {
            Debug.LogError($"[SpeciesSpawner] 无法获取物种数据，地图ID: {currentMapId}");
            return;
        }

        // 生成动物
        SpawnAnimal(specie, stayTime);
    }

    /// <summary>
    /// 生成动物实例
    /// </summary>
    /// <param name="specie">物种数据</param>
    /// <param name="stayTime">驻场时间</param>
    private void SpawnAnimal(Specie specie, float stayTime)
    {
        // 根据物种ID获取对应的预制体名称
        string prefabName = GetAnimalPrefabName(specie);
        string prefabPath = $"{ANIMAL_PREFAB_BASE_PATH}{prefabName}";

        // 加载动物预制体
        GameObject animalPrefab = Resources.Load<GameObject>(prefabPath);
        if (animalPrefab == null)
        {
            Debug.LogError($"[SpeciesSpawner] 动物预制体不存在: {prefabPath}");
            return;
        }

        // 计算生成位置（基于派发器位置）
        Vector3 spawnPosition = CalculateSpawnPosition();

        // 计算移动方向
        Vector3 moveDirection = CalculateMoveDirection();

        // 实例化动物
        GameObject animalObj = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
        AnimalBehavior animal = animalObj.GetComponent<AnimalBehavior>();

        if (animal != null)
        {
            // 初始化动物
            animal.Initialize(specie, stayTime, moveDirection);

            // 设置动物朝向与移动方向一致
            if (moveDirection != Vector3.zero)
            {
                animalObj.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
        else
        {
            Debug.LogError($"[SpeciesSpawner] 动物预制体缺少Animal组件: {prefabName}");
            Destroy(animalObj);
        }
    }

    /// <summary>
    /// 根据物种数据获取预制体名称
    /// </summary>
    /// <param name="specie">物种数据</param>
    /// <returns>预制体名称</returns>
    private string GetAnimalPrefabName(Specie specie)
    {
        return $"Animal_{specie.VolumeType}_{specie.ID}";
    }

    /// <summary>
    /// 计算生成位置
    /// </summary>
    /// <returns>生成位置</returns>
    private Vector3 CalculateSpawnPosition()
    {
        // 基于派发器对象的位置
        Vector3 basePosition = transform.position;

        // 根据方向添加X轴偏移
        float xOffset = spawnDirection == SpawnDirection.Left ? -spawnXOffset : spawnXOffset;

        // 在Z轴范围内随机
        float zOffset = Random.Range(spawnZRange.x, spawnZRange.y);

        return basePosition + new Vector3(xOffset, 0f, zOffset);
    }

    /// <summary>
    /// 计算移动方向
    /// </summary>
    /// <returns>移动方向向量</returns>
    private Vector3 CalculateMoveDirection()
    {
        // 在设置的角度范围内随机
        float randomAngle = Random.Range(minAngle, maxAngle);
        return Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
    }

    /// <summary>
    /// 设置当前地图
    /// </summary>
    /// <param name="mapId">地图ID</param>
    public void SetCurrentMap(int mapId)
    {
        currentMapId = mapId;
    }

    /// <summary>
    /// 设置生成间隔
    /// </summary>
    /// <param name="interval">生成间隔（秒）</param>
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
    }

    #region 调试
    /// <summary>
    /// 在Scene视图中显示生成范围和移动方向
    /// </summary>
    private void OnDrawGizmos()
    {
        // 生成位置范围
        Gizmos.color = spawnDirection == SpawnDirection.Left ? Color.blue : Color.red;

        Vector3 center = transform.position;
        Vector3 leftPos = center + new Vector3(spawnDirection == SpawnDirection.Left ? -spawnXOffset : spawnXOffset, 0f, spawnZRange.x);
        Vector3 rightPos = center + new Vector3(spawnDirection == SpawnDirection.Left ? -spawnXOffset : spawnXOffset, 0f, spawnZRange.y);

        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawWireSphere(leftPos, 0.5f);
        Gizmos.DrawWireSphere(rightPos, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        // 移动方向范围
        Gizmos.color = Color.green;
        DrawDirectionArc(center, minAngle, maxAngle, 3f);
    }

    /// <summary>
    /// 绘制方向弧线（调试用）
    /// </summary>
    private void DrawDirectionArc(Vector3 center, float startAngle, float endAngle, float radius)
    {
        int segments = 10;
        float angleStep = (endAngle - startAngle) / segments;

        Vector3 prevPoint = center + Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 nextPoint = center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // 绘制起始和结束方向线
        Gizmos.DrawLine(center, center + Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius);
        Gizmos.DrawLine(center, center + Quaternion.Euler(0f, endAngle, 0f) * Vector3.forward * radius);
    }
    #endregion
}

