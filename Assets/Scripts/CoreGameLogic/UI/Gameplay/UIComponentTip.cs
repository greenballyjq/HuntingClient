using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

/// <summary>
/// 文本提示组件
/// </summary>
public class UIComponentTip : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 提示动画配置
    /// </summary>
    [Serializable] private class BlinkConfig
    {
        /// <summary>
        /// 单次闪烁时长（秒）
        /// </summary>
        public float BlinkInterval = 0.5f;

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
    /// 闪烁配置
    /// </summary>
    [SerializeField]
    private BlinkConfig _blinkConfig;

    /// <summary>
    /// 提示文本
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _tipText;

    /// <summary>
    /// 原始颜色
    /// </summary>
    private Color _originColor;

    /// <summary>
    /// 当前播放序列
    /// </summary>
    private Sequence _currentSequence;

    /// <summary>
    /// 播放取消令牌
    /// </summary>
    private CancellationTokenSource _playCts;

    public void Init()
    {
        _originColor = _tipText.color;
    }

    public void CleanUp() 
    {
        CleanAnimation();
    }

    /// <summary>
    /// 播放闪烁动画
    /// </summary>
    /// <param name="text">提示文本</param>
    /// <param name="textColor">文本颜色</param>
    /// <param name="duration">播放总时长（秒）</param>
    public async UniTask PlayTipAnimationAsync(string text, Color textColor, float duration)
    {
        CleanAnimation();

        _playCts = new CancellationTokenSource();
        var token = _playCts.Token;

        _tipText.text = text;

        Color originColor = _originColor;
        originColor = new Color(textColor.r, textColor.g, textColor.b, 1f);
        _tipText.color = originColor;

        float stepDuration = _blinkConfig.BlinkInterval / 2f;
        Color targetColor = new Color(originColor.r, originColor.g, originColor.b, _blinkConfig.TargetOpacity);
        int blinkCount = Mathf.Max(1, Mathf.CeilToInt(duration / _blinkConfig.BlinkInterval));

        _currentSequence = DOTween.Sequence();
        for (int i = 0; i < blinkCount; i++)
        {
            _currentSequence.Append(_tipText.DOColor(targetColor, stepDuration));
            _currentSequence.Append(_tipText.DOColor(originColor, stepDuration));
        }

        _currentSequence.SetUpdate(_blinkConfig.UseUnscaledTime);
        _currentSequence.OnComplete(() => { _tipText.color = originColor; });
        _currentSequence.Play();


        while (_currentSequence.IsActive())
        {
            token.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// 清理动画
    /// </summary>
    private void CleanAnimation()
    {
        _playCts?.Cancel();
        _playCts?.Dispose();
        _currentSequence?.Kill();
        _playCts = null;
        _currentSequence = null;
    }
}
