using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using GameFramework.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 打猎配置管理器
/// </summary>
public class HuntingConfigManager : ConfigManager<HuntingConfigManager>
{
    /// <summary>
    /// 数值表名称列表
    /// </summary>
    protected override List<string> TableNames => new List<string>
    {
        "huntingconfig_tbglobal",
        "huntingconfig_tbbullet",
        "huntingconfig_tbspecie",
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
        "huntingconfig_tbluckybuff",
        "huntingconfig_tbluckygift",
        "huntingconfig_prop_tbprop",
        "huntingconfig_prop_tbpropbombardment",
        "huntingconfig_prop_tbpropaimassist",
        "huntingconfig_prop_tbproptrap",
    };

    /// <summary>
    /// SO资源路径
    /// </summary>
    private const string PATH_SO = "Assets/Arts/SO";

    /// <summary>
    /// 掉落奖励关联资源配置
    /// </summary>
    private DropRewardRefSo _dropRewardRefSo;
    public DropRewardRefSo DropRewardRefSo => _dropRewardRefSo;

    /// <summary>
    /// 子弹关联资源配置
    /// </summary>
    private BulletRefSo _bulletRefSo;
    public BulletRefSo BulletRefSo => _bulletRefSo;

    /// <summary>
    /// 道具关联资源配置
    /// </summary>
    private PropRefSo _propRefSo;
    public PropRefSo PropRefSo => _propRefSo;

    /// <summary>
    /// 技能关联资源配置
    /// </summary>
    private SkillRefSo _skillRefSo;
    public SkillRefSo SkillRefSo => _skillRefSo;

    /// <summary>
    /// 动物关联资源配置
    /// </summary>
    private AnimalRefSo _animalRefSo;
    public AnimalRefSo _AnimalRefSo => _animalRefSo;

    /// <summary>
    /// 地图关联资源配置
    /// </summary>
    private MapRefSo _mapRefSo;
    public MapRefSo MapRefSo => _mapRefSo;

    private ResourceManager _resourceManager;

    #region 内部方法
    protected override async void InitializeAsync()
    {
        _resourceManager = GameServiceLocator.ResourceManager;
        try
        {
            Log.Info($"[{GetType().Name}] 开始加载配置表");

            await LoadTablesAsync();
            await LoadScriptableObjects();

            Initialized = true;

            Log.Info($"[{GetType().Name}] 配置表加载完成");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[{GetType().Name}] 配置加载失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载SO配置表
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadScriptableObjects()
    {
        _dropRewardRefSo = await _resourceManager.LoadAssetAsync<DropRewardRefSo>($"{PATH_SO}/DropRewardRefSo");
        _bulletRefSo = await _resourceManager.LoadAssetAsync<BulletRefSo>($"{PATH_SO}/BulletRefSo");
        _propRefSo = await _resourceManager.LoadAssetAsync<PropRefSo>($"{PATH_SO}/PropRefSo");
        _skillRefSo = await _resourceManager.LoadAssetAsync<SkillRefSo>($"{PATH_SO}/SkillRefSo");
        _animalRefSo = await _resourceManager.LoadAssetAsync<AnimalRefSo>($"{PATH_SO}/AnimalRefSo");
        _mapRefSo = await _resourceManager.LoadAssetAsync<MapRefSo>($"{PATH_SO}/MapRefSo");
    }
    #endregion

    #region 数值表访问
    /// <summary>
    /// 全局数值表
    /// </summary>
    public TbGlobal GlobalTable => _tables.TbGlobal;

    /// <summary>
    /// 子弹数值表
    /// </summary>
    public TbBullet BulletTable => _tables.TbBullet;

    /// <summary>
    /// 物种数值表
    /// </summary>
    public TbSpecie SpecieTable => _tables.TbSpecie;

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
    /// 技能数值主表
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
    /// 幸运仪式增益数值表
    /// </summary>
    public TbLuckyBuff LuckyBuffTable => _tables.TbLuckyBuff;

    /// <summary>
    /// 幸运仪式礼包数值表
    /// </summary>
    public TbLuckyGift LuckyGiftTable => _tables.TbLuckyGift;

    /// <summary>
    /// 道具数值总表
    /// </summary>
    public TbProp PropTable => _tables.TbProp;

    /// <summary>
    /// 炮火轰炸道具数值表
    /// </summary>
    public TbPropBombardment PropBombardmentTable => _tables.TbPropBombardment;

    /// <summary>
    /// 指哪打哪道具数值表
    /// </summary>
    public TbPropAimAssist PropAimAssistTable => _tables.TbPropAimAssist;

    /// <summary>
    /// 智能诱捕陷阱道具数值表
    /// </summary>
    public TbPropTrap PropTrapTable => _tables.TbPropTrap;
    #endregion

    #region 数值表单个数据项访问
    /// <summary>
    /// 获取单个子弹数据
    /// </summary>
    public Bullet GetBullet(int id) => BulletTable.Get(id);

    /// <summary>
    /// 通过子弹类型获取单个子弹数据
    /// </summary>
    public Bullet GetBullet(EBulletType type)
        => BulletTable.DataList.FirstOrDefault(b => b.BulletType == type);

    /// <summary>
    /// 获取单个物种数据
    /// </summary>
    public Specie GetSpecie(int id) => SpecieTable.Get(id);

    /// <summary>
    /// 获取单个地图数据
    /// </summary>
    public Map GetMap(int id) => MapTable.Get(id);

    /// <summary>
    /// 通过地图类型获取单个地图数据
    /// </summary>
    public Map GetMap(EMapType type)
        => MapTable.DataList.FirstOrDefault(m => m.MapType == type);

    /// <summary>
    /// 获取单个角色数据
    /// </summary>
    public Role GetRole(int id) => RoleTable.Get(id);

    /// <summary>
    /// 通过角色类型获取单个角色数据
    /// </summary>
    public Role GetRole(ERoleType type)
        => RoleTable.DataList.FirstOrDefault(r => r.RoleType == type);

    /// <summary>
    /// 获取单个肉条数据
    /// </summary>
    public MeatProgress GetMeatProgress(int id) => MeatProgressTable.Get(id);

    /// <summary>
    /// 获取单个能量条数据
    /// </summary>
    public EnergyProgress GetEnergyProgress(int id) => EnergyProgressTable.Get(id);

    /// <summary>
    /// 获取单个任务数据
    /// </summary>
    public Quest GetQuest(int id) => QuestTable.Get(id);

    /// <summary>
    /// 通过任务类型获取单个任务数据
    /// </summary>
    public Quest GetQuest(EQuestType type)
        => QuestTable.DataList.FirstOrDefault(q => q.QuestType == type);

    /// <summary>
    /// 获取单个技能数据
    /// </summary>
    public Skill GetSkill(int id) => SkillTable.Get(id);

    /// <summary>
    /// 通过技能类型获取单个技能数据
    /// </summary>
    public Skill GetSkill(ESkillType type)
        => SkillTable.DataList.FirstOrDefault(s => s.SkillType == type);

    /// <summary>
    /// 获取色块人技能数据
    /// </summary>
    public Skill3KP GetSkill3KP(int id) => Skill3KPTable.Get(id);

    /// <summary>
    /// 获取紫薇技能数据
    /// </summary>
    public SkillZiWei GetSkillZiWei(int id) => SkillZiWeiTable.Get(id);

    /// <summary>
    /// 获取大美丽技能数据
    /// </summary>
    public SkillDaMeiLi GetSkillDaMeiLi(int id) => SkillDaMeiLiTable.Get(id);

    /// <summary>
    /// 获取金状元技能数据
    /// </summary>
    public SkillJinZhuangYuan GetSkillJinZhuangYuan(int id)
        => SkillJinZhuangYuanTable.Get(id);

    /// <summary>
    /// 获取亚克东技能数据
    /// </summary>
    public SkillYaKeDong GetSkillYaKeDong(int id) => SkillYaKeDongTable.Get(id);

    /// <summary>
    /// 获取单个幸运仪式增益数据
    /// </summary>
    public LuckyBuff GetLuckyBuff(int id) => LuckyBuffTable.Get(id);

    /// <summary>
    /// 通过幸运仪式增益类型获取单个增益数据
    /// </summary>
    public LuckyBuff GetLuckyBuff(ELuckyBuffType type)
        => LuckyBuffTable.DataList.FirstOrDefault(b => b.LuckyBuffType == type);

    /// <summary>
    /// 通过幸运仪式增益类型和强度获取单个增益数据
    /// </summary>
    public LuckyBuff GetLuckyBuff(ELuckyBuffType type, ELuckyBuffStrengthType strength)
        => LuckyBuffTable.DataList.FirstOrDefault(b => b.LuckyBuffType == type && b.LuckyBuffStrengthType == strength);

    /// <summary>
    /// 获取单个幸运仪式礼包数据
    /// </summary>
    public LuckyGift GetLuckyGift(int id) => LuckyGiftTable.Get(id);

    /// <summary>
    /// 通过礼包类型获取单个礼包数据
    /// </summary>
    public LuckyGift GetLuckyGift(ELuckyGiftType type)
        => LuckyGiftTable.DataList.FirstOrDefault(g => g.LuckyGiftType == type);

    /// <summary>
    /// 获取单个道具数据
    /// </summary>
    public Prop GetProp(int id) => PropTable.Get(id);

    /// <summary>
    /// 通过道具类型获取单个道具数据
    /// </summary>
    public Prop GetProp(EPropType type)
        => PropTable.DataList.FirstOrDefault(p => p.PropType == type);

    /// <summary>
    /// 获取炮火轰炸道具数据
    /// </summary>
    public PropBombardment GetPropBombardment(int id) => PropBombardmentTable.Get(id);

    /// <summary>
    /// 获取指哪打哪道具数据
    /// </summary>
    public PropAimAssist GetPropAimAssist(int id) => PropAimAssistTable.Get(id);

    /// <summary>
    /// 获取智能诱捕陷阱道具数据
    /// </summary>
    public PropTrap GetPropTrap(int id) => PropTrapTable.Get(id);
    #endregion

    #region 全局相关特殊方法
    /// <summary>
    /// 获取隐藏地图倒计时时长
    /// </summary>
    public float GetHiddenMapCountdownTime()
        => GlobalTable.DataList[0].HiddenMapCountDownTime;

    /// <summary>
    /// 获取任务全局配置
    /// </summary>
    public QuestGlobal GetQuestGlobal()
        => GlobalTable.DataList[0].QuestGlobal;
    #endregion

    #region 子弹相关特殊方法
    /// <summary>
    /// 获取所有特殊子弹的数据
    /// </summary>
    public List<Bullet> GetAllSpecialBullets()
        => BulletTable.DataList.Where(b => b.BulletType != EBulletType.Normal).ToList();

    /// <summary>
    /// 随机获取一个特殊子弹的数据
    /// </summary>
    public Bullet GetRandomSpecialBullet()
    {
        var specialBullets = GetAllSpecialBullets();
        return specialBullets[Random.Range(0, specialBullets.Count)];
    }  
    #endregion

    #region 物种相关特殊方法
    /// <summary>
    /// 获取地图所有物种的数据
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <returns></returns>
    public Dictionary<ESpecieType, int[]> GetMapSpecies(int mapId)
        => GetMap(mapId).MapSpecies;

    /// <summary>
    /// 获取地图物种类型权重
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <returns></returns>
    public Dictionary<ESpecieType, float> GetMapSpecieTypeWeights(int mapId)
        => GetMap(mapId).SpecieTypeWeights;
    #endregion

    #region 地图相关特殊方法
    /// <summary>
    /// 随机获取一个主地图的数据
    /// </summary>
    public Map GetRandomMainMap()
    {
        var maps = MapTable.DataList.Where(m => m.MapType != EMapType.Hidden).ToList();
        return maps[Random.Range(0, maps.Count)];
    }
    #endregion

    #region 角色相关特殊方法
    /// <summary>
    /// 随机获取一个角色数据
    /// </summary>
    public Role GetRandomRole()
        => RoleTable.DataList[Random.Range(0, RoleTable.DataList.Count)];
    #endregion

    #region 肉度条相关特殊方法
    #endregion

    #region 能量条相关特殊方法
    #endregion

    #region 任务相关特殊方法
    /// <summary>
    /// 随机获取一个任务配置
    /// </summary>
    /// <returns>随机任务配置</returns>
    public Quest GetRandomQuest()
        => QuestTable.DataList[Random.Range(0, QuestTable.DataList.Count)];
    #endregion

    #region 技能相关特殊方法
    #endregion

    #region 幸运仪式相关特殊方法
    /// <summary>
    /// 根据礼包类型和权重随机抽取一个幸运仪式增益数据
    /// </summary>
    public LuckyBuff GetLuckyBuffByGiftAndWeights(ELuckyGiftType giftType)
    {
        var gift = GetLuckyGift(giftType);

        ELuckyBuffType buffType = GetBuffTypeByWeight(gift.BuffTypeWeights);
        ELuckyBuffStrengthType buffStrength = GetBuffStrengthByWeight(gift.BuffStrengthWeights);

        return GetLuckyBuff(buffType, buffStrength);
    }

    /// <summary>
    /// 根据权重随机抽取幸运仪式增益类型
    /// </summary>
    private ELuckyBuffType GetBuffTypeByWeight(Dictionary<ELuckyBuffType, int> typeWeights)
    {
        int totalWeight = typeWeights.Values.Sum();
        int randomPoint = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var kv in typeWeights)
        {
            cumulative += kv.Value;
            if (randomPoint < cumulative)
                return kv.Key;
        }

        return default;
    }

    /// <summary>
    /// 根据权重随机抽取幸运仪式增益强度
    /// </summary>
    private ELuckyBuffStrengthType GetBuffStrengthByWeight(Dictionary<ELuckyBuffStrengthType, int> strengthWeights)
    {
        int totalWeight = strengthWeights.Values.Sum();
        int randomPoint = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var kv in strengthWeights)
        {
            cumulative += kv.Value;
            if (randomPoint < cumulative)
                return kv.Key;
        }

        return default;
    }
    #endregion

    #region 道具相关特殊方法
    /// <summary>
    /// 随机获取一个道具配置
    /// </summary>
    public Prop GetRandomProp()
        => PropTable.DataList[Random.Range(0, PropTable.DataList.Count)];
    #endregion

    #region 测试
    /// <summary>
    /// 获取模拟排行榜数据
    /// </summary>
    /// <returns>排行榜面板数据</returns>
    public RankingPanelData GetMockRankingData()
    {
        var panelData = new RankingPanelData
        {
            DailyRankingList = new List<RankingItemData>(),
            WeeklyRankingList = new List<RankingItemData>()
        };

        string[] dailyPlayerNames = { "日榜冠军", "日榜亚军", "日榜季军", "日榜第四", "日榜第五", "日榜第六", "日榜第七", "日榜第八", "日榜第九", "日榜第十", "日榜十一", "日榜十二", "日榜十三", "日榜十四", "日榜十五", "日榜十六", "日榜十七", "日榜十八", "日榜十九", "日榜二十" };
        string[] dailyTimeFormats = { "00:58:23", "01:02:15", "01:05:47", "01:08:32", "01:12:09", "01:15:44", "01:18:26", "01:21:53", "01:24:17", "01:27:38", "01:30:52", "01:33:14", "01:36:28", "01:39:41", "01:42:55", "01:46:08", "01:49:22", "01:52:35", "01:55:49", "01:59:02" };

        string[] weeklyPlayerNames = { "周榜冠军", "周榜亚军", "周榜季军", "周榜第四", "周榜第五", "周榜第六", "周榜第七", "周榜第八", "周榜第九", "周榜第十", "周榜十一", "周榜十二", "周榜十三", "周榜十四", "周榜十五", "周榜十六", "周榜十七", "周榜十八", "周榜十九", "周榜二十", "周榜二一", "周榜二二", "周榜二三", "周榜二四", "周榜二五" };
        string[] weeklyTimeFormats = { "00:45:12", "00:48:33", "00:51:56", "00:55:19", "00:58:42", "01:02:05", "01:05:28", "01:08:51", "01:12:14", "01:15:37", "01:19:00", "01:22:23", "01:25:46", "01:29:09", "01:32:32", "01:35:55", "01:39:18", "01:42:41", "01:46:04", "01:49:27", "01:52:50", "01:56:13", "01:59:36", "02:02:59", "02:06:22" };

        for (int i = 0; i < dailyPlayerNames.Length; i++)
        {
            panelData.DailyRankingList.Add(new RankingItemData
            {
                Avatar = null,
                PlayerName = dailyPlayerNames[i],
                ClearTime = dailyTimeFormats[i]
            });
        }

        for (int i = 0; i < weeklyPlayerNames.Length; i++)
        {
            panelData.WeeklyRankingList.Add(new RankingItemData
            {
                Avatar = null,
                PlayerName = weeklyPlayerNames[i],
                ClearTime = weeklyTimeFormats[i]
            });
        }

        return panelData;
    }

    /// <summary>
    /// 随机获取一个跟随Boss的物种
    /// </summary>
    /// <returns></returns>
    public Specie GetRandomBossFollow()
    {
        var species = SpecieTable.DataList.Where(s => s.SpecieType == ESpecieType.Small).ToList();
        return species[Random.Range(0, species.Count)];
    }

    /// <summary>
    /// 随机获取一个物种
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <returns></returns>
    public Specie GetRandomSpecie(int mapId)
    {
        var map = GetMap(mapId);

        float totalWeight = map.SpecieTypeWeights.Values.Sum();
        float randomPoint = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var kv in map.SpecieTypeWeights)
        {
            cumulative += kv.Value;
            if (randomPoint <= cumulative)
            {
                var specieIds = map.MapSpecies[kv.Key];
                int specieId = specieIds[Random.Range(0, specieIds.Length)];
                return GetSpecie(specieId);
            }
        }

        return null;
    }

    /// <summary>
    /// 随机获取一个Boss物种的数据
    /// </summary>
    public Specie GetRandomBoss()
        => SpecieTable.DataList.Where(s => s.SpecieType == ESpecieType.Boss).ToList()[Random.Range(0, SpecieTable.DataList.Count(s => s.SpecieType == ESpecieType.Boss))];
    #endregion
}