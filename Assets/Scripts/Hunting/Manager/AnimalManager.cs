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
        /// 事件中心
        /// </summary>
        private EventManager Events => GameServiceLocator.Event;

        /// <summary>
        /// 对象池
        /// </summary>
        private GameObjectPoolManager Pool => GameServiceLocator.Pool;

        public override void Init()
        {
            RegisterEvents();
            Debug.Log("[AnimalManager] 初始化完成");
        }

        public override void Update()
        {
        }

        /// <summary>
        /// 释放
        /// </summary>
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
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Events.RemoveListener(SpawnEvents.SpeciesSpawned, OnSpeciesSpawned);
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
        /// <param name="args"></param>
        private void TriggerAnimalSpawned(AnimalSpawnedEventArgs args)
        {
            Events.Trigger(AnimalEvents.AnimalSpawned, args);
        }

        /// <summary>
        /// 触发动物死亡事件
        /// </summary>
        /// <param name="args"></param>
        private void TriggerAnimalDied(AnimalDiedEventArgs args)
        {
            Events.Trigger(AnimalEvents.AnimalDied, args);
        }

        /// <summary>
        /// 触发动物逃跑事件
        /// </summary>
        /// <param name="args"></param>
        private void TriggerAnimalFled(AnimalFledEventArgs args)
        {
            Events.Trigger(AnimalEvents.AnimalFled, args);
        }

        /// <summary>
        /// 触发动物掉落奖励事件
        /// </summary>
        /// <param name="args"></param>
        private void TriggerAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            Events.Trigger(AnimalEvents.AnimalDropReward, args);
        }

        /// <summary>
        /// 动物死亡预告回调
        /// </summary>
        public void OnAnimalDiedNotice(AnimalBehavior animal, EDropType dropType, int dropAmount)
        {
            if (animal == null)
                return;

            // 先广播掉落，再广播死亡
            TriggerAnimalDropReward(new AnimalDropRewardEventArgs
            {
                Sender = this,
                Animal = animal,
                DropType = dropType,
                Amount = dropAmount
            });

            TriggerAnimalDied(new AnimalDiedEventArgs
            {
                Sender = this,
                Animal = animal,
                SpecieData = animal.specieData,
                DropType = dropType,
                DropAmount = dropAmount
            });

            ReturnAnimal(animal);
        }

        /// <summary>
        /// 动物逃跑预告回调
        /// </summary>
        public void OnAnimalFledNotice(AnimalBehavior animal)
        {
            if (animal == null)
                return;

            TriggerAnimalFled(new AnimalFledEventArgs
            {
                Sender = this,
                Animal = animal,
                SpecieData = animal.specieData,
            });

            ReturnAnimal(animal);
        }
        #endregion     
    }
}