using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 结算统计项组件
/// </summary>
public class UIComponentSettlementStatItem : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCount;

    /// <summary>
    /// 字符串构建器
    /// </summary>
    private StringBuilder _stringBuilder = new StringBuilder();

    /// <summary>
    /// 当前值
    /// </summary>
    private int _currentValue;

    public virtual void Init()
    {
        RefreshCountDisplay(0);
    }

    public void CleanUp() { }

    #region 公共方法
    /// <summary>
    /// 设置数量
    /// </summary>
    public void SetCount(int count)
    {
        RefreshCountDisplay(count);
    }

    /// <summary>
    /// 播放数量变化动画
    /// </summary>
    public async UniTask PlayCountAsync(int targetCount, float duration)
    {
        await DOVirtual
            .Int(_currentValue, targetCount, duration, RefreshCountDisplay)
            .SetLink(gameObject)
            .ToUniTask();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 刷新数量显示
    /// </summary>
    private void RefreshCountDisplay(int count)
    {
        _currentValue = count;
        _stringBuilder.Clear();
        _stringBuilder.Append(count);
        _textCount.text = _stringBuilder.ToString();
    }
    #endregion
}
