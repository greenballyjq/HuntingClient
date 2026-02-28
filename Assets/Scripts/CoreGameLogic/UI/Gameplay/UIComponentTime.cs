using GameFramework.Core.UI;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 时间显示组件
/// </summary>
public class UIComponentTime : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 时间文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textTime;

    /// <summary>
    /// 更新间隔（秒）
    /// </summary>
    [SerializeField] private float _updateInterval = 0.1f;

    /// <summary>
    /// 单局流程引用
    /// </summary>
    private RoundFlow _roundFlow => RoundFlow.Instance;

    /// <summary>
    /// 更新计时器
    /// </summary>
    private float _updateTimer;

    /// <summary>
    /// 字符串构建器（复用避免GC）
    /// </summary>
    private readonly StringBuilder _stringBuilder = new StringBuilder(12);

    /// <summary>
    /// 毫秒字体大小百分比
    /// </summary>
    private const int FontSizePercentage = 80;

    public void Init()
    {
        _updateTimer = 0f;
        UpdateTimeDisplay();
    }

    public void CleanUp()
    {
        
    }

    private void Update()
    {
        _updateTimer += Time.deltaTime;

        if (_updateTimer >= _updateInterval)
        {
            _updateTimer = 0f;
            UpdateTimeDisplay();
        }
    }

    #region 私有方法
    /// <summary>
    /// 更新时间显示
    /// </summary>
    private void UpdateTimeDisplay()
    {
        float elapsedTime = _roundFlow.RoundElapsedTime;
        int totalMilliseconds = Mathf.FloorToInt(elapsedTime * 1000f);
        int minutes = totalMilliseconds / 60000;
        int seconds = (totalMilliseconds % 60000) / 1000;
        int milliseconds = (totalMilliseconds % 1000) / 10;

        _stringBuilder.Clear();
        _stringBuilder.Append(minutes.ToString("D2"));
        _stringBuilder.Append("'");
        _stringBuilder.Append(seconds.ToString("D2"));
        _stringBuilder.Append("''");
        _stringBuilder.Append($"<size={FontSizePercentage}%>");
        _stringBuilder.Append(milliseconds.ToString("D2"));
        _stringBuilder.Append("'''</size>");

        _textTime.text = _stringBuilder.ToString();
    }
    #endregion
}

