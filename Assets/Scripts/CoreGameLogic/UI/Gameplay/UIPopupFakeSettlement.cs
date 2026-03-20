using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 假结算弹窗
/// </summary>
public class UIPopupFakeSettlement : UIPopupNormalSettlement
{
    /// <summary>
    /// 窗口抖动配置
    /// </summary>
    [Serializable]
    private class WindowShakeConfig
    {
        /// <summary>
        /// 抖动时长（秒）
        /// </summary>
        public float Duration = 3f;

        /// <summary>
        /// 抖动强度（像素）
        /// </summary>
        public float Strength = 30f;

        /// <summary>
        /// 抖动频率（每秒抖动次数）
        /// </summary>
        public float Frequency = 10f;

        /// <summary>
        /// 强度增长曲线
        /// </summary>
        public AnimationCurve StrengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// 频率增长曲线
        /// </summary>
        public AnimationCurve FrequencyCurve = AnimationCurve.Linear(0f, 0.3f, 1f, 1f);

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// 按钮发光频闪配置
    /// </summary>
    [Serializable]
    private class ButtonGlowConfig
    {
        /// <summary>
        /// 持续时长（秒）
        /// </summary>
        public float Duration = 3f;

        /// <summary>
        /// 频闪频率（每秒闪烁次数）
        /// </summary>
        public float Frequency = 6f;

        /// <summary>
        /// 发光颜色
        /// </summary>
        public Color GlowColor = Color.white;

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// 窗口矩形变换
    /// </summary>
    [SerializeField] private RectTransform _windowRectTransform;

    /// <summary>
    /// 分享按钮图片
    /// </summary>
    [SerializeField] private Image _imageButtonShare;

    /// <summary>
    /// 领取按钮图片
    /// </summary>
    [SerializeField] private Image _imageButtonClaim;

    /// <summary>
    /// 双倍领取按钮图片
    /// </summary>
    [SerializeField] private Image _imageButtonDoubleClaim;

    /// <summary>
    /// 抖动配置
    /// </summary>
    [SerializeField] private WindowShakeConfig _shakeConfig;

    /// <summary>
    /// 发光配置
    /// </summary>
    [SerializeField] private ButtonGlowConfig _buttonGlowConfig;

    /// <summary>
    /// 原始锚点位置
    /// </summary>
    private Vector2 _originAnchoredPos;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _originAnchoredPos = _windowRectTransform.anchoredPosition;
    }

    #region 公共方法
    /// <summary>
    /// 播放抖动动画
    /// </summary>
    public async UniTask PlayWindowShakeAsync()
    {
        float elapsed = 0f;

        await DOTween.To(
                () => elapsed,
                x => elapsed = x,
                _shakeConfig.Duration,
                _shakeConfig.Duration
            )
            .SetLink(gameObject)
            .SetUpdate(_shakeConfig.UseUnscaledTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float t = elapsed / _shakeConfig.Duration;
                float strength01 = _shakeConfig.StrengthCurve.Evaluate(t);
                float freq01 = _shakeConfig.FrequencyCurve.Evaluate(t);
                float currentStrength = _shakeConfig.Strength * strength01;
                float currentFrequency = _shakeConfig.Frequency * freq01;
                float time = (_shakeConfig.UseUnscaledTime ? Time.unscaledTime : Time.time) * currentFrequency;
                float nx = Mathf.PerlinNoise(0f, time) * 2f - 1f;
                float ny = Mathf.PerlinNoise(100f, time) * 2f - 1f;
                Vector2 offset = new Vector2(nx, ny) * currentStrength;
                _windowRectTransform.anchoredPosition = _originAnchoredPos + offset;
            })
            .OnComplete(() => _windowRectTransform.anchoredPosition = _originAnchoredPos)
            .ToUniTask();
    }

    /// <summary>
    /// 播放按钮发光频闪
    /// </summary>
    public async UniTask PlayButtonGlowAsync()
    {
        Image[] images = { _imageButtonShare, _imageButtonClaim, _imageButtonDoubleClaim };
        Color[] originColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
            originColors[i] = images[i].color;

        float halfPeriod = 0.5f / _buttonGlowConfig.Frequency;
        int loopCount = Mathf.CeilToInt(_buttonGlowConfig.Duration / halfPeriod);

        var tasks = new UniTask[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            int idx = i;
            tasks[i] = images[i]
                .DOColor(_buttonGlowConfig.GlowColor, halfPeriod)
                .SetLink(gameObject)
                .SetUpdate(_buttonGlowConfig.UseUnscaledTime)
                .SetLoops(loopCount, LoopType.Yoyo)
                .OnComplete(() => images[idx].color = originColors[idx])
                .ToUniTask();
        }
        await UniTask.WhenAll(tasks);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 分享按钮点击事件回调
    /// </summary>
    protected override void OnShareButtonClicked()
    {
        RemoveAllButtonListeners();
        RoundFlow.SwitchToNextMode(EGameplayMode.HiddenMap).Forget();
    }

    /// <summary>
    /// 领取按钮点击事件回调
    /// </summary>
    protected override void OnClaimButtonClicked()
    {
        RemoveAllButtonListeners();
        RoundFlow.SwitchToNextMode(EGameplayMode.HiddenMap).Forget();
    }

    /// <summary>
    /// 双倍领取按钮点击事件回调
    /// </summary>
    protected override void OnDoubleClaimButtonClicked()
    {
        RemoveAllButtonListeners();
        RoundFlow.SwitchToNextMode(EGameplayMode.HiddenMap).Forget();
    }

    /// <summary>
    /// 移除所有按钮监听
    /// </summary>
    private void RemoveAllButtonListeners()
    {
        ButtonShare.onClick.RemoveListener(OnShareButtonClicked);
        ButtonClaim.onClick.RemoveListener(OnClaimButtonClicked);
        ButtonDoubleClaim.onClick.RemoveListener(OnDoubleClaimButtonClicked);
    }
    #endregion
}
