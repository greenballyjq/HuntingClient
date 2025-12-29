using UnityEngine;
using System.Collections.Generic;
using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;

/// <summary>
/// 测试版派发器
/// 不依赖游戏流程框架
/// </summary>
public class TestSpawner : MonoBehaviour
{
    [Header("派发间隔")]
    [SerializeField] private float spawnInterval = 2f;

    [Header("物种生成范围（默认在一条线范围内生成，调成0,0会在一个点上生成）")]
    [SerializeField] private Vector2 spawnLineRange = new Vector2(-3f, 3f);

    [Header("最小派发角度（物种会朝被派发的角度前进）")]
    [SerializeField] private float minSpawnAngle = 0f;

    [Header("最大派发角度（物种会朝被派发的角度前进）")]
    [SerializeField] private float maxSpawnAngle = 60f;

    [Header("启用派发")]
    [SerializeField] private bool isActive = true;


    private int _mapId = 1;

    

    /// <summary>
    /// 配置管理器
    /// </summary>
    private TestConfigManager Config => TestConfigManager.Instance;

    /// <summary>
    /// 上次派发的时间戳
    /// </summary>
    private float _lastSpawnTime;

    /// <summary>
    /// 已生成的动物列表（用于管理）
    /// </summary>
    private List<GameObject> _spawnedAnimals = new List<GameObject>();

    /// <summary>
    /// 配置是否已初始化
    /// </summary>
    private bool _configReady = false;

    /// <summary>
    /// 动物根节点
    /// </summary>
    private Transform _animalsRoot;

    private async void Start()
    {
        _lastSpawnTime = Time.time;

        // 创建或获取动物根节点
        CreateAnimalsRoot();

        // 等待配置管理器初始化
        if (!Config.Initialized)
        {
            await Config.WaitForInitializationAsync();
        }
        _configReady = true;
    }

    /// <summary>
    /// 创建或获取动物根节点
    /// </summary>
    private void CreateAnimalsRoot()
    {
        // 查找场景中是否已存在根节点
        GameObject rootObj = GameObject.Find("场景上的动物");
        
        if (rootObj == null)
        {
            // 如果不存在，创建新的根节点
            rootObj = new GameObject("场景上的动物");
        }
        
        _animalsRoot = rootObj.transform;
    }

    private void Update()
    {
        if (!isActive || !_configReady)
            return;

        if (Time.time - _lastSpawnTime < spawnInterval)
            return;

        SpawnAnimal();
        _lastSpawnTime = Time.time;
    }

    /// <summary>
    /// 派发动物
    /// </summary>
    public void SpawnAnimal()
    {
        // 从配置中获取随机物种和驻场时间
        var (specie, stayTime) = Config.GetRandomSpecieForMap(_mapId);

        if (specie == null)
        {
            Debug.LogError($"[StandaloneSpawner] 地图 {_mapId} 未配置可用物种");
            return;
        }

        // 计算生成位置和方向
        Vector3 spawnPosition = CalculateSpawnPosition();
        Vector3 moveDirection = CalculateMoveDirection();

        // 从配置中获取预制体路径并加载
        LoadAndSpawnAnimal(specie, spawnPosition, moveDirection, stayTime);
    }

    /// <summary>
    /// 加载并生成动物
    /// </summary>
    private async void LoadAndSpawnAnimal(Specie specieData, Vector3 position, Vector3 direction, float stayTime)
    {
        // 从配置中获取预制体路径
        string prefabPath = specieData.TestPrefabResourcePath;

        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError($"[StandaloneSpawner] 物种 {specieData.ID} 的预制体路径为空");
            return;
        }

        // 从 Resources 加载预制体
        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"[StandaloneSpawner] 无法加载预制体: {prefabPath}");
            return;
        }

        // 实例化动物
        GameObject animalObj = Instantiate(prefab, position, Quaternion.LookRotation(direction));

        // 将动物设置为根节点的子对象
        if (_animalsRoot != null)
        {
            animalObj.transform.SetParent(_animalsRoot);
        }

        // 初始化动物
        InitializeAnimal(animalObj, specieData, stayTime);

        // 记录生成的动物
        _spawnedAnimals.Add(animalObj);
    }

    /// <summary>
    /// 初始化动物
    /// </summary>
    private void InitializeAnimal(GameObject animalObj, Specie specieData, float stayTime)
    {
        // 优先使用测试动物类
        var testAnimal = animalObj.GetComponent<TestAnimalBehavior>();
        
        if (testAnimal != null)
        {
            // 使用测试动物类初始化
            testAnimal.Init(specieData, stayTime);
            return;
        }

        // 如果没有测试类，尝试使用原版 AnimalBehavior（兼容性）
        var animalBehavior = animalObj.GetComponent<AnimalBehavior>();

        if (animalBehavior != null)
        {
            // 使用配置数据初始化
            animalBehavior.Init(specieData, stayTime);
        }
        else
        {
            Debug.LogWarning($"[TestSpawner] 动物预制体没有 TestAnimalBehavior 或 AnimalBehavior 组件，将作为普通 GameObject 生成");
        }
    }

    /// <summary>
    /// 计算派发位置
    /// </summary>
    private Vector3 CalculateSpawnPosition()
    {
        float offset = Random.Range(spawnLineRange.x, spawnLineRange.y);
        Vector3 localOffset = new Vector3(0f, 0f, offset);
        return transform.position + transform.rotation * localOffset;
    }

    /// <summary>
    /// 计算派发方向
    /// </summary>
    private Vector3 CalculateMoveDirection()
    {
        float angle = Random.Range(minSpawnAngle, maxSpawnAngle);
        return Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
    }

    /// <summary>
    /// 设置启用状态
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
    }

    /// <summary>
    /// 设置地图ID
    /// </summary>
    public void SetMapId(int id)
    {
        _mapId = id;
    }

    /// <summary>
    /// 清理所有已生成的动物
    /// </summary>
    public void ClearAllAnimals()
    {
        foreach (var animal in _spawnedAnimals)
        {
            if (animal != null)
                Destroy(animal);
        }
        _spawnedAnimals.Clear();
    }

    #region 编辑器可视化
    private void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.green : Color.gray;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // 绘制生成线
        Vector3 startPoint = origin + transform.rotation * new Vector3(0f, 0f, spawnLineRange.x);
        Vector3 endPoint = origin + transform.rotation * new Vector3(0f, 0f, spawnLineRange.y);

        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawSphere(startPoint, 0.15f);
        Gizmos.DrawSphere(endPoint, 0.15f);
        Gizmos.DrawSphere(origin, 0.1f);

        // 绘制角度范围
        Gizmos.color = Color.red;
        float arcLength = 2.5f;
        Vector3 minDir = Quaternion.AngleAxis(minSpawnAngle, Vector3.up) * forward;
        Vector3 maxDir = Quaternion.AngleAxis(maxSpawnAngle, Vector3.up) * forward;
        Gizmos.DrawLine(origin, origin + minDir * arcLength);
        Gizmos.DrawLine(origin, origin + maxDir * arcLength);
    }
    #endregion
}