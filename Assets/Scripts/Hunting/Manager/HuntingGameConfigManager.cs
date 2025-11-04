using cfg;
using cfg.HuntingConfig;
using GameFramework.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 打猎游戏专用配置管理器
    /// </summary>
    public class HuntingGameConfigManager : ConfigManager<HuntingGameConfigManager>
    {
        /// <summary>
        /// 数值表名称列表
        /// </summary>
        protected override List<string> TableNames => new List<string>
        {
            "huntingconfig_tbbullet",
            "huntingconfig_tbspecie",
            "huntingconfig_tbspawn",
            "huntingconfig_tbmap",
            "huntingconfig_tbprogress",
            "huntingconfig_tbdynamicquest"
        };

        #region 数值表访问
        /// <summary>
        /// 子弹数值表
        /// </summary>
        public TbBullet BulletTable => _tables?.TbBullet;

        /// <summary>
        /// 物种数值表
        /// </summary>
        public TbSpecie SpecieTable => _tables?.TbSpecie;

        /// <summary>
        /// 派发数值表
        /// </summary>
        public TbSpawn SpawnTable => _tables?.TbSpawn;

        /// <summary>
        /// 地图数值表
        /// </summary>
        public TbMap MapTable => _tables?.TbMap; 

        /// <summary>
        /// 角色数值表
        /// </summary>
        public TbRole RoleTable => _tables?.TbRole;

        /// <summary>
        /// 全局进度数值表
        /// </summary>
        public TbProgress ProgressTable => _tables?.TbProgress;

        /// <summary>
        /// 动态任务数值表
        /// </summary>
        public TbDynamicQuest DynamicQuestTable => _tables?.TbDynamicQuest;
        #endregion

        #region 数值表单个数据项访问
        /// <summary>
        /// 获取单个子弹数据
        /// </summary>
        /// <param name="id">子弹ID</param>
        /// <returns></returns>
        public Bullet GetBullet(int id) => BulletTable.Get(id);

		/// <summary>
		/// 通过子弹类型获取单个子弹数据
		/// </summary>
		/// <param name="type">子弹类型</param>
		/// <returns></returns>
		public Bullet GetBullet(EBulletType type)
			=> BulletTable?.DataList?.FirstOrDefault(b => b.BulletType == type);
        
        /// <summary>
        /// 获取单个物种数据
        /// </summary>
        /// <param name="id">物种ID</param>
        /// <returns></returns>
        public Specie GetSpecie(int id) => SpecieTable.Get(id);

        /// <summary>
        /// 获取单个派发数据
        /// </summary>
        /// <param name="id">派发ID</param>
        /// <returns></returns>
        public Spawn GetSpawn(int id) => SpawnTable.Get(id);

        /// <summary>
        /// 获取单个地图数据
        /// </summary>
        /// <param name="id">地图ID</param>
        /// <returns></returns>
        public Map GetMap(int id) => MapTable.Get(id);

        /// <summary>
        /// 通过地图类型获取单个地图数据
        /// </summary>
        /// <param name="type">地图类型</param>
        /// <returns></returns>
        public Map GetMap(EMapType type)
             => MapTable?.DataList?.FirstOrDefault(p => p.MapType == type);

        /// <summary>
        /// 获取单个角色数据
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public Role GetRole(int id) => RoleTable.Get(id);

        /// <summary>
        /// 通过角色类型获取单个角色数据
        /// </summary>
        /// <param name="type">角色类型</param>
        /// <returns></returns>
        public Role GetRole(ERoleType type)
            => RoleTable?.DataList?.FirstOrDefault(p => p.RoleType == type);

        /// <summary>
        /// 获取单个全局进度条数据
        /// </summary>
        /// <param name="id">进度条ID</param>
        /// <returns></returns>
        public Progress GetProgress(int id) => ProgressTable.Get(id);

		/// <summary>
		/// 通过进度条类型获取单个进度条数据
		/// </summary>
		/// <param name="type">进度条类型</param>
		/// <returns></returns>
		public Progress GetProgress(EProgressType type)
			=> ProgressTable?.DataList?.FirstOrDefault(p => p.ProgressType == type);

        /// <summary>
        /// 获取单个动态任务数据
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        public DynamicQuest GetDynamicQuest(int id) => DynamicQuestTable.Get(id);

		/// <summary>
		/// 通过任务类型获取单个动态任务数据
		/// </summary>
		/// <param name="type">任务类型</param>
		/// <returns></returns>
		public DynamicQuest GetDynamicQuest(EDynamicQuestType type)
			=> DynamicQuestTable?.DataList?.FirstOrDefault(q => q.DynamicQuestType == type);
        #endregion

        #region 子弹相关特殊方法
        /// <summary>
        /// 获取所有特殊子弹的数据
        /// </summary>
        /// <returns></returns>
        public List<Bullet> GetAllSpecialBullets()
        => _tables?.TbBullet?.DataList?.Where(b => b.BulletType != EBulletType.Normal).ToList();

        /// <summary>
        /// 随机获取一个特殊子弹的数据
        /// </summary>
        /// <returns></returns>
        public Bullet GetRandomSpecialBullet()
        {
            var specialBullets = GetAllSpecialBullets();
            return specialBullets?.Count > 0 ? specialBullets[Random.Range(0, specialBullets.Count)] : null;
        }
        #endregion

        #region 物种相关特殊方法
        /// <summary>
        /// 根据地图与体型，在对应物种池内按权重随机选择一个物种
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="volumeType">体型</param>
        /// <returns>选中的物种</returns>
        public Specie GetRandomSpecieByMapAndVolume(int mapId, EVolumeType volumeType)
        {
			var map = GetMap(mapId);
			if (map == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetRandomSpecieByMapAndVolume: 地图不存在 mapId={mapId}");
				return null;
			}
			if (map.SpeciesByVolume == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetRandomSpecieByMapAndVolume: 地图未配置物种池 mapId={mapId}");
				return null;
			}
			if (!map.SpeciesByVolume.TryGetValue(volumeType, out var specieWeights) || specieWeights == null || specieWeights.Length == 0)
			{
				Debug.LogWarning($"[HuntingGameConfigManager] GetRandomSpecieByMapAndVolume: 该体型在地图下无候选物种 mapId={mapId}, volumeType={volumeType}");
				return null;
			}

			// 计算该体型物种池的总权重
			float totalWeight = 0f;
			for (int i = 0; i < specieWeights.Length; i++)
			{
				float w = specieWeights[i].Weight;
				if (w > 0f) totalWeight += w;
			}
			if (totalWeight <= 0f)
			{
				Debug.LogWarning($"[HuntingGameConfigManager] GetRandomSpecieByMapAndVolume: 该体型所有权重<=0 mapId={mapId}, volumeType={volumeType}");
				// 返回第一个有效物种以避免中断
				for (int i = 0; i < specieWeights.Length; i++)
				{
					var fallback = GetSpecie(specieWeights[i].SpecieId);
					if (fallback != null) return fallback;
				}
				return null;
			}

			// 在 [0, totalWeight] 上取随机点并累加命中
			float randomPoint = Random.Range(0f, totalWeight);
			float cumulative = 0f;
			for (int i = 0; i < specieWeights.Length; i++)
			{
				float w = specieWeights[i].Weight;
				if (w <= 0f) continue;
				cumulative += w;
				if (randomPoint <= cumulative)
				{
					return GetSpecie(specieWeights[i].SpecieId);
				}
			}

			// 理论上不会到这里，兜底返回首个有效
			Debug.LogWarning($"[HuntingGameConfigManager] GetRandomSpecieByMapAndVolume: 未命中加权范围，使用首个有效作为兜底 mapId={mapId}, volumeType={volumeType}");
			return GetSpecie(specieWeights[0].SpecieId);
        }
        #endregion

        #region 派发相关特殊方法
        /// <summary>
        /// 根据地图的体型策略按比例随机选择一种体型
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>选中的体型</returns>
        public EVolumeType GetRandomVolumeTypeByStrategy(int mapId)
        {
			var strategy = GetMapSpawnStrategy(mapId);
			if (strategy == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetRandomVolumeTypeByStrategy: 体型策略不存在 mapId={mapId}");
				return EVolumeType.Small;
			}
			if (strategy.VolumeRatio == null || strategy.VolumeRatio.Count == 0)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetRandomVolumeTypeByStrategy: 策略未配置体型比例 mapId={mapId}");
				return EVolumeType.Small;
			}

			// 仅对策略中配置的体型做加权
			float totalRatio = 0f;
			foreach (var kv in strategy.VolumeRatio)
			{
				if (kv.Value > 0f) totalRatio += kv.Value;
			}
			if (totalRatio <= 0f)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetRandomVolumeTypeByStrategy: 所有体型比例<=0 mapId={mapId}");
				return EVolumeType.Small;
			}

			float randomPoint = Random.Range(0f, totalRatio);
			float cumulative = 0f;
			foreach (var kv in strategy.VolumeRatio)
			{
				float ratio = kv.Value;
				if (ratio <= 0f) continue;
				cumulative += ratio;
				if (randomPoint <= cumulative)
					return kv.Key;
			}

			// 兜底选择第一个键
			Debug.LogWarning($"[HuntingGameConfigManager] GetRandomVolumeTypeByStrategy: 未命中加权范围，使用第一个体型兜底 mapId={mapId}");
			foreach (var kv in strategy.VolumeRatio)
				return kv.Key;
			return EVolumeType.Small;
        }

        /// <summary>
        /// 获取地图下某体型的驻场时间
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="volumeType">体型</param>
        /// <returns>驻场时间（秒）</returns>
        public float GetStayTimeByVolumeType(int mapId, EVolumeType volumeType)
        {
			var strategy = GetMapSpawnStrategy(mapId);
			if (strategy == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetStayTimeByVolumeType: 体型策略不存在 mapId={mapId}");
				return 10f;
			}
			if (strategy.StayTime == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetStayTimeByVolumeType: 策略未配置驻场时间 mapId={mapId}");
				return 10f;
			}
			if (strategy.StayTime.TryGetValue(volumeType, out var stayTime))
				return stayTime;

			Debug.LogWarning($"[HuntingGameConfigManager] GetStayTimeByVolumeType: 该体型未配置驻场时间 mapId={mapId}, volumeType={volumeType}，使用默认值");
			return 10f;
        }
        #endregion

        #region 地图相关特殊方法
        /// <summary>
        /// 获取地图使用的体型策略（体型比例与驻场时间）
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>体型策略</returns>
        public Spawn GetMapSpawnStrategy(int mapId)
        {
			var map = GetMap(mapId);
			if (map == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetMapSpawnStrategy: 地图不存在 mapId={mapId}");
				return null;
			}
			var strategy = GetSpawn(map.SpawnStrategyId);
			if (strategy == null)
			{
				Debug.LogError($"[HuntingGameConfigManager] GetMapSpawnStrategy: 体型策略不存在 mapId={mapId}, strategyId={map.SpawnStrategyId}");
			}
			return strategy;
        }
        #endregion

        #region 全局进度相关特殊方法
        #endregion

        #region 动态任务相关特殊方法
        #endregion

        #region 业务需求方法
        /// <summary>
        /// 综合方法：随机派发一只物种（基于地图、体型策略、体型内物种权重）
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>物种数据与驻场时间</returns>
        public (Specie specie, float stayTime) GetRandomSpecieForMap(int mapId)
        {
			// 1) 体型：按地图体型策略的比例加权随机
			EVolumeType volumeType = GetRandomVolumeTypeByStrategy(mapId);

			// 2) 物种：在该体型下按权重随机选一个
			var specie = GetRandomSpecieByMapAndVolume(mapId, volumeType);
			if (specie == null)
			{
				Debug.LogWarning($"[HuntingGameConfigManager] GetRandomSpecieForMap: 未选出物种 mapId={mapId}, volumeType={volumeType}");
			}

			// 3) 驻场：从地图的体型策略获取
			float stayTime = GetStayTimeByVolumeType(mapId, volumeType);

			return (specie, stayTime);
        }
        #endregion
    }
}