using System.Collections.Generic;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// 肉度条管理器
/// </summary>
public class MeatProgressManager : IRoundManager, IRoundResettable
{
    /// <summary>
    /// 单刻度所需值
    /// </summary>
    private int _valuePerScale;
    public int ValuePerScale => _valuePerScale;

    /// <summary>
    /// 总刻度数
    /// </summary>
    private int _totalScale;
    public int TotalScale => _totalScale;

    /// <summary>
    /// 满条所需肉度值
    /// </summary>
    private int _totalMeatValue;
    public int TotalMeatValue => _totalMeatValue;

    /// <summary>
    /// 当前地图肉度值
    /// </summary>
    private int _currentMeatValue;
    public int CurrentMeatValue => _currentMeatValue;

    /// <summary>
    /// 已完成的刻度数
    /// </summary>
    private int _completedScaleCount;
    public int CompletedScaleCount => _completedScaleCount;

    /// <summary>
    /// 所有地图肉量累加
    /// </summary>
    private int _totalMeatAcrossMaps;
    public int TotalMeatAcrossMaps => _totalMeatAcrossMaps;

    /// <summary>
    /// 各地图肉量记录
    /// </summary>
    private readonly List<(int mapId, int meatAmount)> _meatPerMap = new List<(int mapId, int meatAmount)>();

    /// <summary>
    /// 当前地图ID
    /// </summary>
    private int _currentMapId;

    /// <summary>
    /// 总进度比例
    /// </summary>
    public float TotalProgressRatio => Mathf.Clamp01((float)_currentMeatValue / _totalMeatValue);

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        RegisterEvents();

        _currentMapId = context.MapData.ID;

        var meatProgress = _configManager.GetMeatProgress(1);
        _valuePerScale = meatProgress.ValuePerScale;
        _totalScale = meatProgress.TotalScale;
        _totalMeatValue = _valuePerScale * _totalScale;
        _completedScaleCount = 0;

        Log.Info("[MeatProgressManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();

        Log.Info("[MeatProgressManager] 已释放");
    }

    public void Cleanup()
    {
        _meatPerMap.Add((_currentMapId, _currentMeatValue));
        _totalMeatAcrossMaps += _currentMeatValue;

        _currentMeatValue = 0;
        _completedScaleCount = 0;
    }

    public void ReInit(Map mapData)
    {
        _currentMapId = mapData.ID;
    }

    #region 公共方法
    /// <summary>
    /// 增加肉度值
    /// </summary>
    /// <param name="amount">增加的肉度值</param>
    public void AddMeatAmount(int amount)
    {
        int beforeScaleCount = _completedScaleCount;

        _currentMeatValue += amount;

        int clampedForProgress = Mathf.Min(_currentMeatValue, _totalMeatValue);
        _completedScaleCount = Mathf.Min(_totalScale, Mathf.FloorToInt(clampedForProgress / _valuePerScale));

        TriggerMeatValueChanged(new MeatValueChangedEventArgs
        {
            CurrentMeatValue = _currentMeatValue,
            TotalProgressRatio = TotalProgressRatio,
            CompletedScaleCount = _completedScaleCount
        });

        if (_completedScaleCount > beforeScaleCount)
        {
            TriggerMeatScaleCompleted(new MeatScaleCompletedEventArgs
            {
                TotalScaleCount = _totalScale,
                CompletedScaleCount = _completedScaleCount
            });

            if (_completedScaleCount == _totalScale)
            {
                TriggerMeatScaleFull();
            }
        }
    }

    /// <summary>
    /// 肉量换算为三千盘金币数量
    /// </summary>
    public int GetThreeKPCoinCountFromMeat(int meatValue)
    {
        int rate = _configManager.GetMeatToThreeKPCoinRate();
        return meatValue / rate;
    }

    /// <summary>
    /// 获取各地图肉量统计
    /// </summary>
    public void GetMeatPerMapStatistics(List<(int mapId, int meatAmount)> results)
    {
        results.Clear();
        results.AddRange(_meatPerMap);
        results.Add((_currentMapId, _currentMeatValue));
    }
    #endregion

    #region 私有方法
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }
    
    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    /// <summary>
    /// 掉落奖励生效事件回调
    /// </summary>
    private void OnDropRewardArrived(DropRewardArrivedEventArgs args)
    {
        if (args.DropType == EDropType.Meat)
            AddMeatAmount(args.DropCount);
    }
    
    /// <summary>
    /// 触发肉度值变化事件
    /// </summary>
    private void TriggerMeatValueChanged(MeatValueChangedEventArgs args)
    {
        _eventManager.Trigger(MeatEvents.MeatValueChanged, args);
    }

    /// <summary>
    /// 触发肉度刻度完成事件
    /// </summary>
    private void TriggerMeatScaleCompleted(MeatScaleCompletedEventArgs args)
    {
        _eventManager.Trigger(MeatEvents.MeatScaleCompleted, args);
    }

    /// <summary>
    /// 触发肉度刻度满事件
    /// </summary>
    private void TriggerMeatScaleFull() 
    {
        _eventManager.Trigger(MeatEvents.MeatScaleFull);
    }
    #endregion
}