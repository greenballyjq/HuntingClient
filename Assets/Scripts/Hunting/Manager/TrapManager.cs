using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Pool;
using Hunting.Game.Props;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 陷阱管理器
    /// </summary>
    public class TrapManager : BaseGameManager
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 对象池管理器
        /// </summary>
        private GameObjectPoolManager Pool => GameServiceLocator.Pool;

        /// <summary>
        /// 场上所有陷阱列表
        /// </summary>
        private List<GameObject> _activeTraps = new List<GameObject>();

        public override void Init()
        {
            RegisterEvents();
            Debug.Log("[TrapManager] 初始化完成");
        }

        public override void Update() { }

        public override void Release()
        {
            UnregisterEvents();
            ClearAllTraps();
            Debug.Log("[TrapManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 创建陷阱
        /// </summary>
        /// <param name="position">陷阱位置</param>
        /// <param name="attractRadius">吸引半径</param>
        /// <param name="triggerRadius">触发半径</param>
        /// <param name="attractRadiusRangeByVolume">按体型划分的吸引半径范围比例</param>
        /// <param name="prefabPath">预制体资源路径</param>
        /// <returns>陷阱游戏对象</returns>
        public async UniTask<GameObject> CreateTrapAsync(
            Vector3 position,
            float attractRadius,
            float triggerRadius,
            Dictionary<EVolumeType, float[]> attractRadiusRangeByVolume,
            string prefabPath
        )
        {
            // 从对象池获取
            GameObject trap = await Pool.SpawnAsync(prefabPath);

            // 初始化陷阱
            IPoolItem poolItem = trap.GetComponent<IPoolItem>();

            trap.transform.position = position;
            trap.transform.rotation = Quaternion.identity;

            TrapBehavior trapBehavior = trap.GetComponent<TrapBehavior>();
            trapBehavior.Init(attractRadius, triggerRadius, attractRadiusRangeByVolume);

            // 注册到列表
            _activeTraps.Add(trap);

            return trap;
        }

        /// <summary>
        /// 获取所有陷阱位置
        /// </summary>
        /// <returns>陷阱位置列表</returns>
        public List<Vector3> GetAllTrapPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (GameObject trap in _activeTraps)
                positions.Add(trap.transform.position);

            return positions;
        }

        /// <summary>
        /// 清理所有陷阱
        /// </summary>
        private void ClearAllTraps()
        {
            foreach (GameObject trap in _activeTraps)
                Pool.Despawn(trap);

            _activeTraps.Clear();
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(PropEvents.TrapTriggered, OnTrapTriggered);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(PropEvents.TrapTriggered, OnTrapTriggered);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 陷阱触发事件回调
        /// </summary>
        private void OnTrapTriggered(TrapTriggeredEventArgs args)
        {
            _activeTraps.Remove(args.Trap.gameObject);
            Pool.Despawn(args.Trap.gameObject);
        }

        /// <summary>
        /// 单局结束事件回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            ClearAllTraps();
        }
        #endregion
    }
}

