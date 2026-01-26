using cfg.HuntingConfig.Enum;
using UnityEngine;


/// <summary>
/// 结算奖励管理器
/// </summary>
public class SettlementRewardManager : IRoundManager
{
    /// <summary>
    /// 完成的肉条刻度
    /// </summary>
    private int _completedMeatScale;

    /// <summary>
    /// 肉条积分奖励
    /// </summary>
    private int _pointReward;

    /// <summary>
    /// 肉条奖励金币
    /// </summary>
    private int _coinFromMeat;

    /// <summary>
    /// 物种掉落金币
    /// </summary>
    private int _coinFromSpecie;

    /// <summary>
    /// 任务奖励金币
    /// </summary>
    private int _coinFromQuest;

    /// <summary>
    /// 额外结算倍率
    /// </summary>
    private float _extraMultiplier = 1f;

    /// <summary>
    /// 是否已经应用广告翻倍
    /// </summary>
    private bool _isDoubleApplied;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    public void Init(RoundContext context)
    {
        RegisterEvents();
        Debug.Log("[SettlementRewardManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();
        Debug.Log("[SettlementRewardManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 设置奖励倍率
    /// </summary>
    public void SetRewardMultiplier(float multiplier)
    {
        _extraMultiplier = multiplier;
    }

    /// <summary>
    /// 设置奖励翻倍
    /// </summary>
    public void SetRewardDouble()
    {
        _isDoubleApplied = true;
        TriggerSettlementCalculated(new SettlementCalculatedEventArgs
        {
            Sender = this,
            CompletedMeatBars = _completedMeatScale,
            BaseCoin = _coinFromMeat,
            BaseMastery = _pointReward,
            CoinFromSpecie = _coinFromSpecie,
            CoinFromQuest = _coinFromQuest,
            ExtraMultiplier = _extraMultiplier,
            IsDoubleApplied = _isDoubleApplied,
            TotalCoin = GetTotalCoin(),
            TotalMastery = GetTotalMastery()
        });
    }

    /// <summary>
    /// 计算奖励
    /// </summary>
    public void CalculateReward()
    {
        // 触发结算开始事件
        TriggerSettlementStarted();

        // 计算肉度条基础奖励
        CalculateBaseReward();
        
        // 触发结算计算事件
        TriggerSettlementCalculated(new SettlementCalculatedEventArgs
        {
            Sender = this,
            CompletedMeatBars = _completedMeatScale,
            BaseCoin = _coinFromMeat,
            BaseMastery = _pointReward,
            CoinFromSpecie = _coinFromSpecie,
            CoinFromQuest = _coinFromQuest,
            ExtraMultiplier = _extraMultiplier,
            IsDoubleApplied = _isDoubleApplied,
            TotalCoin = GetTotalCoin(),
            TotalMastery = GetTotalMastery()
        });
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 计算肉度条基础奖励
    /// </summary>
    private void CalculateBaseReward()
    {
        var reward = _configManager.GetMeatProgressReward(_completedMeatScale);
        if (reward == null)
        {
            _coinFromMeat = 0;
            _pointReward = 0;
            return;
        }
        
        _coinFromMeat = reward.ThreeKPCoin;
        _pointReward = reward.Point;
    }

    /// <summary>
    /// 计算包含倍率与翻倍后的总金币
    /// </summary>
    private int GetTotalCoin()
    {
        int total = _coinFromMeat + _coinFromSpecie + _coinFromQuest;
        total = Mathf.RoundToInt(total * _extraMultiplier);
        if (_isDoubleApplied)
            total *= 2;

        return total;
    }

    /// <summary>
    /// 计算包含翻倍后的熟练度
    /// </summary>
    private int GetTotalMastery()
    {
        int total = _pointReward;
        total = Mathf.RoundToInt(total * _extraMultiplier);
        if (_isDoubleApplied)
            total *= 2;

        return total;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        _eventManager.AddListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
        _eventManager.AddListener(QuestEvents.QuestCompleted, OnQuestCompleted);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        _eventManager.RemoveListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
        _eventManager.RemoveListener(QuestEvents.QuestCompleted, OnQuestCompleted);
    }

    /// <summary>
    /// 掉落奖励回调
    /// </summary>
    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        _coinFromSpecie += args.DropRewards[EDropType.ThreeKPCoin];
    }

    /// <summary>
    /// 肉度条数量变化回调
    /// </summary>
    private void OnMeatScaleCompleted(MeatScaleCompletedEventArgs args)
    {
        _completedMeatScale = Mathf.Max(0, args.CompletedScaleCount);
    }

    /// <summary>
    /// 任务完成回调
    /// </summary>
    private void OnQuestCompleted(QuestCompletedEventArgs args)
    {
        _coinFromQuest += args.RewardCoin;
    }

    /// <summary>
    /// 触发结算开始事件
    /// </summary>
    private void TriggerSettlementStarted()
    {
        _eventManager.Trigger(SettlementEvents.SettlementStarted);
    }

    /// <summary>
    /// 触发结算计算事件
    /// </summary>
    private void TriggerSettlementCalculated(SettlementCalculatedEventArgs args)
    {
        _eventManager.Trigger(SettlementEvents.SettlementCalculated, args);
    }
    #endregion
}

