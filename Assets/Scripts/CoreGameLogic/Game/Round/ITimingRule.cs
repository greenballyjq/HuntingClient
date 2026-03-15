/// <summary>
/// 计时规则
/// </summary>
public interface ITimingRule
{
    /// <summary>
    /// 是否为倒计时显示
    /// </summary>
    bool IsCountdown { get; }

    /// <summary>
    /// 获取当前秒数
    /// </summary>
    float GetCurrentSeconds();
}

/// <summary>
/// 累加时间计时规则
/// </summary>
public class ElapsedTimingRule : ITimingRule
{
    private MainMapMode _mainMapMode;

    public bool IsCountdown => false;

    public ElapsedTimingRule(MainMapMode mainMapMode)
    {
        _mainMapMode = mainMapMode;
    }

    public float GetCurrentSeconds()
    {
        return _mainMapMode.ElapsedTime;
    }
}

/// <summary>
/// 倒计时计时规则
/// </summary>
public class CountdownTimingRule : ITimingRule
{
    private HiddenMapMode _hiddenMapMode;

    public bool IsCountdown => true;

    public CountdownTimingRule(HiddenMapMode hiddenMapMode)
    {
        _hiddenMapMode = hiddenMapMode;
    }

    public float GetCurrentSeconds()
    {
        return _hiddenMapMode.CountdownRemaining;
    }
}
