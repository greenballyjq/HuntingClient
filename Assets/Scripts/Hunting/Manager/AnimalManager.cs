using System.Collections.Generic;
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
        /// 动物列表
        /// </summary>
        private readonly List<AnimalBehavior> _animals = new List<AnimalBehavior>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Events => GameServiceLocator.Event;

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
            ClearAll();
            Debug.Log("[AnimalManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 获取所有动物
        /// </summary>
        public List<AnimalBehavior> GetAllAnimals() => _animals;

        /// <summary>
        /// 批量暂停/恢复
        /// </summary>
        public void SetAllPaused(bool paused)
        {

        }

        /// <summary>
        /// 清空并回收全部动物
        /// </summary>
        public void ClearAll()
        {
            for (int i = _animals.Count - 1; i >= 0; i--)
            {
                var a = _animals[i];
                if (a != null)
                {
                    // 回收到对象池
                    Pool.Push(a.gameObject);
                }
            }
            _animals.Clear();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 回收指定动物到对象池并从管理列表移除
        /// </summary>
        /// <param name="animal"></param>
        private void ReturnAnimal(AnimalBehavior animal)
        {
            if (animal == null)
                return;

            _animals.Remove(animal);
            Pool.Push(animal.gameObject);
        }

        /// <summary>
        /// 根据物种配置生成预制体路径
        /// </summary>
        /// <param name="specie"></param>
        /// <returns></returns>
        private string GetPrefabPath(Specie specie)
        {
            return $"Animals/Animal_{specie.VolumeType}_{specie.ID}";
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Events.AddListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
            Events.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
            Events.AddListener(AnimalEvents.AnimalFled, OnAnimalFled);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Events.RemoveListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
            Events.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
            Events.RemoveListener(AnimalEvents.AnimalFled, OnAnimalFled);
        }
        
        /// <summary>
        /// 物种派发回调
        /// </summary>
        private async void OnSpeciesSpawned(SpeciesSpawnEventArgs args)
        {
            // 从对象池获取对应物种的预制体
            string prefabPath = GetPrefabPath(args.SpecieData);
            var go = await Pool.PullAsync(prefabPath);
            if (go == null)
            {
                Debug.LogError($"[AnimalManager] 从对象池获取失败: {prefabPath}");
                return;
            }

            // 设置初始位置与方向
            go.transform.position = args.Position;
            if (args.Direction != Vector3.zero)
                go.transform.rotation = Quaternion.LookRotation(args.Direction);

            // 绑定并初始化动物行为组件
            var animal = go.GetComponent<AnimalBehavior>();
            if (animal == null)
            {
                Debug.LogError("[AnimalManager] 预制体缺少 AnimalBehavior 组件");
                Pool.Push(go);
                return;
            }

            animal.Init(args.SpecieData, args.StayTime, args.Direction);
            _animals.Add(animal);

            // 对外通知动物已生成
            TriggerAnimalSpawned(new AnimalSpawnedEventArgs
            {
                Sender = this,
                Spawner = args.Spawner,
                Animal = animal,
                SpecieData = args.SpecieData,
                Position = args.Position,
                Direction = args.Direction,
                StayTime = args.StayTime
            });
        }
 
        /// <summary>
        /// 触发动物生成完成事件
        /// </summary>
        private void TriggerAnimalSpawned(AnimalSpawnedEventArgs args)
        {
            Events.Trigger(AnimalEvents.AnimalSpawned, args);
        }

        /// <summary>
        /// 动物死亡事件回调
        /// </summary>
        private void OnAnimalDied(AnimalDiedEventArgs args)
        {
            if (args.Animal == null)
                return;

            // 回收动物到对象池
            ReturnAnimal(args.Animal);
        }

        /// <summary>
        /// 动物逃跑事件回调
        /// </summary>
        private void OnAnimalFled(AnimalFledEventArgs args)
        {
            if (args.Animal == null)
                return;

            // 回收动物到对象池
            ReturnAnimal(args.Animal);
        }
        #endregion     
    }
}
