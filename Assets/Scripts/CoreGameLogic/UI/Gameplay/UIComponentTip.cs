using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 文本提示组件
/// 在屏幕上显示带闪烁效果的文本提示，支持覆盖播放与不可覆盖模式。
/// <para>覆盖播放：新提示可覆盖当前提示，被覆盖的提示入队并在后台按真实时间继续计时，当前提示播完后自动从队列恢复仍有剩余时长的提示。</para>
/// <para>不可覆盖：标记为不可覆盖的提示在播放期间会拒绝后续所有提示，直到播完。</para>
/// </summary>
public class UIComponentTip : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 闪烁配置
    /// </summary>
    [Serializable]
    private class BlinkConfig
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
    /// 待恢复的提示项
    /// </summary>
    private struct PendingTip
    {
        /// <summary>提示文本</summary>
        public string Text;
        /// <summary>文本颜色</summary>
        public Color Color;
        /// <summary>入队时的剩余播放时长（秒）</summary>
        public float RemainingAtPause;
        /// <summary>入队时刻</summary>
        public float PausedAt;
    }

    /// <summary>
    /// 提示文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _tipText;

    /// <summary>
    /// 闪烁动画配置
    /// </summary>
    [SerializeField] private BlinkConfig _blinkConfig;

    /// <summary>
    /// 被覆盖的提示队列
    /// </summary>
    private readonly Queue<PendingTip> _pending = new Queue<PendingTip>();

    /// <summary>
    /// 当前提示文本
    /// </summary>
    private string _currentText;

    /// <summary>
    /// 当前提示颜色
    /// </summary>
    private Color _currentColor;

    /// <summary>
    /// 当前提示剩余播放时长（秒）
    /// </summary>
    private float _currentRemaining;

    /// <summary>
    /// 当前提示是否可被覆盖
    /// </summary>
    private bool _currentOverridable;

    /// <summary>
    /// 当前播放任务的取消令牌源
    /// </summary>
    private CancellationTokenSource _playCts;

    /// <summary>
    /// 当前闪烁动画序列
    /// </summary>
    private Sequence _blinkSequence;

    private float _playStartedAt;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void CleanUp()
    {
        ForceStopAll();
    }

    /// <summary>
    /// 播放提示
    /// </summary>
    /// <param name="text">提示文本</param>
    /// <param name="color">文本颜色</param>
    /// <param name="duration">播放时长（秒）</param>
    /// <param name="overridable">是否可被后续提示覆盖</param>
    public void Play(string text, Color color, float duration, bool overridable = true)
    {
        // 当前不可覆盖则直接丢弃
        if (!_currentOverridable && _playCts != null)
            return;

        // 当前项入队，记录实际剩余时长和入队时刻
        if (_playCts != null)
        {
            float elapsed = Time.realtimeSinceStartup - _playStartedAt;
            float actualRemaining = Mathf.Max(0f, _currentRemaining - elapsed);
            _pending.Enqueue(new PendingTip
            {
                Text = _currentText,
                Color = _currentColor,
                RemainingAtPause = actualRemaining,
                PausedAt = Time.realtimeSinceStartup
            });
            _playCts.Cancel();
            _playCts.Dispose();
        }

        _currentText = text;
        _currentColor = color;
        _currentRemaining = duration;
        _currentOverridable = overridable;

        RefreshDisplay();
        gameObject.SetActive(true);

        _playStartedAt = Time.realtimeSinceStartup;
        _playCts = new CancellationTokenSource();
        PlayCurrentAsync(_playCts.Token).Forget();
    }

    /// <summary>
    /// 强制结束所有提示
    /// </summary>
    public void ForceStopAll()
    {
        _playCts?.Cancel();
        _playCts?.Dispose();
        _playCts = null;
        _blinkSequence?.Kill();
        _blinkSequence = null;
        _pending.Clear();
        _currentText = null;
        gameObject.SetActive(false);
    }

    #region 私有方法
    /// <summary>
    /// 刷新提示显示
    /// </summary>
    private void RefreshDisplay()
    {
        _tipText.text = _currentText;
        _tipText.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, 1f);
    }

    /// <summary>
    /// 启动闪烁动画
    /// </summary>
    private void StartBlink()
    {
        _blinkSequence?.Kill();
        float step = _blinkConfig.BlinkInterval * 0.5f;
        Color target = new Color(_currentColor.r, _currentColor.g, _currentColor.b, _blinkConfig.TargetOpacity);
        _blinkSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .SetUpdate(_blinkConfig.UseUnscaledTime)
            .Append(_tipText.DOColor(target, step))
            .Append(_tipText.DOColor(new Color(_currentColor.r, _currentColor.g, _currentColor.b, 1f), step))
            .SetLoops(-1)
            .Play();
    }

    /// <summary>
    /// 停止闪烁动画
    /// </summary>
    private void StopBlink()
    {
        _blinkSequence?.Kill();
        _blinkSequence = null;
        if (_currentText != null)
            _tipText.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, 1f);
    }

    /// <summary>
    /// 播放当前提示
    /// </summary>
    private async UniTaskVoid PlayCurrentAsync(CancellationToken token)
    {
        StartBlink();
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_currentRemaining), ignoreTimeScale: _blinkConfig.UseUnscaledTime, cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        StopBlink();
        _playCts?.Dispose();
        _playCts = null;
        TryShowNextPending();
    }

    /// <summary>
    /// 当前播完，从队列取下一个未超时的提示继续播放
    /// </summary>
    private void TryShowNextPending()
    {
        float now = Time.realtimeSinceStartup;
        while (_pending.Count > 0)
        {
            var item = _pending.Dequeue();
            // 被覆盖期间时间照常流逝
            float remaining = item.RemainingAtPause - (now - item.PausedAt);
            if (remaining <= 0f)
                continue;

            _currentText = item.Text;
            _currentColor = item.Color;
            _currentRemaining = remaining;
            _currentOverridable = true;

            RefreshDisplay();
            StartBlink();

            _playStartedAt = Time.realtimeSinceStartup;
            _playCts = new CancellationTokenSource();
            PlayCurrentAsync(_playCts.Token).Forget();
            return;
        }
        gameObject.SetActive(false);
        _currentText = null;
        _playCts = null;
    }
    #endregion
}
