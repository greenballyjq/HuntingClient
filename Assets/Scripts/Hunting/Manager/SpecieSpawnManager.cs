using System.Collections.Generic;
using GameFramework.Core;
using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 物种派发管理器
    /// </summary>
    public class SpecieSpawnManager : BaseGameManager
    {
        /// <summary>
        /// 派发器列表
        /// </summary>
        private readonly List<SpecieSpawner> _spawners = new List<SpecieSpawner>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 初始化管理器
        /// </summary>
        public override void Init()
        {
            RegisterEvents();
            Debug.Log("[SpecieSpawnManager] 初始化完成");
        }

        /// <summary>
        /// 每帧更新
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
            Debug.Log("[SpecieSpawnManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 根据索引获取派发器
        /// </summary>
        public SpecieSpawner GetSpawner(int index)
        {
            if (index < 0 || index >= _spawners.Count)
                return null;

            return _spawners[index];
        }
        /// <summary>
        /// 设置单个派发器的启用状态
        /// </summary>
        public void SetSpawnerActive(SpecieSpawner spawner, bool active)
        {
            if (spawner == null)
                return;

            if (spawner.IsActive == active)
                return;

            Debug.Log($"[SpeciesSpawnManager] 设置派发器 {spawner.name} 的启用状态为 {active}");

            spawner.SetActive(active);
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
        /// 设置单个派发器的地图 ID
        /// </summary>
        public void SetSpawnerMap(SpecieSpawner spawner, int mapId)
        {
            if (spawner == null)
                return;

            spawner.SetMap(mapId);
        }

        /// <summary>
        /// 设置所有派发器的地图 ID
        /// </summary>
        public void SetAllMap(int mapId)
        {
            for (int i = 0; i < _spawners.Count; i++)
                _spawners[i].SetMap(mapId);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 收集场景中的派发器
        /// </summary>
        private void CollectSpawners()
        {
            SpecieSpawner[] found = Object.FindObjectsOfType<SpecieSpawner>(true);
            _spawners.AddRange(found);
            Debug.Log($"[SpeciesSpawnManager] 收集到派发器数量：{_spawners.Count}");
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener("GameStarted", OnGameStarted);
            Event.AddListener("GamePaused", OnGamePaused);
            Event.AddListener("GameResumed", OnGameResumed);
            Event.AddListener("GameEnded", OnGameEnded);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener("GameStarted", OnGameStarted);
            Event.RemoveListener("GamePaused", OnGamePaused);
            Event.RemoveListener("GameResumed", OnGameResumed);
            Event.RemoveListener("GameEnded", OnGameEnded);
        }

        /// <summary>
        /// 游戏开始回调
        /// </summary>
        private void OnGameStarted()
        {
            CollectSpawners();
            SetAllActive(true);
        }

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
        #endregion
    }
}

