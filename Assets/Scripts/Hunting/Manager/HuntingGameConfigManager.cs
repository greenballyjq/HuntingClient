using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
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
            "huntingconfig_tbrole",
            "huntingconfig_tbmeatprogress",
            "huntingconfig_tbenergyprogress",
            "huntingconfig_tbquest",
            "huntingconfig_skill_tbskill",
            "huntingconfig_skill_tbskill3kp",
            "huntingconfig_skill_tbskillziwei",
            "huntingconfig_skill_tbskilldameili",
            "huntingconfig_skill_tbskilljinzhuangyuan",
            "huntingconfig_skill_tbskillyakedong",
            "huntingconfig_tblucky",
        };

        #region 数值表访问
        /// <summary>
        /// 子弹数值表
        /// </summary>
        public TbBullet BulletTable => _tables.TbBullet;

        /// <summary>
        /// 物种数值表
        /// </summary>
        public TbSpecie SpecieTable => _tables.TbSpecie;

        /// <summary>
        /// 派发数值表
        /// </summary>
        public TbSpawn SpawnTable => _tables.TbSpawn;

        /// <summary>
        /// 地图数值表
        /// </summary>
        public TbMap MapTable => _tables.TbMap; 

        /// <summary>
        /// 角色数值表
        /// </summary>
        public TbRole RoleTable => _tables.TbRole;

        /// <summary>
        /// 肉度条数值表
        /// </summary>
        public TbMeatProgress MeatProgressTable => _tables.TbMeatProgress;

        /// <summary>
        /// 丰收能量条数值表
        /// </summary>
        public TbEnergyProgress EnergyProgressTable => _tables.TbEnergyProgress;

        /// <summary>
        /// 任务数值表
        /// </summary>
        public TbQuest QuestTable => _tables.TbQuest;

        /// <summary>
        /// 技能主表
        /// </summary>
        public TbSkill SkillTable => _tables.TbSkill;

        /// <summary>
        /// 色块人技能数值表
        /// </summary>
        public TbSkill3KP Skill3KPTable => _tables.TbSkill3KP;

        /// <summary>
        /// 紫薇技能数值表
        /// </summary>
        public TbSkillZiWei SkillZiWeiTable => _tables.TbSkillZiWei;

        /// <summary>
        /// 大美丽技能数值表
        /// </summary>
        public TbSkillDaMeiLi SkillDaMeiLiTable => _tables.TbSkillDaMeiLi;

        /// <summary>
        /// 金状元技能数值表
        /// </summary>
        public TbSkillJinZhuangYuan SkillJinZhuangYuanTable => _tables.TbSkillJinZhuangYuan;

        /// <summary>
        /// 亚克东技能数值表
        /// </summary>
        public TbSkillYaKeDong SkillYaKeDongTable => _tables.TbSkillYaKeDong;

        /// <summary>
        /// 幸运仪式数值表
        /// </summary>
        public TbLucky LuckyTable => _tables.TbLucky;
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
			=> BulletTable.DataList.FirstOrDefault(b => b.BulletType == type);
        
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
             => MapTable.DataList.FirstOrDefault(p => p.MapType == type);

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
            => RoleTable.DataList.FirstOrDefault(p => p.RoleType == type);

        /// <summary>
        /// 获取单个肉度条数据
        /// </summary>
        /// <param name="id">肉度条ID</param>
        /// <returns></returns>
        public MeatProgress GetMeatProgress(int id) => MeatProgressTable.Get(id);

        /// <summary>
        /// 获取单个丰收能量条数据
        /// </summary>
        /// <param name="id">丰收能量条ID</param>
        /// <returns></returns>
        public EnergyProgress GetEnergyProgress(int id) => EnergyProgressTable.Get(id);

        /// <summary>
        /// 获取单个任务数据
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        public Quest GetDynamicQuest(int id) => QuestTable.Get(id);

        /// <summary>
        /// 获取单个幸运仪式配置
        /// </summary>
        /// <param name="id">增益ID</param>
        /// <returns></returns>
        public Lucky GetLucky(int id) => LuckyTable.Get(id);

        /// <summary>
        /// 通过幸运仪式类型获取单个配置
        /// </summary>
        /// <param name="type">增益类型</param>
        /// <returns></returns>
        public Lucky GetLucky(ELuckyType type)
            => LuckyTable.DataList.FirstOrDefault(p => p.LuckyType == type);

		/// <summary>
        /// 获取单个技能配置
        /// </summary>
        /// <param name="id">技能ID</param>
        /// <returns></returns>
        public Skill GetSkill(int id) => SkillTable.Get(id);

        /// <summary>
        /// 通过技能类型获取单个技能配置
        /// </summary>
        /// <param name="type">技能类型</param>
        /// <returns></returns>
        public Skill GetSkill(ESkillType type)
            => SkillTable.DataList.FirstOrDefault(p => p.SkillType == type);

        /// <summary>
        /// 获取色块人技能参数
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <returns></returns>
        public Skill3KP GetSkill3KP(int id) => Skill3KPTable.Get(id);

        /// <summary>
        /// 获取紫薇技能参数
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <returns></returns>
        public SkillZiWei GetSkillZiWei(int id) => SkillZiWeiTable.Get(id);

        /// <summary>
        /// 获取大美丽技能参数
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <returns></returns>
        public SkillDaMeiLi GetSkillDaMeiLi(int id) => SkillDaMeiLiTable.Get(id);

        /// <summary>
        /// 获取金状元技能参数
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <returns></returns>
        public SkillJinZhuangYuan GetSkillJinZhuangYuan(int id)
            => SkillJinZhuangYuanTable.Get(id);

        /// <summary>
        /// 获取亚克东技能参数
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <returns></returns>
        public SkillYaKeDong GetSkillYaKeDong(int id) => SkillYaKeDongTable.Get(id);

		/// <summary>
		/// 通过任务类型获取单个任务数据
		/// </summary>
		/// <param name="type">任务类型</param>
		/// <returns></returns>
		public Quest GetDynamicQuest(EQuestType type)
			=> QuestTable.DataList.FirstOrDefault(q => q.QuestType == type);
        #endregion

        #region 角色相关特殊方法
        #endregion

        #region 地图相关特殊方法
        /// <summary>
        /// 获取地图使用的体型策略（体型比例与驻场时间）
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>体型策略</returns>
        public Spawn GetMapSpawnStrategy(int mapId)
        => GetSpawn(GetMap(mapId).SpawnStrategyId);
        #endregion

        #region 子弹相关特殊方法
        /// <summary>
        /// 获取所有特殊子弹的数据
        /// </summary>
        /// <returns></returns>
        public List<Bullet> GetAllSpecialBullets()
        => BulletTable.DataList.Where(b => b.BulletType != EBulletType.Normal).ToList();

        /// <summary>
        /// 随机获取一个特殊子弹的数据
        /// </summary>
        /// <returns></returns>
        public Bullet GetRandomSpecialBullet()
        => GetAllSpecialBullets()[Random.Range(0, GetAllSpecialBullets().Count)];
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
			// 取得当前地图该体型对应的物种权重列表
			var specieWeights = GetMap(mapId).SpeciesByVolume[volumeType];
            
			// 累加权重用于后续随机
			float totalWeight = 0f;
			for (int i = 0; i < specieWeights.Length; i++)
			{
				float w = specieWeights[i].Weight;
				if (w > 0f) totalWeight += w;
			}
            
			// 在总权重范围内取随机点
			float randomPoint = Random.Range(0f, totalWeight);
			float cumulative = 0f;
			for (int i = 0; i < specieWeights.Length; i++)
			{
				float w = specieWeights[i].Weight;
				if (w <= 0f) continue;
                
				// 累加权重，一旦超过随机点即命中
				cumulative += w;
				if (randomPoint <= cumulative)
				{
					return GetSpecie(specieWeights[i].SpecieId);
				}
			}
			return null;
        }

        /// <summary>
        /// 随机派发一只物种（基于地图、体型策略、体型内物种权重）
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>物种数据与驻场时间</returns>
        public (Specie specie, float stayTime) GetRandomSpecieForMap(int mapId)
        {
            // 1) 体型：按地图体型策略的比例加权随机
            EVolumeType volumeType = GetRandomVolumeTypeByStrategy(mapId);

            // 2) 物种：在该体型下按权重随机选一个
            var specie = GetRandomSpecieByMapAndVolume(mapId, volumeType);

            // 3) 驻场：从地图的体型策略获取
            float stayTime = GetStayTimeByVolumeType(mapId, volumeType);

            return (specie, stayTime);
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
			// 获取地图配置的体型策略
			var strategy = GetMapSpawnStrategy(mapId);
            
			// 计算所有体型比例总和
			float totalRatio = strategy.VolumeRatio.Values.Sum();
            
			// 取随机点并根据权重命中体型
			float randomPoint = Random.Range(0f, totalRatio);
			float cumulative = 0f;
			foreach (var kv in strategy.VolumeRatio)
			{
				float ratio = kv.Value;
				cumulative += ratio;
				if (randomPoint <= cumulative)
					return kv.Key;
			}
            
			return default;
        }

        /// <summary>
        /// 获取地图下某体型的驻场时间
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="volumeType">体型</param>
        /// <returns>驻场时间（秒）</returns>
        public float GetStayTimeByVolumeType(int mapId, EVolumeType volumeType)
        => GetMapSpawnStrategy(mapId).StayTime[volumeType];
        #endregion

        #region 肉度条相关特殊方法
        /// <summary>
        /// 根据完成的肉度条数量获取默认肉度条奖励
        /// </summary>
        /// <param name="completedBars">已完成的肉度条数量</param>
        /// <returns>对应的奖励配置</returns>
        public MeatProgressReward GetMeatProgressReward(int completedBars)
        => GetMeatProgress(1).RewardSteps[completedBars];
        #endregion

        #region 能量条相关特殊方法
        #endregion

        #region 技能相关特殊方法
        #endregion

        #region 任务相关特殊方法
        #endregion
    }
}