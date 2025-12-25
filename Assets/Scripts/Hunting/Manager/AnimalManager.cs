using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using Hunting.Game.Animal;
using Hunting.Round;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 动物管理器
    /// </summary>
    public class AnimalManager : IRoundManager
    {
        /// <summary>
        /// 单局服务集合
        /// </summary>
        private readonly RoundServices _services;

        public AnimalManager(RoundServices services)
        {
            _services = services;
        }

        /// <summary>
        /// 初始化单局管理器
        /// </summary>
        /// <param name="context">单局上下文</param>
        public void Init(RoundContext context)
        {
            RegisterEvents();
            Debug.Log("[AnimalManager] 初始化完成");
        }

        /// <summary>
        /// 释放单局管理器
        /// </summary>
        public void Dispose()
        {
            UnregisterEvents();
            Debug.Log("[AnimalManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 生成动物
        /// </summary>
        public async UniTask<AnimalBehavior> SpawnAnimalAsync(Specie specieData, Vector3 position, Vector3 direction, float stayTime, Spawner spawner = null)
        {
            var go = await _services.Pool.SpawnAsync(specieData.PrefabResourcePath);
            if (go == null)
            {
                Debug.LogError($"[AnimalManager] 从对象池获取失败: {specieData.PrefabResourcePath}");
                return null;
            }

            var animal = go.GetComponent<AnimalBehavior>();
            if (animal == null)
            {
                Debug.LogError("[AnimalManager] 预制体缺少 AnimalBehavior 组件");
                _services.Pool.Despawn(go);
                return null;
            }

            go.transform.position = position;
            if (direction != Vector3.zero)
                go.transform.rotation = Quaternion.LookRotation(direction);

            animal.Init(specieData, stayTime);

            TriggerAnimalSpawned(new AnimalSpawnedEventArgs
            {
                Sender = this,
                Spawner = spawner,
                Animal = animal,
                SpecieData = specieData,
                Position = position,
                Direction = direction,
                StayTime = stayTime
            });

            return animal;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            _services.Event.AddListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
            _services.Event.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
            _services.Event.AddListener(AnimalEvents.AnimalFled, OnAnimalFled);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            _services.Event.RemoveListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
            _services.Event.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
            _services.Event.RemoveListener(AnimalEvents.AnimalFled, OnAnimalFled);
        }

        /// <summary>
        /// 物种派发回调
        /// </summary>
        private async void OnSpeciesSpawned(SpeciesSpawnEventArgs args)
        {
            await SpawnAnimalAsync(args.SpecieData, args.Position, args.Direction, args.StayTime, args.Spawner);
        }

        /// <summary>
        /// 动物死亡事件回调
        /// </summary>
        private void OnAnimalDied(AnimalDiedEventArgs args)
        {
            _services.Pool.Despawn(args.Animal.gameObject);
        }

        /// <summary>
        /// 动物逃跑事件回调
        /// </summary>
        private void OnAnimalFled(AnimalFledEventArgs args)
        {
            _services.Pool.Despawn(args.Animal.gameObject);
        }

        /// <summary>
        /// 触发动物生成完成事件
        /// </summary>
        private void TriggerAnimalSpawned(AnimalSpawnedEventArgs args)
        {
            _services.Event.Trigger(AnimalEvents.AnimalSpawned, args);
        }
        #endregion
    }
}
