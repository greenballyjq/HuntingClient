using System;
using System.Collections.Generic;
using System.Linq;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using cfg.HuntingConfig.Skill;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 测试用配置管理器 - 单例、挂载式、不依赖游戏启动器
/// 可以直接挂载到场景中使用，会自动初始化
/// </summary>
public class TestConfigManager : MonoBehaviour
{
    private static TestConfigManager _instance;

    /// <summary>
    /// 单例实例
    /// </summary>
    public static TestConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试从场景中查找
                _instance = FindObjectOfType<TestConfigManager>();

                // 如果场景中没有，则自动创建
                if (_instance == null)
                {
                    GameObject go = new GameObject("TestConfigManager");
                    _instance = go.AddComponent<TestConfigManager>();
                    Debug.Log("[TestConfigManager] 自动创建 TestConfigManager 实例");
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 数值表名称列表
    /// </summary>
    private readonly List<string> _tableNames = new List<string>
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
        "huntingconfig_prop_tbprop",
        "huntingconfig_prop_tbpropbombardment",
        "huntingconfig_prop_tbpropaimassist",
        "huntingconfig_prop_tbproptrap",
    };

    /// <summary>
    /// 配置表容器
    /// </summary>
    private cfg.Tables _tables;

    /// <summary>
    /// 初始化状态
    /// </summary>
    public bool Initialized { get; private set; }

    private void Awake()
    {
        // 单例模式处理
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        // 自动初始化
        InitializeAsync();
    }

    /// <summary>
    /// 异步初始化配置表
    /// </summary>
    private async void InitializeAsync()
    {
        try
        {
            await LoadTablesAsync();
            Initialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TestConfigManager] 配置表加载失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载配置表
    /// </summary>
    private async System.Threading.Tasks.Task LoadTablesAsync()
    {
        List<Luban.ByteBuf> byteBuffs = new List<Luban.ByteBuf>();

        foreach (var tableName in _tableNames)
        {
            // 从 Resources 加载配置
            TextAsset textAsset = Resources.Load<TextAsset>($"Bundle/Raw/Configs/bytes/{tableName}");

            if (textAsset == null)
            {
                Debug.LogError($"[TestConfigManager] 配置加载失败：未找到资源 Bundle/Raw/Configs/bytes/{tableName}");
                continue;
            }

            byteBuffs.Add(new Luban.ByteBuf(textAsset.bytes));
        }

        int index = 0;
        _tables = new cfg.Tables(_ => byteBuffs[index++]);

        await Cysharp.Threading.Tasks.UniTask.Yield();
    }

    /// <summary>
    /// 等待配置管理器初始化完成
    /// </summary>
    public async Cysharp.Threading.Tasks.UniTask WaitForInitializationAsync()
    {
        while (!Initialized)
        {
            await Cysharp.Threading.Tasks.UniTask.Delay(10);
        }
    }

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
    /// 肉度条数值表
    /// </summary>
    public TbMeatProgress MeatProgressTable => _tables?.TbMeatProgress;

    /// <summary>
    /// 丰收能量条数值表
    /// </summary>
    public TbEnergyProgress EnergyProgressTable => _tables?.TbEnergyProgress;

    /// <summary>
    /// 任务数值表
    /// </summary>
    public TbQuest QuestTable => _tables?.TbQuest;

    /// <summary>
    /// 技能主表
    /// </summary>
    public TbSkill SkillTable => _tables?.TbSkill;

    /// <summary>
    /// 色块人技能数值表
    /// </summary>
    public TbSkill3KP Skill3KPTable => _tables?.TbSkill3KP;

    /// <summary>
    /// 紫薇技能数值表
    /// </summary>
    public TbSkillZiWei SkillZiWeiTable => _tables?.TbSkillZiWei;

    /// <summary>
    /// 大美丽技能数值表
    /// </summary>
    public TbSkillDaMeiLi SkillDaMeiLiTable => _tables?.TbSkillDaMeiLi;

    /// <summary>
    /// 金状元技能数值表
    /// </summary>
    public TbSkillJinZhuangYuan SkillJinZhuangYuanTable => _tables?.TbSkillJinZhuangYuan;

    /// <summary>
    /// 亚克东技能数值表
    /// </summary>
    public TbSkillYaKeDong SkillYaKeDongTable => _tables?.TbSkillYaKeDong;

    /// <summary>
    /// 幸运仪式数值表
    /// </summary>
    public TbLuckyBuff LuckyTable => _tables?.TbLuckyBuff;

    /// <summary>
    /// 道具总表
    /// </summary>
    public TbProp PropTable => _tables?.TbProp;

    /// <summary>
    /// 炮火轰炸道具参数数值表
    /// </summary>
    public TbPropBombardment PropBombardmentTable => _tables?.TbPropBombardment;

    /// <summary>
    /// 指哪打哪道具参数数值表
    /// </summary>
    public TbPropAimAssist PropAimAssistTable => _tables?.TbPropAimAssist;

    /// <summary>
    /// 智能诱捕陷阱道具参数数值表
    /// </summary>
    public TbPropTrap PropTrapTable => _tables?.TbPropTrap;
    #endregion

    #region 数值表单个数据项访问
    /// <summary>
    /// 获取单个子弹数据
    /// </summary>
    public Bullet GetBullet(int id) => BulletTable?.Get(id);

    /// <summary>
    /// 通过子弹类型获取单个子弹数据
    /// </summary>
    public Bullet GetBullet(EBulletType type)
        => BulletTable?.DataList.FirstOrDefault(b => b.BulletType == type);

    /// <summary>
    /// 获取单个物种数据
    /// </summary>
    public Specie GetSpecie(int id) => SpecieTable?.Get(id);

    /// <summary>
    /// 获取单个派发数据
    /// </summary>
    public Spawn GetSpawn(int id) => SpawnTable?.Get(id);

    /// <summary>
    /// 获取单个地图数据
    /// </summary>
    public Map GetMap(int id) => MapTable?.Get(id);

    /// <summary>
    /// 通过地图类型获取单个地图数据
    /// </summary>
    public Map GetMap(EMapType type)
        => MapTable?.DataList.FirstOrDefault(p => p.MapType == type);

    /// <summary>
    /// 获取单个角色数据
    /// </summary>
    public Role GetRole(int id) => RoleTable?.Get(id);

    /// <summary>
    /// 通过角色类型获取单个角色数据
    /// </summary>
    public Role GetRole(ERoleType type)
        => RoleTable?.DataList.FirstOrDefault(p => p.RoleType == type);

    /// <summary>
    /// 获取单个肉度条数据
    /// </summary>
    public MeatProgress GetMeatProgress(int id) => MeatProgressTable?.Get(id);

    /// <summary>
    /// 获取单个丰收能量条数据
    /// </summary>
    public EnergyProgress GetEnergyProgress(int id) => EnergyProgressTable?.Get(id);

    /// <summary>
    /// 获取单个任务数据
    /// </summary>
    public Quest GetDynamicQuest(int id) => QuestTable?.Get(id);

    /// <summary>
    /// 获取单个幸运仪式配置
    /// </summary>
    public LuckyBuff GetLucky(int id) => LuckyTable?.Get(id);

    /// <summary>
    /// 通过幸运仪式类型获取单个配置
    /// </summary>
    public LuckyBuff GetLucky(ELuckyBuffType type)
        => LuckyTable?.DataList.FirstOrDefault(p => p.LuckyBuffType == type);

    /// <summary>
    /// 获取单个道具配置
    /// </summary>
    public Prop GetProp(int id) => PropTable?.Get(id);

    /// <summary>
    /// 通过道具类型获取单个配置
    /// </summary>
    public Prop GetProp(EPropType type)
        => PropTable?.DataList.FirstOrDefault(p => p.PropType == type);

    /// <summary>
    /// 获取单个技能配置
    /// </summary>
    public Skill GetSkill(int id) => SkillTable?.Get(id);

    /// <summary>
    /// 通过技能类型获取单个技能配置
    /// </summary>
    public Skill GetSkill(ESkillType type)
        => SkillTable?.DataList.FirstOrDefault(p => p.SkillType == type);

    /// <summary>
    /// 通过任务类型获取单个任务数据
    /// </summary>
    public Quest GetDynamicQuest(EQuestType type)
        => QuestTable?.DataList.FirstOrDefault(q => q.QuestType == type);
    #endregion

    #region 角色相关特殊方法
    /// <summary>
    /// 获取默认角色（小蓝人）
    /// </summary>
    public Role GetDefaultRole()
        => GetRole(ERoleType.Bule);

    /// <summary>
    /// 随机获取一个角色
    /// </summary>
    public Role GetRandomRole()
        => RoleTable?.DataList[UnityEngine.Random.Range(0, RoleTable.DataList.Count)];
    #endregion

    #region 地图相关特殊方法
    /// <summary>
    /// 获取地图使用的体型策略（体型比例与驻场时间）
    /// </summary>
    public Spawn GetMapSpawnStrategy(int mapId)
        => GetSpawn(GetMap(mapId)?.SpawnStrategyId ?? 0);

    /// <summary>
    /// 随机获取一个地图
    /// </summary>
    public Map GetRandomMap()
        => MapTable?.DataList[UnityEngine.Random.Range(0, MapTable.DataList.Count)];
    #endregion

    #region 子弹相关特殊方法
    /// <summary>
    /// 获取所有特殊子弹的数据
    /// </summary>
    public List<Bullet> GetAllSpecialBullets()
        => BulletTable?.DataList.Where(b => b.BulletType != EBulletType.Normal).ToList();

    /// <summary>
    /// 随机获取一个特殊子弹的数据
    /// </summary>
    public Bullet GetRandomSpecialBullet()
    {
        var specialBullets = GetAllSpecialBullets();
        if (specialBullets == null || specialBullets.Count == 0)
            return null;
        return specialBullets[UnityEngine.Random.Range(0, specialBullets.Count)];
    }
    #endregion

    #region 物种相关特殊方法
    /// <summary>
    /// 根据地图与体型，在对应物种池内按权重随机选择一个物种
    /// </summary>
    public Specie GetRandomSpecieByMapAndVolume(int mapId, EVolumeType volumeType)
    {
        var map = GetMap(mapId);
        if (map == null)
            return null;

        // 取得当前地图该体型对应的物种权重列表
        if (!map.SpeciesByVolume.TryGetValue(volumeType, out var specieWeights))
            return null;

        // 累加权重用于后续随机
        float totalWeight = 0f;
        for (int i = 0; i < specieWeights.Length; i++)
        {
            float w = specieWeights[i].Weight;
            if (w > 0f) totalWeight += w;
        }

        if (totalWeight <= 0f)
            return null;

        // 在总权重范围内取随机点
        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);
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
    public EVolumeType GetRandomVolumeTypeByStrategy(int mapId)
    {
        // 获取地图配置的体型策略
        var strategy = GetMapSpawnStrategy(mapId);
        if (strategy == null)
            return default;

        // 计算所有体型比例总和
        float totalRatio = strategy.VolumeRatio.Values.Sum();

        if (totalRatio <= 0f)
            return default;

        // 取随机点并根据权重命中体型
        float randomPoint = UnityEngine.Random.Range(0f, totalRatio);
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
    public float GetStayTimeByVolumeType(int mapId, EVolumeType volumeType)
    {
        var strategy = GetMapSpawnStrategy(mapId);
        if (strategy == null || !strategy.StayTime.TryGetValue(volumeType, out int stayTime))
            return 0f;
        return stayTime;
    }
    #endregion

    #region 任务相关特殊方法
    /// <summary>
    /// 随机获取一个任务配置
    /// </summary>
    public Quest GetRandomQuest()
        => QuestTable?.DataList[UnityEngine.Random.Range(0, QuestTable.DataList.Count)];

    /// <summary>
    /// 从任务配置中随机获取目标值
    /// </summary>
    public int GetRandomTargetValue(Quest quest)
    {
        if (quest == null || quest.TargetRange == null || quest.TargetRange.Length == 0)
            return 0;
        if (quest.TargetRange.Length == 1)
            return quest.TargetRange[0];
        return UnityEngine.Random.Range(quest.TargetRange[0], quest.TargetRange[1] + 1);
    }

    /// <summary>
    /// 从任务配置中随机获取奖励值
    /// </summary>
    public int GetRandomRewardValue(Quest quest)
    {
        if (quest == null || quest.RewardRange == null || quest.RewardRange.Length == 0)
            return 0;
        if (quest.RewardRange.Length == 1)
            return quest.RewardRange[0];
        return UnityEngine.Random.Range(quest.RewardRange[0], quest.RewardRange[1] + 1);
    }
    #endregion
}