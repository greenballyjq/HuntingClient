using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using System;

/// <summary>
/// 道具使用提示组件
/// </summary>
public class UIComponentPropUseTip : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 闪烁动画配置
    /// </summary>
    [Serializable]
    private class BlinkConfig
    {
        /// <summary>
        /// 闪烁次数
        /// </summary>
        public int BlinkCount = 3;

        /// <summary>
        /// 单次间隔时长（秒）
        /// </summary>
        public float IntervalDuration = 0.5f;

        /// <summary>
        /// 目标透明度
        /// </summary>
        [Range(0f, 1f)]
        public float TargetOpacity = 0.5f;

        /// <summary>
        /// 是否使用未缩放时间
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// 提示文本
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _tipText;

    /// <summary>
    /// 闪烁配置
    /// </summary>
    [SerializeField]
    private BlinkConfig _blinkConfig;

    /// <summary>
    /// 原始颜色
    /// </summary>
    private Color _originColor;

    public void Init()
    {
        _originColor = _tipText.color;
    }

    public void CleanUp() { }

    /// <summary>
    /// 播放闪烁动画
    /// </summary>
    public async UniTask PlayBlinkAsync()
    {
        float stepDuration = _blinkConfig.IntervalDuration / 2f;
        Color targetColor = new Color(_originColor.r, _originColor.g, _originColor.b, _blinkConfig.TargetOpacity);

        Sequence blinkSequence = DOTween.Sequence();

        for (int i = 0; i < _blinkConfig.BlinkCount; i++)
        {
            blinkSequence.Append(_tipText.DOColor(targetColor, stepDuration));
            blinkSequence.Append(_tipText.DOColor(_originColor, stepDuration));
        }

        blinkSequence.SetUpdate(_blinkConfig.UseUnscaledTime);

        blinkSequence.OnComplete(() =>
        {
            _tipText.color = _originColor;
        });

        blinkSequence.Play();

        await blinkSequence.AsyncWaitForCompletion();
    }
}
