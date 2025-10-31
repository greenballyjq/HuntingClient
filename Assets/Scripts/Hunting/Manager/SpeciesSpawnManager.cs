using System.Collections.Generic;
using GameFramework.Core;
using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 物种派发管理器
    /// </summary>
    public class SpeciesSpawnManager : BaseGameManager
    {
        /// <summary>
        /// 派发器列表
        /// </summary>
        private readonly List<SpeciesSpawner> _spawners = new List<SpeciesSpawner>();

        /// <summary>
        /// 派发器列表
        /// </summary>
        public List<SpeciesSpawner> Spawners => _spawners;

        /// <summary>
        /// 事件中心
        /// </summary>
        private EventManager Events => GameServiceLocator.Event;

        /// <summary>
        /// 初始化管理器
        /// </summary>
        public override void Init()
        {
            RegisterEvents();
        }

        /// <summary>
        /// 每帧更新（当前无需求）
        /// </summary>
        public override void Update()
        {
        }

        /// <summary>
        /// 释放管理器
        /// </summary>
        public override void Release()
        {
            UnregisterEvents();
            _spawners.Clear();
        }

        /// <summary>
        /// 根据索引获取派发器
        /// </summary>
        public SpeciesSpawner GetSpawner(int index)
        {
            if (index < 0 || index >= _spawners.Count)
            {
                return null;
            }
            return _spawners[index];
        }

        /// <summary>
        /// 重新收集场景中的派发器
        /// </summary>
        public void RefreshSpawners()
        {
            _spawners.Clear();
            CollectSpawners();
            OnSpawnManagerReady();
        }

        /// <summary>
        /// 设置所有派发器的启用状态
        /// </summary>
        public void SetAllActive(bool active)
        {
            for (int i = 0; i < _spawners.Count; i++)
                SetSpawnerActive(_spawners[i], active);
        }

        /// <summary>
        /// 设置单个派发器的启用状态
        /// </summary>
        public void SetSpawnerActive(SpeciesSpawner spawner, bool active)
        {
            if (spawner == null)
                return;

            if (spawner.IsActive == active)
                return;

            spawner.SetActive(active);

            var args = new SpawnerActiveChangedEventArgs
            {
                Sender = this,
                Spawner = spawner,
                IsActive = active
            };
            Events.Trigger(SpawnEvents.SpawnerActiveChanged, args);
        }

        /// <summary>
        /// 设置所有派发器的地图 ID
        /// </summary>
        public void SetAllMap(int mapId)
        {
            for (int i = 0; i < _spawners.Count; i++)
                _spawners[i].SetMap(mapId);
        }

        /// <summary>
        /// 设置单个派发器的地图 ID
        /// </summary>
        public void SetSpawnerMap(SpeciesSpawner spawner, int mapId)
        {
            if (spawner == null)
                return;

            spawner.SetMap(mapId);
        }

        /// <summary>
        /// 管理器准备完成事件
        /// </summary>
        private void OnSpawnManagerReady()
        {
            var args = new SpawnManagerReadyEventArgs
            {
                Sender = this,
                SpawnerCount = _spawners.Count
            };
            Events.Trigger(SpawnEvents.SpawnManagerReady, args);
        }

        /// <summary>
        /// 收集场景中的派发器
        /// </summary>
        private void CollectSpawners()
        {
            SpeciesSpawner[] found = Object.FindObjectsOfType<SpeciesSpawner>(true);
            _spawners.AddRange(found);
            Debug.Log($"[SpeciesSpawnManager] 收集到派发器数量：{_spawners.Count}");
        }

        /// <summary>
        /// 注册游戏生命周期事件
        /// </summary>
        private void RegisterEvents()
        {
            Events.AddListener(HuntingEvents.HuntingGameStarted,OnHuntingGameStarted);
            Events.AddListener("GameStarted", OnGameStarted);
            Events.AddListener("GamePaused", OnGamePaused);
            Events.AddListener("GameResumed", OnGameResumed);
            Events.AddListener("GameEnded", OnGameEnded);
        }

        /// <summary>
        /// 注销游戏生命周期事件
        /// </summary>
        private void UnregisterEvents()
        {
            Events.RemoveListener("GameStarted", OnGameStarted);
            Events.RemoveListener("GamePaused", OnGamePaused);
            Events.RemoveListener("GameResumed", OnGameResumed);
            Events.RemoveListener("GameEnded", OnGameEnded);
        }

        private void OnHuntingGameStarted()
        {
            CollectSpawners();
            SetAllActive(true);
        }

        /// <summary>
        /// 游戏开始回调
        /// </summary>
        private void OnGameStarted(){}

        /// <summary>
        /// 游戏暂停回调
        /// </summary>
        private void OnGamePaused()
        {
            SetAllActive(false);
        }

        /// <summary>
        /// 游戏恢复回调
        /// </summary>
        private void OnGameResumed()
        {
            SetAllActive(true);
        }

        /// <summary>
        /// 游戏结束回调
        /// </summary>
        private void OnGameEnded()
        {
            SetAllActive(false);
        }
    }
}

