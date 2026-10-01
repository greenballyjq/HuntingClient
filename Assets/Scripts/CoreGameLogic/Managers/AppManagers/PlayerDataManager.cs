using cfg.HuntingConfig.Enum;
using GameFramework.Game;
using GameFramework.Utility;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// 玩家数据管理器
/// </summary>
public class PlayerDataManager : IAppManager
{
    private const int DefaultThreeKPCoinAmount = 50;
    private const int DefaultPropCount = 3;

    /// <summary>
    /// 三千盘金币数量
    /// </summary>
    private int _threeKPCoinAmount;

    /// <summary>
    /// 积分数量
    /// </summary>
    private int _pointAmount;

    /// <summary>
    /// 完成局数
    /// </summary>
    private int _completedRoundCount;

    /// <summary>
    /// 击败 Boss 总数
    /// </summary>
    private int _totalBossKillCount;

    /// <summary>
    /// 金币人所在格子索引，-1 为场外
    /// </summary>
    private int _coinManSlotIndex = -1;

    /// <summary>
    /// 各道具数量
    /// </summary>
    private readonly Dictionary<EPropType, int> _propCounts = new Dictionary<EPropType, int>();

    /// <summary>
    /// 各 Boss 击杀数
    /// </summary>
    private readonly Dictionary<EBossType, int> _bossKills = new Dictionary<EBossType, int>();

    private EventManager _eventManager;
    private SaveService _saveService;

    /// <summary>
    /// 金币人所在格子索引，-1 为场外
    /// </summary>
    public int CoinManSlotIndex => _coinManSlotIndex;

    public UniTask InitAsync()
    {
        BindServices();
        SubscribeEvents();

        if (!_saveService.TryRead(out PlayerSaveData data) || data == null)
            ApplyDefaults();
        else
            ApplySaveData(data);

        Log.Info("[PlayerDataManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        UnsubscribeEvents();
        Log.Info("[PlayerDataManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 获取三千盘金币数量
    /// </summary>
    public int GetThreeKPCoinAmount()
    {
        return _threeKPCoinAmount;
    }

    /// <summary>
    /// 更新三千盘金币数量
    /// </summary>
    public void UpdateThreeKPCoinAmount(int amount)
    {
        int oldAmount = _threeKPCoinAmount;
        _threeKPCoinAmount += amount;
        int deltaAmount = _threeKPCoinAmount - oldAmount;
        TriggerThreeKPCoinAmountChanged(new ThreeKPCoinAmountChangedEventArgs
        {
            CurrentAmount = _threeKPCoinAmount,
            DeltaAmount = deltaAmount
        });
    }

    /// <summary>
    /// 获取道具数量
    /// </summary>
    public int GetPropCount(EPropType propType)
    {
        return _propCounts.TryGetValue(propType, out int count) ? count : 0;
    }

    /// <summary>
    /// 更新道具数量
    /// </summary>
    public void UpdatePropCount(EPropType propType, int amount)
    {
        int oldAmount = GetPropCount(propType);
        _propCounts[propType] = oldAmount + amount;
        int currentAmount = _propCounts[propType];
        TriggerPropCountChanged(new PropCountChangedEventArgs
        {
            PropType = propType,
            CurrentAmount = currentAmount,
            DeltaAmount = amount
        });
    }

    /// <summary>
    /// 金币是否足够
    /// </summary>
    public bool EnoughThreeKp(int priceCoin)
    {
        return _threeKPCoinAmount >= priceCoin;
    }

    /// <summary>
    /// 完成局数 +1
    /// </summary>
    public void AddCompletedRound()
    {
        _completedRoundCount++;
    }

    /// <summary>
    /// 设置金币人所在格子
    /// </summary>
    public void SetCoinManSlotIndex(int slotIndex)
    {
        _coinManSlotIndex = slotIndex;
    }

    /// <summary>
    /// 写入存档
    /// </summary>
    public void Save()
    {
        _saveService.Write(ToSaveData());
    }
    #endregion

    #region 私有方法
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _saveService = GameServiceLocator.GetAppManager<SaveService>();
    }

    private void SubscribeEvents()
    {
        _eventManager.AddListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
        _eventManager.AddListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    private void UnsubscribeEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
        _eventManager.RemoveListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    private void ApplyDefaults()
    {
        _threeKPCoinAmount = DefaultThreeKPCoinAmount;
        _pointAmount = 0;
        _completedRoundCount = 0;
        _totalBossKillCount = 0;
        _coinManSlotIndex = -1;
        _propCounts.Clear();
        _bossKills.Clear();

        foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
            _propCounts[propType] = DefaultPropCount;
    }

    private void ApplySaveData(PlayerSaveData data)
    {
        _threeKPCoinAmount = data.ThreeKPCoinAmount;
        _pointAmount = data.PointAmount;
        _completedRoundCount = data.CompletedRoundCount;
        _totalBossKillCount = data.TotalBossKillCount;
        _coinManSlotIndex = data.CoinManSlotIndex;

        _propCounts.Clear();
        foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
            _propCounts[propType] = 0;
        if (data.PropCounts != null)
        {
            for (int i = 0; i < data.PropCounts.Length; i++)
            {
                PropCountEntry entry = data.PropCounts[i];
                _propCounts[entry.PropType] = entry.Count;
            }
        }

        _bossKills.Clear();
        if (data.BossKills != null)
        {
            for (int i = 0; i < data.BossKills.Length; i++)
            {
                BossKillEntry entry = data.BossKills[i];
                _bossKills[entry.BossType] = entry.Count;
            }
        }
    }

    private PlayerSaveData ToSaveData()
    {
        var propCounts = new PropCountEntry[_propCounts.Count];
        int propIndex = 0;
        foreach (var pair in _propCounts)
        {
            propCounts[propIndex++] = new PropCountEntry
            {
                PropType = pair.Key,
                Count = pair.Value
            };
        }

        var bossKills = new BossKillEntry[_bossKills.Count];
        int bossIndex = 0;
        foreach (var pair in _bossKills)
        {
            bossKills[bossIndex++] = new BossKillEntry
            {
                BossType = pair.Key,
                Count = pair.Value
            };
        }

        return new PlayerSaveData
        {
            Version = 2,
            ThreeKPCoinAmount = _threeKPCoinAmount,
            PointAmount = _pointAmount,
            CompletedRoundCount = _completedRoundCount,
            TotalBossKillCount = _totalBossKillCount,
            PropCounts = propCounts,
            BossKills = bossKills,
            CoinManSlotIndex = _coinManSlotIndex
        };
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 触发三千盘金币数量改变事件
    /// </summary>
    private void TriggerThreeKPCoinAmountChanged(ThreeKPCoinAmountChangedEventArgs args)
    {
        _eventManager.Trigger(PlayerDataEvents.ThreeKPCoinAmountChanged, args);
    }

    /// <summary>
    /// 触发道具数量改变事件
    /// </summary>
    private void TriggerPropCountChanged(PropCountChangedEventArgs args)
    {
        _eventManager.Trigger(PlayerDataEvents.PropCountChanged, args);
    }

    /// <summary>
    /// 动物掉落奖励事件回调
    /// </summary>
    private void OnDropRewardArrived(DropRewardArrivedEventArgs args)
    {
        if (args.DropType == EDropType.ThreeKPCoin)
            UpdateThreeKPCoinAmount(args.DropCount);
    }

    /// <summary>
    /// 任务完成事件回调
    /// </summary>
    private void OnQuestCompleted(QuestCompletedEventArgs args)
    {
        UpdateThreeKPCoinAmount(args.RewardValue);
    }

    /// <summary>
    /// Boss 进死亡时按类型累计击杀
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (args.SpecieData == null)
            return;

        EBossType bossType = args.SpecieData.BossType;
        if (bossType == EBossType.None)
            return;

        _bossKills.TryGetValue(bossType, out int count);
        _bossKills[bossType] = count + 1;
        _totalBossKillCount++;
    }
    #endregion
}
