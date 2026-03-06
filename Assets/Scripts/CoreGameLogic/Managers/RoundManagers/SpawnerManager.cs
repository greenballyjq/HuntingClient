using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Manager;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 派发管理器
    /// </summary>
    public class SpawnerManager : IRoundManager, IRoundUpdatable, IRoundResettable
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager => GameServiceLocator.EventManager;

        /// <summary>
        /// 资源加载管理器
        /// </summary>
        private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

        /// <summary>
        /// 对象池管理器
        /// </summary>
        private GameObjectPoolManager _gameObjectPoolManager => GameServiceLocator.GameObjectPoolManager;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

        /// <summary>
        /// 所有派发器列表
        /// </summary>
        private readonly List<BaseSpawner> _spawners = new List<BaseSpawner>();

        /// <summary>
        /// 当前地图数据
        /// </summary>
        private Map _currentMapData;

        /// <summary>
        /// 当前物种类型权重
        /// </summary>
        private Dictionary<ESpecieType, float> _currentSpecieTypeWeights;

        /// <summary>
        /// 缓存的物种数据
        /// </summary>
        private readonly Dictionary<int, Specie> _cachedSpecieData = new Dictionary<int, Specie>();

        /// <summary>
        /// 缓存的预制体
        /// </summary>
        private readonly Dictionary<int, GameObject> _cachedPrefabs = new Dictionary<int, GameObject>();

        public void Init(RoundContext context)
        {
            _currentMapData = context.MapData;
            CollectSpawners();
            InitializeWeights();
            CacheSpecieData();
            CachePrefabs().Forget();
            Debug.Log("[SpawnerManager] 初始化完成");
        }

        public void Dispose()
        {
            _spawners.Clear();
            _cachedSpecieData.Clear();
            _cachedPrefabs.Clear();
            Debug.Log("[SpawnerManager] 已释放");
        }

        public void Cleanup()
        {
            _spawners.Clear();
            _cachedSpecieData.Clear();
            _cachedPrefabs.Clear();
        }

        public void ReInit(RoundContext context)
        {
            _currentMapData = context.HiddenMapData;
            CollectSpawners();
            InitializeWeights();
            CacheSpecieData();
            CachePrefabs().Forget();
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            foreach (var spawner in _spawners)
            {
                if (spawner is AutoSpawner autoSpawner)
                    autoSpawner.DoUpdate(dt);
            }
        }

        #region 公共方法
        /// <summary>
        /// 处理派发请求
        /// </summary>
        /// <param name="spawner">派发器</param>
        /// <param name="spawnInfo">派发信息</param>
        /// <returns>生成的动物实例</returns>
        public BaseAnimalBehaviour HandleSpawnRequest(BaseSpawner spawner, SpawnInfo spawnInfo)
        {
            Specie specie = GetSpecieByStrategy(spawner.Tag);
            return SpawnAnimal(specie, spawnInfo);
        }

        /// <summary>
        /// 获取派发器
        /// </summary>
        /// <typeparam name="T">派发器类型</typeparam>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public T GetSpawner<T>(string tag) where T : BaseSpawner
            => _spawners.OfType<T>().FirstOrDefault(s => s.Tag == tag);

        /// <summary>
        /// 获取所有匹配标签的派发器
        /// </summary>
        /// <typeparam name="T">派发器类型</typeparam>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public List<T> GetSpawners<T>(string tag) where T : BaseSpawner
            => _spawners.OfType<T>().Where(s => s.Tag == tag).ToList();

        /// <summary>
        /// 更新权重
        /// </summary>
        /// <param name="type">物种类型</param>
        /// <param name="weight">权重值</param>
        public void UpdateWeight(ESpecieType type, float weight)
        {
            if (_currentSpecieTypeWeights.ContainsKey(type))
                _currentSpecieTypeWeights[type] = weight;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 收集所有派发器
        /// </summary>
        private void CollectSpawners()
        {
            _spawners.Clear();
            _spawners.AddRange(Object.FindObjectsOfType<BaseSpawner>());
        }

        /// <summary>
        /// 初始化权重
        /// </summary>
        private void InitializeWeights()
        {
            _currentSpecieTypeWeights = _configManager.GetMapSpecieTypeWeights(_currentMapData.ID);
        }

        /// <summary>
        /// 缓存物种数据
        /// </summary>
        private void CacheSpecieData()
        {
            var mapSpecies = _configManager.GetMapSpecies(_currentMapData.ID);
            foreach (var kv in mapSpecies)
                foreach (var specieId in kv.Value)
                    _cachedSpecieData[specieId] = _configManager.GetSpecie(specieId);
        }

        /// <summary>
        /// 缓存预制体
        /// </summary>
        private async UniTaskVoid CachePrefabs()
        {
            foreach (var kv in _cachedSpecieData)
            {
                var prefab = await _resourceManager.LoadAssetAsync<GameObject>(kv.Value.PrefabResourcePath);
                _cachedPrefabs[kv.Key] = prefab;
            }
        }

        /// <summary>
        /// 根据策略获取物种
        /// </summary>
        /// <param name="spawnerTag">派发器标签</param>
        /// <returns></returns>
        private Specie GetSpecieByStrategy(string spawnerTag)
        {
            if (spawnerTag == "Random")
                return GetRandomSpecieByWeight();
            else if (spawnerTag == "Boss")
                return _configManager.GetRandomBoss();

            return null;
        }

        /// <summary>
        /// 根据权重随机获取物种
        /// </summary>
        /// <returns></returns>
        private Specie GetRandomSpecieByWeight()
        {
            float totalWeight = _currentSpecieTypeWeights.Values.Sum();
            float randomPoint = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var kv in _currentSpecieTypeWeights)
            {
                cumulative += kv.Value;
                if (randomPoint <= cumulative)
                {
                    var specieIds = _currentMapData.MapSpecies[kv.Key];
                    int specieId = specieIds[Random.Range(0, specieIds.Length)];
                    return _cachedSpecieData[specieId];
                }
            }

            return null;
        }

        /// <summary>
        /// 生成动物
        /// </summary>
        /// <param name="specie">物种数据</param>
        /// <param name="spawnInfo">派发信息</param>
        /// <returns>生成的动物实例</returns>
        private BaseAnimalBehaviour SpawnAnimal(Specie specie, SpawnInfo spawnInfo)
        {
            var go = _gameObjectPoolManager.Spawn(_cachedPrefabs[specie.ID]);
            go.transform.position = spawnInfo.Position;

            var animal = go.GetComponent<BaseAnimalBehaviour>();
            animal.Init(specie);
            animal.Moveable.SetDirection(spawnInfo.Direction);

            TriggerAnimalGenerated(new AnimalGeneratedEventArgs
            {
                Animal = animal,
            });
            return animal;
        }

        /// <summary>
        /// 触发动物生成事件
        /// </summary>
        /// <param name="animal">动物实例</param>
        private void TriggerAnimalGenerated(AnimalGeneratedEventArgs args)
        {
            _eventManager.Trigger(AnimalEvents.AnimalGenerated, args);
        }
        #endregion
    }

}

