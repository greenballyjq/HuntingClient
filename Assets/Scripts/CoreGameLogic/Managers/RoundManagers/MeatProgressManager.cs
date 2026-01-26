using cfg.HuntingConfig.Enum;
using UnityEngine;

/// <summary>
/// 肉度条管理器
/// </summary>
public class MeatProgressManager : IRoundManager
{
    /// <summary>
    /// 单刻度所需值
    /// </summary>
    private float _valuePerScale;
    public float ValuePerScale => _valuePerScale;

    /// <summary>
    /// 总刻度数
    /// </summary>
    private int _totalScale;
    public int TotalScale => _totalScale;

    /// <summary>
    /// 总肉度值
    /// </summary>
    private float _totalMeatValue;
    public float TotalMeatValue => _totalMeatValue;

    /// <summary>
    /// 当前肉度值
    /// </summary>
    private float _currentMeatValue;
    public float CurrentMeatProgress => _currentMeatValue;

    /// <summary>
    /// 已完成的刻度数
    /// </summary>
    private int _completedScaleCount;
    public int CompletedScaleCount => _completedScaleCount;

    /// <summary>
    /// 总进度比例
    /// </summary>
    public float TotalProgressRatio => Mathf.Clamp01(_currentMeatValue / _totalMeatValue);

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
        _valuePerScale = meatProgress.ValuePerScale;
        _totalScale = meatProgress.TotalScale;
        _totalMeatValue = _valuePerScale * _totalScale;
        _completedScaleCount = 0;

        RegisterEvents();

        Debug.Log("[MeatProgressManager] 初始化完成");
    }

    public void Dispose()
    {
        UnregisterEvents();

        Debug.Log("[MeatProgressManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 增加肉度值
    /// </summary>
    /// <param name="amount">增加的肉度值</param>
    public void AddMeatValue(float amount)
    {
        int beforeScaleCount = _completedScaleCount;

        _currentMeatValue += amount;
        if (_currentMeatValue > _totalMeatValue)
            _currentMeatValue = _totalMeatValue;

        _completedScaleCount = Mathf.FloorToInt(_currentMeatValue / _valuePerScale);

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
                CompletedScaleCount = _completedScaleCount
            });

            if (_completedScaleCount == _totalScale)
                _eventManager.Trigger(MeatEvents.MeatScaleFull);
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

    /// <summary>
    /// 动物掉落奖励事件回调
    /// </summary>
    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        AddMeatValue(args.DropRewards[EDropType.Meat]);
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
    #endregion
}