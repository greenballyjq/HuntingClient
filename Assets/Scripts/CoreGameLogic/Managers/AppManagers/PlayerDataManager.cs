using cfg.HuntingConfig.Enum;
using GameFramework.Game;
using GameFramework.Utility;
using System;
using System.Collections.Generic;

/// <summary>
/// 玩家数据管理器
/// </summary>
public class PlayerDataManager : IAppManager
{
    /// <summary>
    /// 三千盘金币数量
    /// </summary>
    private int _threeKPCoinAmount;

    /// <summary>
    /// 各道具数量
    /// </summary>
    private Dictionary<EPropType, int> _propCounts = new Dictionary<EPropType, int>();

    private EventManager _eventManager;

    public void Init()
    {
        RegisterServices();
        RegisterEvents();

        // TODO 未来从服务器获取玩家数据
        _threeKPCoinAmount = 50;
        foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
            _propCounts[propType] = 3;
        
        Log.Info("[PlayerDataManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();
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
        return _propCounts[propType];
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
    #endregion

    #region 私有方法
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
    }

    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        if (args.DropRewards.TryGetValue(EDropType.ThreeKPCoin, out int count) && count > 0)
            UpdateThreeKPCoinAmount(count);
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
    #endregion
}
