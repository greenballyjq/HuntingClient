using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;
using GameFramework.UI;
using GameFramework.Utility;
using Luban;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 打猎配置管理器
/// </summary>
public class HuntingConfigManager : IAppManager
{
    private const string ConfigBytesPath = "Bundle/Raw/Configs/bytes";
    private const string SOPath = "Assets/Arts/SO/Refs";

    private static readonly List<string> TableNames = new List<string>
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

    /// <summary>
    /// 角色表现资源配置
    /// </summary>
    private RoleRefSo _roleRefSo;
    public RoleRefSo RoleRefSo => _roleRefSo;

    /// <summary>
    /// UI / 流程音效资源配置
    /// </summary>
    private UiAudioRefSo _uiAudioRefSo;
    public UiAudioRefSo UiAudioRefSo => _uiAudioRefSo;

    private ResourceManager _resourceManager;
    private cfg.Tables _tables;

    public async UniTask InitAsync()
    {
        _resourceManager = GameServiceLocator.ResourceManager;
        try
        {
            Log.Info("[HuntingConfigManager] 开始加载配置表");

            await LoadTablesAsync();
            await LoadScriptableObjects();

            Log.Info("[HuntingConfigManager] 配置表加载完成");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[HuntingConfigManager] 配置加载失败: {ex.Message}");
        }
    }
    public void Dispose()
    {
        _tables = null;
    }

    private async UniTask LoadTablesAsync()
    {
        var byteBuffs = new List<ByteBuf>();
        foreach (var tableName in TableNames)
        {
            var textAsset = Resources.Load<TextAsset>($"{ConfigBytesPath}/{tableName}");
            if (textAsset == null)
            {
                Log.Error($"[HuntingConfigManager] 配置加载失败：未找到资源 {ConfigBytesPath}/{tableName}");
                continue;
            }

            byteBuffs.Add(new ByteBuf(textAsset.bytes));
        }

        int index = 0;
        _tables = new cfg.Tables(_ => byteBuffs[index++]);
        await UniTask.Yield();
    }
    private async UniTask LoadScriptableObjects()
    {
        _dropRewardRefSo = await _resourceManager.LoadAssetAsync<DropRewardRefSo>($"{SOPath}/DropRewardRefSo");
        _bulletRefSo = await _resourceManager.LoadAssetAsync<BulletRefSo>($"{SOPath}/BulletRefSo");
        _propRefSo = await _resourceManager.LoadAssetAsync<PropRefSo>($"{SOPath}/PropRefSo");
        _skillRefSo = await _resourceManager.LoadAssetAsync<SkillRefSo>($"{SOPath}/SkillRefSo");
        _animalRefSo = await _resourceManager.LoadAssetAsync<AnimalRefSo>($"{SOPath}/AnimalRefSo");
        _mapRefSo = await _resourceManager.LoadAssetAsync<MapRefSo>($"{SOPath}/MapRefSo");
        _roleRefSo = await _resourceManager.LoadAssetAsync<RoleRefSo>($"{SOPath}/RoleRefSo");
        _uiAudioRefSo = await _resourceManager.LoadAssetAsync<UiAudioRefSo>($"{SOPath}/UiAudioRefSo");
        UIButton.SetDefaults(_uiAudioRefSo);
    }

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
    /// 获取单个肉条数据（全局配置表仅一行）
    /// </summary>
    public MeatProgress GetMeatProgress() => MeatProgressTable.DataList[0];

    /// <summary>
    /// 获取单个肉条数据
    /// </summary>
    public MeatProgress GetMeatProgress(int id) => MeatProgressTable.Get(id);

    /// <summary>
    /// 获取能量条数据（全局配置表仅一行）
    /// </summary>
    public EnergyProgress GetEnergyProgress() => EnergyProgressTable.DataList[0];

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
    public const int GlobalMainConfigId = 1;

    /// <summary>
    /// 获取全局配置行
    /// </summary>
    public Global GetGlobal(int id) => GlobalTable.Get(id);

    /// <summary>
    /// 肉兑换三千盘金币汇率
    /// </summary>
    public int GetMeatToThreeKPCoinRate() 
        => GetGlobal(GlobalMainConfigId).MeatToThreeKPCoinRate;

    /// <summary>
    /// 普通结算固定积分量
    /// </summary>
    public int GetSettlementPointRewardAmount() 
        => GetGlobal(GlobalMainConfigId).PointRewardAmount;

    /// <summary>
    /// 获取道具价格
    /// </summary>
    public int GetPropPrice()
        => GetGlobal(GlobalMainConfigId).PropPrice;

    /// <summary>
    /// 获取隐藏地图倒计时时长
    /// </summary>
    public float GetHiddenMapCountdownTime()
        => GetGlobal(GlobalMainConfigId).HiddenMapCountDownTime;

    /// <summary>
    /// 获取隐藏地图概率
    /// </summary>
    public float GetHiddenMapProbability()
        => GetGlobal(GlobalMainConfigId).HiddenMapProbability;  

    /// <summary>
    /// 获取任务全局配置
    /// </summary>
    public QuestGlobal GetQuestGlobal()
        => GetGlobal(GlobalMainConfigId).QuestGlobal;

    
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
    /// 随机获取一个Boss物种的数据
    /// </summary>
    public Specie GetRandomBoss()
        => SpecieTable.DataList.Where(s => s.SpecieType == ESpecieType.Boss).ToList()[Random.Range(0, SpecieTable.DataList.Count(s => s.SpecieType == ESpecieType.Boss))];
    #endregion
}