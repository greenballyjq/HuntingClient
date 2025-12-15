using System.Collections.Generic;
using GameFramework.Core;
using GameFramework.Game;
using Hunting.Game.Animal;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 派发器管理器
    /// </summary>
    public class SpawnerManager : BaseGameManager
    {
        /// <summary>
        /// 派发器列表
        /// </summary>
        private readonly List<Spawner> _spawners = new List<Spawner>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        public override void Init()
        {
            RegisterEvents();
            Debug.Log("[SpecieSpawnManager] 初始化完成");
        }

        public override void Update()
        {
        }

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
        public Spawner GetSpawner(int index)
        {
            if (index < 0 || index >= _spawners.Count)
                return null;

            return _spawners[index];
        }
        /// <summary>
        /// 设置单个派发器的启用状态
        /// </summary>
        public void SetSpawnerActive(Spawner spawner, bool active)
        {
            if (spawner == null)
                return;

            if (spawner.IsActive == active)
                return;

            spawner.SetActive(active);
        }

        /// <summary>
        /// 设置所有派发器的启用状态
        /// </summary>
        public void SetAllActive(bool active)
        {
            for (int i = 0; i < _spawners.Count; i++)
                SetSpawnerActive(_spawners[i], active);

            Debug.Log($"[SpeciesSpawnManager] 设置所有派发器的启用状态为 {active}");
        }

        /// <summary>
        /// 设置单个派发器的地图 ID
        /// </summary>
        public void SetSpawnerMap(Spawner spawner, int mapId)
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
            Spawner[] found = Object.FindObjectsOfType<Spawner>(true);
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
            Event.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 本局开始回调
        /// </summary>
        private void OnRoundStarted(RoundStartedEventArgs args)
        {
            CollectSpawners();
            SetAllActive(true);
        }

        /// <summary>
        /// 本局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            SetAllActive(false);
        }
        #endregion
    }
}

