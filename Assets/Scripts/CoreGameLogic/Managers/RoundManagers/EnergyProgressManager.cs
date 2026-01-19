using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using UnityEngine;


/// <summary>
/// 丰收能量条管理器
/// </summary>
public class EnergyProgressManager : IRoundManager, IRoundUpdatable
{
    /// <summary>
    /// 单条所需值
    /// </summary>
    private float _valuePerBar;

    /// <summary>
    /// 总条数
    /// </summary>
    private int _totalBar;

    /// <summary>
    /// 每秒增加量
    /// </summary>
    private float _increasePerSecond;

    /// <summary>
    /// 当前能量值
    /// </summary>
    private float _currentEnergyValue;

    /// <summary>
    /// 已完成的能量条数
    /// </summary>
    private int _completedBars;

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
        EnergyProgress energyProgress = _configManager.GetEnergyProgress(1);
        _valuePerBar = energyProgress.ValuePerBar;
        _totalBar = energyProgress.TotalBar;
        _increasePerSecond = energyProgress.IncreasePerSecond;

        RegisterEvents();

        Debug.Log("[EnergyProgressManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        AddEnergyValue(deltaTime * _increasePerSecond);
    }

    public void Dispose()
    {
        UnregisterEvents();

        Debug.Log("[EnergyProgressManager] 已释放");
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
    /// 获取单条所需值
    /// </summary>
    public float GetValuePerBar()
    {
        return _valuePerBar;
    }

    /// <summary>
    /// 获取当前能量值
    /// </summary>
    public float GetCurrentEnergyValue()
    {
        return _currentEnergyValue;
    }

    /// <summary>
    /// 获取总条数
    /// </summary>
    public int GetTotalBar()
    {
        return _totalBar;
    }

    /// <summary>
    /// 获取已完成的能量条数
    /// </summary>
    public int GetCompletedBars()
    {
        return _completedBars;
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

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    /// <summary>
    /// 动物掉落奖励回调
    /// </summary>
    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        AddEnergyValue(args.DropRewards[EDropType.Energy]);
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

