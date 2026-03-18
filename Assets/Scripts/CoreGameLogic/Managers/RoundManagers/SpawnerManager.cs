using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 派发管理器
    /// </summary>
    public class SpawnerManager : IRoundManager, IRoundUpdatable, IRoundResettable
    {
        /// <summary>
        /// 所有派发器列表
        /// </summary>
        private readonly List<BaseSpawner> _spawners = new List<BaseSpawner>();

        /// <summary>
        /// 地图数据缓存
        /// </summary>
        private Map _mapData;

        /// <summary>
        /// 动物类型权重缓存
        /// </summary>
        private Dictionary<ESpecieType, float> _animalTypeWeights;

        /// <summary>
        /// 动物配置缓存
        /// </summary>
        private Dictionary<int, Specie> _animalDatas = new Dictionary<int, Specie>();

        /// <summary>
        /// 动物预制体缓存
        /// </summary>
        private Dictionary<int, GameObject> _animalPrefabs = new Dictionary<int, GameObject>();

        private EventManager _eventManager;
        private ResourceManager _resourceManager;
        private GameObjectPoolManager _gameObjectPoolManager;
        private HuntingConfigManager _configManager;
        private AnimalManager _animalManager;

        public void Init(RoundContext context)
        {
            _mapData = context.MapData;

            RegisterServices();
            CollectSpawners();
            InitializeWeights();
            CacheAnimalDatas(_mapData.ID);

            Log.Info("[SpawnerManager] 初始化完成");
        }

        public void Dispose()
        {
            _spawners.Clear();
            ClearAnimalCache();

            Log.Info("[SpawnerManager] 已释放");
        }

        public void Cleanup()
        {
            _spawners.Clear();
            ClearAnimalCache();
        }

        public void ReInit(Map mapData)
        {
            _mapData = mapData;
            CollectSpawners();
            InitializeWeights();
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
            if (_animalTypeWeights.ContainsKey(type))
                _animalTypeWeights[type] = weight;
        }
        #endregion

        #region 私有方法
        private void RegisterServices()
        {
            _eventManager = GameServiceLocator.EventManager;
            _resourceManager = GameServiceLocator.ResourceManager;
            _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
            _configManager = GameServiceLocator.ConfigManager;
            _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        }

        /// <summary>
        /// 缓存动物配置
        /// </summary>
        /// <param name="mapId">地图ID</param>
        private void CacheAnimalDatas(int mapId)
        {
            ClearAnimalCache();

            _animalDatas = _animalManager.AnimalDatas;
            _animalPrefabs = _animalManager.AnimalPrefabs;
        }

        /// <summary>
        /// 清理动物配置缓存
        /// </summary>
        private void ClearAnimalCache()
        {
            _animalDatas.Clear();
            _animalPrefabs.Clear();
        }

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
            _animalTypeWeights = _configManager.GetMapSpecieTypeWeights(_mapData.ID);
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
            float totalWeight = _animalTypeWeights.Values.Sum();
            float randomPoint = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var kv in _animalTypeWeights)
            {
                cumulative += kv.Value;
                if (randomPoint <= cumulative)
                {
                    var specieIds = _mapData.MapSpecies[kv.Key];
                    int specieId = specieIds[Random.Range(0, specieIds.Length)];
                    return _animalDatas[specieId];
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
            var go = _gameObjectPoolManager.Spawn(_animalPrefabs[specie.ID]);
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

