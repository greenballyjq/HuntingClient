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
        public class SpawnerManager : IMapWorld, IRoundUpdatable
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

        private EventManager _eventManager;
        private GameObjectPoolManager _gameObjectPoolManager;
        private HuntingConfigManager _configManager;
        private AnimalManager _animalManager;
        private RoundNumericLayer _numeric;

        public UniTask InitAsync(RoundContext context)
        {
            _mapData = context.MapData;

            BindServices();
            CollectSpawners();
            InitializeWeights();

            Log.Info("[SpawnerManager] 初始化完成");
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _spawners.Clear();

            Log.Info("[SpawnerManager] 已释放");
        }

        public void Unbind()
        {
            _spawners.Clear();
        }

        public void Bind(Map mapData)
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
        #endregion

        #region 私有方法
        private void BindServices()
        {
            _eventManager = GameServiceLocator.EventManager;
            _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
            _configManager = GameServiceLocator.ConfigManager;
            _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
            _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
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
                return GetMapBoss();

            return null;
        }

        private Specie GetMapBoss()
        {
            if (!_mapData.MapSpecies.TryGetValue(ESpecieType.Boss, out int[] bossIds) || bossIds == null || bossIds.Length == 0)
            {
                Log.Error($"[SpawnerManager] 地图 {_mapData.ID} 未配置 Boss 物种");
                return null;
            }

            return RequireCachedSpecie(bossIds[Random.Range(0, bossIds.Length)]);
        }

        private Specie RequireCachedSpecie(int specieId)
        {
            if (_animalManager.AnimalDatas.TryGetValue(specieId, out Specie specie) && specie != null)
                return specie;

            Log.Error($"[SpawnerManager] 本图未缓存物种 {specieId}");
            return null;
        }

        /// <summary>
        /// 根据权重随机获取物种
        /// </summary>
        /// <returns></returns>
        private Specie GetRandomSpecieByWeight()
        {
            float totalWeight = 0f;
            foreach (var kv in _animalTypeWeights)
                totalWeight += _numeric.EvaluateSpawnWeight(kv.Key, kv.Value);

            if (totalWeight <= 0f)
                return null;

            float randomPoint = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var kv in _animalTypeWeights)
            {
                cumulative += _numeric.EvaluateSpawnWeight(kv.Key, kv.Value);
                if (randomPoint <= cumulative)
                {
                    var specieIds = _mapData.MapSpecies[kv.Key];
                    int specieId = specieIds[Random.Range(0, specieIds.Length)];
                    return RequireCachedSpecie(specieId);
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
            if (specie == null)
            {
                Log.Error("[SpawnerManager] 物种为空，无法生成");
                return null;
            }

            if (!_animalManager.AnimalPrefabs.TryGetValue(specie.ID, out GameObject prefab) || prefab == null)
            {
                Log.Error($"[SpawnerManager] 本图未缓存物种预制体: {specie.ID}");
                return null;
            }

            var go = _gameObjectPoolManager.Spawn(prefab);
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

