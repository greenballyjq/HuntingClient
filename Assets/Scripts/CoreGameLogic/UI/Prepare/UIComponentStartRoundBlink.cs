using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始打猎按钮闪烁
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Button))]
public class UIComponentStartRoundBlink : MonoBehaviour
{
    /// <summary>
    /// 闪烁动画配置
    /// </summary>
    [Serializable]
    private class BlinkConfig
    {
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
    /// 闪烁配置
    /// </summary>
    [SerializeField]
    private BlinkConfig _blinkConfig = new BlinkConfig();

    /// <summary>
    /// 原始透明度
    /// </summary>
    private float _originAlpha = 1f;

    private CanvasGroup _canvasGroup;
    private Tween _blinkTween;
    private bool _isBlinking;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _originAlpha = _canvasGroup.alpha;
    }

    private void OnDisable()
    {
        StopBlink();
    }

    #region 公共方法
    /// <summary>
    /// 开始循环闪烁
    /// </summary>
    public void StartBlink()
    {
        if (_isBlinking)
            return;

        _isBlinking = true;
        _canvasGroup.alpha = _originAlpha;

        float stepDuration = _blinkConfig.IntervalDuration / 2f;

        _blinkTween = _canvasGroup
            .DOFade(_blinkConfig.TargetOpacity, stepDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .SetUpdate(_blinkConfig.UseUnscaledTime)
            .SetLink(gameObject);
    }

    /// <summary>
    /// 停止闪烁并恢复原始透明度
    /// </summary>
    public void StopBlink()
    {
        _isBlinking = false;

        if (_blinkTween != null && _blinkTween.IsActive())
            _blinkTween.Kill();
        _blinkTween = null;

        if (_canvasGroup != null)
            _canvasGroup.alpha = _originAlpha;
    }
    #endregion
}
