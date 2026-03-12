using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;


/// <summary>
/// 能量条管理器
/// </summary>
public class EnergyProgressManager : IRoundManager, IRoundUpdatable
{
    /// <summary>
    /// 单条所需值
    /// </summary>
    private float _valuePerBar;
    public float ValuePerBar => _valuePerBar;

    /// <summary>
    /// 总条数
    /// </summary>
    private int _totalBar;
    public int TotalBar => _totalBar;

    /// <summary>
    /// 每秒增加量
    /// </summary>
    private float _increasePerSecond;
    public float IncreasePerSecond => _increasePerSecond;

    /// <summary>
    /// 当前能量值
    /// </summary>
    private float _currentEnergyValue;
    public float CurrentEnergyValue => _currentEnergyValue;

    /// <summary>
    /// 已完成的能量条数
    /// </summary>
    private int _completedBars;
    public int CompletedBars => _completedBars;

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        RegisterEvents();

        EnergyProgress energyProgress = _configManager.GetEnergyProgress(1);
        _valuePerBar = energyProgress.ValuePerBar;
        _totalBar = energyProgress.TotalBar;
        _increasePerSecond = energyProgress.IncreasePerSecond;
        
        Log.Info("[EnergyProgressManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        AddEnergyValue(deltaTime * _increasePerSecond);
    }

    public void Dispose()
    {
        UnregisterEvents();

        Log.Info("[EnergyProgressManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 增加能量
    /// </summary>
    public void AddEnergyValue(float amount)
    {
        if (_completedBars >= _totalBar)
            return;

        _currentEnergyValue += amount;

        bool barIncreased = false;
        while (_currentEnergyValue >= _valuePerBar && _completedBars < _totalBar)
        {
            _currentEnergyValue -= _valuePerBar;
            _completedBars++;
            barIncreased = true;
        }

        if (_completedBars >= _totalBar)
        {
            _completedBars = _totalBar;
            _currentEnergyValue = 0f;
        }

        TriggerProgressChanged(new EnergyProgressChangedEventArgs
        {
            Sender = this,
            CurrentEnergy = _currentEnergyValue,
            CurrentBars = _completedBars,
        });
        if (barIncreased)
        {
            TriggerBarCountChanged(new EnergyBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = _completedBars
            });
        }

        if (_completedBars >= _totalBar)
            TriggerMaxBarsReached();
    }

    /// <summary>
    /// 使用一条能量条
    /// </summary>
    public bool UseEnergyOneBar()
    {
        if (_completedBars <= 0)
            return false;

        _completedBars = _completedBars - 1;
        TriggerBarCountChanged(new EnergyBarCountChangedEventArgs
        {
            Sender = this,
            CurrentBars = _completedBars
        });
        TriggerEnergyConsumed(new EnergyConsumedEventArgs
        {
            Sender = this,
            RemainingBars = _completedBars
        });
        TriggerProgressChanged(new EnergyProgressChangedEventArgs
        {
            Sender = this,
            CurrentEnergy = _currentEnergyValue,
            CurrentBars = _completedBars
        });
        return true;
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
    private void OnDropRewardArrived(RewardArrivedEventArgs args)
    {
        if (args.DropType == EDropType.Energy)
            AddEnergyValue(args.DropCount);
    }

    /// <summary>
    /// 触发能量值变化事件
    /// </summary>
    private void TriggerProgressChanged(EnergyProgressChangedEventArgs args)
    {
        _eventManager.Trigger(EnergyEvents.EnergyProgressChanged, args);
    }

    /// <summary>
    /// 触发能量条数变化事件
    /// </summary>
    private void TriggerBarCountChanged(EnergyBarCountChangedEventArgs args)
    {
        _eventManager.Trigger(EnergyEvents.EnergyBarCountChanged, args);
    }

    /// <summary>
    /// 触发能量条达成上限事件
    /// </summary>
    private void TriggerMaxBarsReached()
    {
        _eventManager.Trigger(EnergyEvents.EnergyMaxBarsReached);
    }

    /// <summary>
    /// 触发能量条被消耗事件
    /// </summary>
    private void TriggerEnergyConsumed(EnergyConsumedEventArgs args)
    {
        _eventManager.Trigger(EnergyEvents.EnergyConsumed, args);
    }
    #endregion
}

