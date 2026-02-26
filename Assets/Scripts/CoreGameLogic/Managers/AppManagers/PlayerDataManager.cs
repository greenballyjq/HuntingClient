using cfg.HuntingConfig.Enum;
using GameFramework.Game;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    public void Init()
    {
        // TODO 未来从服务器获取玩家数据
        _threeKPCoinAmount = 50;

        foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
            _propCounts[propType] = 1;
        
        Debug.Log("[PlayerDataManager] 初始化完成");
    }

    public void Dispose()
    {
        Debug.Log("[PlayerDataManager] 已释放");
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
    public void UpdatePropCount(EPropType propType, int deltaAmount)
    {
        int oldAmount = GetPropCount(propType);
        _propCounts[propType] = oldAmount + deltaAmount;
        int currentAmount = _propCounts[propType];
        TriggerPropCountChanged(new PropCountChangedEventArgs
        {
            PropType = propType,
            CurrentAmount = currentAmount,
            DeltaAmount = deltaAmount
        });
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
