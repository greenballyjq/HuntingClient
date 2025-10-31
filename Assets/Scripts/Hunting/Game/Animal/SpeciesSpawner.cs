using cfg.HuntingConfig;
<<<<<<< HEAD
using GameFramework.Core;
using Hunting;
=======
using Cysharp.Threading.Tasks;
using Hunting;
using Hunting.Game.Animal;
>>>>>>> 1371355b3ecf49dbc4f17683cf8de3f94519a339
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 物种派发器
    /// </summary>
    public class SpeciesSpawner : MonoBehaviour
    {
        /// <summary>
        /// 初始地图 ID
        /// </summary>
        private int mapId = 1;

        /// <summary>
        /// 派发间隔（秒）
        /// </summary>
        [SerializeField] private float spawnInterval = 2f;

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
        /// 上次派发的时间戳
        /// </summary>
        private float _lastSpawnTime;

        /// <summary>
        /// 派发器是否处于启用状态。
        /// </summary>
        private bool _isActive = true;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 事件中心
        /// </summary>
        private EventManager Events => GameServiceLocator.Event;

        private void Start()
        {
<<<<<<< HEAD
            _lastSpawnTime = Time.time;
=======
            await UniTask.Delay(10);
        }
        initialized = true;

        Debug.Log($"[SpeciesSpawner] {spawnDirection}方向派发器初始化完成，地图ID: {currentMapId}");
    }
    #endregion

    private async void Start()
    {
        EventManager eventManager = await GameServiceLocator.GetFrameworkManagerAsync<EventManager>();
        eventManager.AddListener(HuntingEvents.HuntingGameStarted, OnHuntingGameStarted);
    }

    private async void OnHuntingGameStarted()
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
>>>>>>> 1371355b3ecf49dbc4f17683cf8de3f94519a339
        }

        private void Update()
        {
            if (!_isActive)
                return;

            if (Time.time - _lastSpawnTime < spawnInterval)
                return;

            TrySpawn();
            _lastSpawnTime = Time.time;
        }

        #region 私有方法
        /// <summary>
        /// 尝试派发
        /// </summary>
        private void TrySpawn()
        {
            var (specie, stayTime) = Config.GetRandomSpecieForMap(mapId);
            if (specie == null)
            {
                Debug.LogError($"[SpeciesSpawner] 地图 {mapId} 未配置可用物种");
                return;
            }

            Vector3 spawnPosition = CalculateSpawnPosition();
            Vector3 moveDirection = CalculateMoveDirection();

            var args = new SpeciesSpawnEventArgs
            {
                Sender = this,
                Spawner = this,
                SpecieData = specie,
                StayTime = stayTime,
                Position = spawnPosition,
                Direction = moveDirection,
                SpawnTime = Time.time
            };

            OnSpeciesSpawn(args);
        }

        /// <summary>
        /// 计算派发的位置
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
        /// 触发派发事件
        /// </summary>
        private void OnSpeciesSpawn(SpeciesSpawnEventArgs args)
        {
            Events.Trigger(SpawnEvents.SpeciesSpawned, args);
            Debug.Log($"[SpeciesSpawner] 发起派发：物种 {args.SpecieData.ID}，位置 {args.Position}");
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 设置地图 ID
        /// </summary>
        public void SetMap(int value)
        {
            mapId = value;
            Debug.Log($"[SpeciesSpawner] 设置地图 ID 为 {mapId}");
        }

        /// <summary>
        /// 启用或禁用派发
        /// </summary>
        public void SetActive(bool value)
        {
            _isActive = value;
            Debug.Log($"[SpeciesSpawner] 派发启用状态改为 {_isActive}");
        }

        /// <summary>
        /// 当前地图 ID
        /// </summary>
        public int MapId => mapId;

        /// <summary>
        /// 当前派发是否启用
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// 当前派发间隔（秒）
        /// </summary>
        public float SpawnInterval => spawnInterval;
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

  
}