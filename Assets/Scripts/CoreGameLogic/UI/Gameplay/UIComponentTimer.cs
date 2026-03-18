using GameFramework.Core.UI;
using GameFramework.Manager;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 计时组件
/// </summary>
public class UIComponentTimer : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 时间文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textTime;

    /// <summary>
    /// 计时规则
    /// </summary>
    private ITimingRule _timingRule;

    /// <summary>
    /// 字符串构建器
    /// </summary>
    private StringBuilder _stringBuilder = new StringBuilder(12);

    private RoundFlow _roundFlow => RoundFlow.Instance;
    private EventManager _eventManager;
    
    private const int ELAPSED_TIME_MS_FONT_SIZE_PERCENTAGE = 80;
    private const int COUNTDOWN_FONT_SIZE_PERCENTAGE = 160;

    private void Awake()
    {
        RegisterServers();
    }

    public void Init()
    {
        _eventManager.AddListener(RoundEvents.TimeUpdated, OnTimeUpdated);

        _timingRule = _roundFlow.GetPlayRule<ITimingRule>();
        if (_timingRule.IsCountdown)
            RefreshCountdownDisplay(_timingRule.GetCurrentSeconds());
        else
            RefreshElapsedTimeDisplay(_timingRule.GetCurrentSeconds());
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(RoundEvents.TimeUpdated, OnTimeUpdated);
        _timingRule = null;
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _eventManager = GameServiceLocator.EventManager;
        
    }

    /// <summary>
    /// 刷新倒计时显示
    /// </summary>
    private void RefreshCountdownDisplay(float seconds)
    {
        int remainingSeconds = Mathf.FloorToInt(seconds);
        _textTime.text = $"<size={COUNTDOWN_FONT_SIZE_PERCENTAGE}%>{remainingSeconds}</size>";
    }

    /// <summary>
    /// 刷新累加时间显示
    /// </summary>
    private void RefreshElapsedTimeDisplay(float seconds)
    {
        int totalMilliseconds = Mathf.FloorToInt(seconds * 1000f);
        int minutes = totalMilliseconds / 60000;
        int secondsPart = (totalMilliseconds % 60000) / 1000;
        int milliseconds = (totalMilliseconds % 1000) / 10;

        _stringBuilder.Clear();
        _stringBuilder.Append(minutes.ToString("D2"));
        _stringBuilder.Append("'");
        _stringBuilder.Append(secondsPart.ToString("D2"));
        _stringBuilder.Append("''");
        _stringBuilder.Append($"<size={ELAPSED_TIME_MS_FONT_SIZE_PERCENTAGE}%>");
        _stringBuilder.Append(milliseconds.ToString("D2"));
        _stringBuilder.Append("'''</size>");

        _textTime.text = _stringBuilder.ToString();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 时间更新事件回调
    /// </summary>
    private void OnTimeUpdated(TimeUpdatedEventArgs args)
    {
        if (args.IsCountdown)
            RefreshCountdownDisplay(args.Seconds);
        else
            RefreshElapsedTimeDisplay(args.Seconds);
    }
    #endregion
}
