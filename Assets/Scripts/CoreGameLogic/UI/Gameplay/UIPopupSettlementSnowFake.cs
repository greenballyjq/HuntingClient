using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// 假结算弹窗
/// </summary>
public class UIPopupSettlementSnowFake : UIBase
{
    /// <summary>
    /// 窗口抖动配置
    /// </summary>
    [System.Serializable]
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
        public bool UseUnscaledTime = true;
    }

    /// <summary>
    /// 按钮发光频闪配置
    /// </summary>
    [System.Serializable]
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
        public bool UseUnscaledTime = true;
    }

    /// <summary>
    /// 金币文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCoin;

    /// <summary>
    /// 熟练度文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textMastery;

    /// <summary>
    /// 按钮图片
    /// </summary>
    [SerializeField] private Image _imageEnterHiddenMapButton;

    /// <summary>
    /// 进入隐藏地图按钮
    /// </summary>
    [SerializeField] private Button _buttonEnterHiddenMap;

    /// <summary>
    /// 抖动配置
    /// </summary>
    [SerializeField] private WindowShakeConfig _shakeConfig;

    /// <summary>
    /// 发光配置
    /// </summary>
    [SerializeField] private ButtonGlowConfig _buttonGlowConfig;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 结算管理器
    /// </summary>
    private SettlementRewardManager _settlementRewardManager => GameServiceLocator.GetRoundManager<SettlementRewardManager>();

    /// <summary>
    /// 窗口矩形变换
    /// </summary>
    private RectTransform _rectTransform;

    /// <summary>
    /// 原始锚点位置
    /// </summary>
    private Vector2 _originAnchoredPos;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originAnchoredPos = _rectTransform.anchoredPosition;

        _buttonEnterHiddenMap.onClick.AddListener(OnEnterHiddenMapButtonClick);
    }

    private void OnDestroy()
    {
        _buttonEnterHiddenMap.onClick.RemoveListener(OnEnterHiddenMapButtonClick);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _eventManager.AddListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);

        // 计算奖励
        _settlementRewardManager.CalculateReward();
    }

    public override void OnClose()
    {
        _eventManager.RemoveListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);

        base.OnClose();
    }

    #region 公共方法
    /// <summary>
    /// 播放抖动动画
    /// </summary>
    public async UniTask PlayWindowShakeAsync()
    {
        float elapsed = 0f;

        Tween shakeTween = DOTween.To(
                () => elapsed,
                x => elapsed = x,
                _shakeConfig.Duration,
                _shakeConfig.Duration
            )
            .SetEase(Ease.Linear)
            .SetUpdate(_shakeConfig.UseUnscaledTime)
            .OnUpdate(() =>
            {
                // 归一化时间
                float t = elapsed / _shakeConfig.Duration;

                // 曲线映射
                float strength01 = _shakeConfig.StrengthCurve.Evaluate(t);
                float freq01 = _shakeConfig.FrequencyCurve.Evaluate(t);

                float currentStrength = _shakeConfig.Strength * strength01;
                float currentFrequency = _shakeConfig.Frequency * freq01;

                float time = (_shakeConfig.UseUnscaledTime ? Time.unscaledTime : Time.time) * currentFrequency;
                float nx = Mathf.PerlinNoise(0f, time) * 2f - 1f;
                float ny = Mathf.PerlinNoise(100f, time) * 2f - 1f;

                // 计算偏移并设置位置
                Vector2 offset = new Vector2(nx, ny) * currentStrength;
                _rectTransform.anchoredPosition = _originAnchoredPos + offset;
            })
            .OnComplete(() =>
            {
                _rectTransform.anchoredPosition = _originAnchoredPos;
            });

        while (shakeTween.IsActive())
            await UniTask.Yield();
    }

    /// <summary>
    /// 播放按钮发光频闪
    /// </summary>
    public async UniTask PlayButtonGlowAsync()
    {
        Color originColor = _imageEnterHiddenMapButton.color;

        float halfPeriod = 0.5f / _buttonGlowConfig.Frequency;

        Tween glowTween = _imageEnterHiddenMapButton
            .DOColor(_buttonGlowConfig.GlowColor, halfPeriod)
            .SetLoops(Mathf.CeilToInt(_buttonGlowConfig.Duration / halfPeriod), LoopType.Yoyo)
            .SetUpdate(_buttonGlowConfig.UseUnscaledTime)
            .OnComplete(() =>
            {
                _imageEnterHiddenMapButton.color = originColor;
            });

        while (glowTween.IsActive())
            await UniTask.Yield();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 进入隐藏地图按钮点击回调
    /// </summary>
    private async void OnEnterHiddenMapButtonClick()
    {
        _buttonEnterHiddenMap.onClick.RemoveListener(OnEnterHiddenMapButtonClick);

        await RoundFlow.Instance.EnterHiddenMapAsync();
    }

    /// <summary>
    /// 结算数据更新
    /// </summary>
    private void OnSettlementCalculated(SettlementCalculatedEventArgs args)
    {
        _textCoin.text = args.TotalCoin.ToString();
        _textMastery.text = args.TotalMastery.ToString();
    }
    #endregion 
}
