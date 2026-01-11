using cfg.HuntingConfig.Enum;
using UnityEngine;


/// <summary>
/// 肉度条管理器
/// </summary>
public class MeatProgressManager : IRoundManager
{
    /// <summary>
    /// 当前肉度值
    /// </summary>
    private float _currentMeatValue;

    /// <summary>
    /// 当前肉度条条数
    /// </summary>
    private int _currentMeatBars;

    /// <summary>
    /// 单条所需肉度值
    /// </summary>
    private float _requiredPerBar;

    /// <summary>
    /// 肉度条上限
    /// </summary>
    private int _maxMeatBars;

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
        var meatProgress = _configManager.GetMeatProgress(1);
        _requiredPerBar = meatProgress.RequiredPerBar;
        _maxMeatBars = meatProgress.MaxBar;

        RegisterEvents();
        
        TriggerProgressChanged(new MeatProgressChangedEventArgs
        {
            Sender = this,
            CurrentMeat = _currentMeatValue,
            CurrentBars = _currentMeatBars
        });
        
        TriggerBarCountChanged(new MeatBarCountChangedEventArgs
        {
            Sender = this,
            CurrentBars = _currentMeatBars
        });
        
        Debug.Log("[MeatProgressManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();
        Debug.Log("[MeatProgressManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 获取当前肉度值
    /// </summary>
    public float GetCurrentMeatValue() => _currentMeatValue;

    /// <summary>
    /// 获取当前肉度条条数
    /// </summary>
    public int GetCurrentMeatBars() => _currentMeatBars;

    /// <summary>
    /// 获取单条所需肉度值
    /// </summary>
    public float GetRequiredPerBar() => _requiredPerBar;

    /// <summary>
    /// 获取肉度条上限
    /// </summary>
    public int GetMaxMeatBars() => _maxMeatBars;

    /// <summary>
    /// 增加肉度
    /// </summary>
    public void AddMeat(float amount)
    {
        if (amount <= 0f || _currentMeatBars >= _maxMeatBars)
            return;

        // 累加肉度值
        _currentMeatValue += amount;
        bool barIncreased = false;

        // 检查是否跨越条数阈值
        while (_currentMeatValue >= _requiredPerBar && _currentMeatBars < _maxMeatBars)
        {
            _currentMeatValue -= _requiredPerBar;
            _currentMeatBars++;
            barIncreased = true;
        }

        if (_currentMeatBars >= _maxMeatBars)
        {
            // 达到条数上限时，清空肉度值并封顶条数
            _currentMeatBars = _maxMeatBars;
            _currentMeatValue = 0f;
        }

        TriggerProgressChanged(new MeatProgressChangedEventArgs
        {
            Sender = this,
            CurrentMeat = _currentMeatValue,
            CurrentBars = _currentMeatBars
        });
        if (barIncreased)
        {
            TriggerBarCountChanged(new MeatBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = _currentMeatBars
            });
        }
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

    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        if (!args.DropRewards.TryGetValue(EDropType.Meat, out var meatAmount) || meatAmount <= 0)
            return;

        AddMeat(meatAmount);
    }

    /// <summary>
    /// 触发肉度值变化事件
    /// </summary>
    private void TriggerProgressChanged(MeatProgressChangedEventArgs args)
    {
        _eventManager.Trigger(MeatEvents.MeatProgressChanged, args);
    }

    /// <summary>
    /// 触发肉度条变化事件
    /// </summary>
    private void TriggerBarCountChanged(MeatBarCountChangedEventArgs args)
    {
        _eventManager.Trigger(MeatEvents.MeatBarCountChanged, args);

        if (_currentMeatBars >= _maxMeatBars)
            _eventManager.Trigger(MeatEvents.MeatMaxBarsReached);
    }
    #endregion
}


