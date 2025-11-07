using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 物种派发器
    /// </summary>
    public class Spawner : MonoBehaviour
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
        private bool _isActive = false;

        /// <summary>
        /// 当前派发是否启用
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 事件中心
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        private void Start()
        {
            _lastSpawnTime = Time.time;
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
            // 从配置按地图与体型策略选出物种与驻场时间
            var (specie, stayTime) = Config.GetRandomSpecieForMap(mapId);
            if (specie == null)
            {
                Debug.LogError($"[SpeciesSpawner] 地图 {mapId} 未配置可用物种");
                return;
            }

            // 计算生成位置与移动方向
            Vector3 spawnPosition = CalculateSpawnPosition();
            Vector3 moveDirection = CalculateMoveDirection();

            // 触发派发事件
            TriggerSpecieSpawn(new SpeciesSpawnEventArgs
            {
                Sender = this,
                Spawner = this,
                SpecieData = specie,
                StayTime = stayTime,
                Position = spawnPosition,
                Direction = moveDirection,
            }); 
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
        private void TriggerSpecieSpawn(SpeciesSpawnEventArgs args)
        {
            Event.Trigger(SpawnEvents.SpeciesSpawned, args);
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