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
            "huntingconfig_tbspeciespawn",
            "huntingconfig_tbmapspecie",
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
        public TbSpecie SpeciesTable => _tables?.TbSpecie;
        /// <summary>
        /// 物种派发数值表
        /// </summary>
        public TbSpecieSpawn SpeciesSpawnTable => _tables?.TbSpecieSpawn;
        /// <summary>
        /// 地图与物种映射数值表
        /// </summary>
        public TbMapSpecie MapSpeciesTable => _tables?.TbMapSpecie;
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
        /// 获取单个物种数据
        /// </summary>
        /// <param name="id">物种ID</param>
        /// <returns></returns>
        public Specie GetSpecie(int id) => SpeciesTable.Get(id);
        /// <summary>
        /// 获取单个物种派发数据，如【小型动物怎么派发】。
        /// </summary>
        /// <param name="volumeType">物种体积类型</param>
        /// <returns></returns>
        public SpecieSpawn GetSpeciesSpawn(EVolumeType volumeType) => SpeciesSpawnTable.Get(volumeType);
        /// <summary>
        /// 获取单个地图与物种映射数据，如【XX地图有XX动物】
        /// </summary>
        /// <param name="id">地图ID</param>
        /// <returns></returns>
        public MapSpecie GetMapSpecies(int id) => MapSpeciesTable.Get(id);
        /// <summary>
        /// 获取单个全局进度条数据，如【血条包】
        /// </summary>
        /// <param name="progressType">进度条类型</param>
        /// <returns></returns>
        public Progress GetProgress(EProgressType progressType) => ProgressTable.Get(progressType);
        /// <summary>
        /// 获取单个动态任务数据，如【打猎大型猎物】
        /// </summary>
        /// <param name="id">动态任务ID</param>
        /// <returns></returns>
        public DynamicQuest GetDynamicQuest(int id) => DynamicQuestTable.Get(id);
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

        #region 物种派发相关特殊方法

        /// <summary>
        /// 根据体型随机选择一种体型（考虑派发比例）
        /// </summary>
        /// <returns>选中的体型类型</returns>
        public EVolumeType GetRandomVolumeTypeByRatio()
        {
            var spawnConfigs = SpeciesSpawnTable?.DataList;
            if (spawnConfigs == null || spawnConfigs.Count == 0)
                return EVolumeType.Small;

            // 计算总权重
            float totalWeight = spawnConfigs.Sum(config => config.SpawnRatio);

            // 随机选择
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var config in spawnConfigs)
            {
                currentWeight += config.SpawnRatio;
                if (randomValue <= currentWeight)
                {
                    return config.VolumeType;
                }
            }

            return EVolumeType.Small; // 默认返回小型
        }

        /// <summary>
        /// 根据地图和体型获取该体型下所有可能的物种ID
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="volumeType">体型类型</param>
        /// <returns>物种ID数组</returns>
        public int[] GetSpecieIdsByMapAndVolume(int mapId, EVolumeType volumeType)
        {
            var mapSpecie = GetMapSpecies(mapId);
            if (mapSpecie?.SpeciesByVolume == null)
                return new int[0];

            return mapSpecie.SpeciesByVolume.TryGetValue(volumeType, out var specieIds)
                ? specieIds
                : new int[0];
        }

        /// <summary>
        /// 根据地图和体型随机选择一个物种（考虑出现概率）
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="volumeType">体型类型</param>
        /// <returns>选中的物种数据</returns>
        public Specie GetRandomSpecieByMapAndVolume(int mapId, EVolumeType volumeType)
        {
            var specieIds = GetSpecieIdsByMapAndVolume(mapId, volumeType);
            if (specieIds.Length == 0)
                return null;

            // 获取所有可能的物种数据
            var possibleSpecies = specieIds
                .Select(id => GetSpecie(id))
                .Where(specie => specie != null)
                .ToList();

            if (possibleSpecies.Count == 0)
                return null;

            // 计算总权重
            float totalWeight = possibleSpecies.Sum(specie => specie.SpawnWeight);

            // 随机选择
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var specie in possibleSpecies)
            {
                currentWeight += specie.SpawnWeight;
                if (randomValue <= currentWeight)
                {
                    return specie;
                }
            }

            return possibleSpecies[0]; // 默认返回第一个
        }

        /// <summary>
        /// 获取体型的驻场时间
        /// </summary>
        /// <param name="volumeType">体型类型</param>
        /// <returns>驻场时间（秒）</returns>
        public float GetStayTimeByVolumeType(EVolumeType volumeType)
        {
            var spawnConfig = GetSpeciesSpawn(volumeType);
            return spawnConfig?.StayTime ?? 10f; // 默认10秒
        }

        /// <summary>
        /// 综合方法：随机生成一个物种数据（考虑地图、体型比例、物种概率）
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>物种数据和驻场时间</returns>
        public (Specie specie, float stayTime) GetRandomSpecieForMap(int mapId)
        {
            // 1. 随机选择体型（考虑派发比例）
            EVolumeType volumeType = GetRandomVolumeTypeByRatio();

            // 2. 根据体型和地图随机选择物种
            Specie specie = GetRandomSpecieByMapAndVolume(mapId, volumeType);

            // 3. 获取该体型的驻场时间
            float stayTime = GetStayTimeByVolumeType(volumeType);

            return (specie, stayTime);
        }

        #endregion

        #region 地图与物种映射相关特殊方法

        /// <summary>
        /// 获取所有地图数据
        /// </summary>
        /// <returns>地图数据列表</returns>
        public List<MapSpecie> GetAllMaps()
        {
            return MapSpeciesTable?.DataList?.ToList() ?? new List<MapSpecie>();
        }

        /// <summary>
        /// 获取默认地图（ID为1的地图）
        /// </summary>
        /// <returns>默认地图数据</returns>
        public MapSpecie GetDefaultMap()
        {
            return GetMapSpecies(1);
        }

        #endregion

        #region 地图与物种映射相关特殊方法
        #endregion

        #region 全局进度相关特殊方法
        #endregion

        #region 动态任务相关特殊方法
        #endregion


    }
}