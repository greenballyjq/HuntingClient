using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using cfg;
using cfg.HuntingConfig;
using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 动物管理器
    /// </summary>
    public class AnimalManager : BaseGameManager
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 对象池管理器
        /// </summary>
        private GameObjectPoolManager Pool => GameServiceLocator.Pool;

        public override void Init()
        {
            RegisterEvents();
            Debug.Log("[AnimalManager] 初始化完成");
        }

        public override void Update(){}

        public override void Release()
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
            var go = await Pool.PullAsync(specieData.PrefabResourcePath);
            if (go == null)
            {
                Debug.LogError($"[AnimalManager] 从对象池获取失败: {specieData.PrefabResourcePath}");
                return null;
            }

            var animal = go.GetComponent<AnimalBehavior>();
            if (animal == null)
            {
                Debug.LogError("[AnimalManager] 预制体缺少 AnimalBehavior 组件");
                Pool.Push(go);
                return null;
            }

            go.transform.position = position;
            if (direction != Vector3.zero)
                go.transform.rotation = Quaternion.LookRotation(direction);

            animal.Init(specieData, stayTime, direction);

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

        #region 私有方法
        /// <summary>
        /// 回收动物到对象池并从管理列表移除
        /// </summary>
        private void RecycleAnimal(AnimalBehavior animal)
        {
            Pool.Push(animal.gameObject);
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
            Event.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
            Event.AddListener(AnimalEvents.AnimalFled, OnAnimalFled);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
            Event.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
            Event.RemoveListener(AnimalEvents.AnimalFled, OnAnimalFled);
        }
        
        /// <summary>
        /// 物种派发回调
        /// </summary>
        private async void OnSpeciesSpawned(SpeciesSpawnEventArgs args)
        {
            // 从对象池获取对应物种的预制体
            await SpawnAnimalAsync(args.SpecieData, args.Position, args.Direction, args.StayTime, args.Spawner);
        }

        /// <summary>
        /// 动物死亡事件回调
        /// </summary>
        private void OnAnimalDied(AnimalDiedEventArgs args)
        {
            if (args.Animal == null)
                return;

            // 回收动物到对象池
            RecycleAnimal(args.Animal);
        }

        /// <summary>
        /// 动物逃跑事件回调
        /// </summary>
        private void OnAnimalFled(AnimalFledEventArgs args)
        {
            if (args.Animal == null)
                return;

            // 回收动物到对象池
            RecycleAnimal(args.Animal);
        }

        /// <summary>
        /// 触发动物生成完成事件
        /// </summary>
        private void TriggerAnimalSpawned(AnimalSpawnedEventArgs args)
        {
            Event.Trigger(AnimalEvents.AnimalSpawned, args);
        }
        #endregion     
    }
}
