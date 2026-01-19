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

    /// <summary>
    /// 总刻度数
    /// </summary>
    private int _totalScale;

    /// <summary>
    /// 当前肉度值
    /// </summary>
    private float _currentMeatValue;

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
        // 累加肉度值
        _currentMeatValue += amount;

        // 超过则封顶
        float totalMeatValue = GetTotalMeatValue();
        if (_currentMeatValue > totalMeatValue)
            _currentMeatValue = totalMeatValue;

        // 触发肉度值变化事件
        TriggerMeatValueChanged(new MeatValueChangedEventArgs
        {
            CurrentMeatValue = _currentMeatValue,
            TotalProgressRatio = GetTotalProgressRatio(),
            CompletedScaleCount = GetCompletedScaleCount()
        });
    }

    /// <summary>
    /// 获取单刻度所需值
    /// </summary>
    /// <returns>单刻度所需值</returns>
    public float GetValuePerScale()
    {
        return _valuePerScale;
    }

    /// <summary>
    /// 获取当前肉度值
    /// </summary>
    /// <returns>当前肉度值</returns>
    public float GetCurrentMeatValue()
    {
        return _currentMeatValue;
    }

    /// <summary>
    /// 获取总肉度值
    /// </summary>
    /// <returns>总肉度值</returns>
    public float GetTotalMeatValue()
    {
        return _valuePerScale * _totalScale;
    }

    /// <summary>
    /// 获取已完成的刻度数
    /// </summary>
    /// <returns>已完成的刻度数</returns>
    public int GetCompletedScaleCount()
    {
        return Mathf.FloorToInt(_currentMeatValue / _valuePerScale);
    }

    /// <summary>
    /// 获取总进度比例
    /// </summary>
    /// <returns>总进度比例（0-1之间）</returns>
    public float GetTotalProgressRatio()
    {
        float totalMeatValue = GetTotalMeatValue();
        if (totalMeatValue <= 0f)
            return 0f;

        return Mathf.Clamp01(_currentMeatValue / totalMeatValue);
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
    #endregion
}

