using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameFramework.Core;

/// <summary>
/// 登录按钮组件
/// </summary>
public class UIComponentLoginButton : MonoBehaviour, IUIComponent
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
    /// 登录按钮
    /// </summary>
    [SerializeField]
    private Button _buttonLogin;

    /// <summary>
    /// 闪烁配置
    /// </summary>
    [SerializeField]
    private BlinkConfig _blinkConfig;

    /// <summary>
    /// 原始透明度
    /// </summary>
    private float _originAlpha = 1;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init()
    {
        _buttonLogin.onClick.AddListener(OnLoginButtonClicked);
    }

    public void CleanUp()
    {
        _buttonLogin.onClick.RemoveListener(OnLoginButtonClicked);
    }

    public async UniTask PlayBlinkAsync()
    {
        float stepDuration = _blinkConfig.IntervalDuration / 2f;
        float targetAlpha = _blinkConfig.TargetOpacity;

        Sequence blinkSequence = DOTween.Sequence();

        for (int i = 0; i < _blinkConfig.BlinkCount; i++)
        {
            blinkSequence.Append(_canvasGroup.DOFade(targetAlpha, stepDuration));
            blinkSequence.Append(_canvasGroup.DOFade(_originAlpha, stepDuration));
        }

        blinkSequence.SetUpdate(_blinkConfig.UseUnscaledTime);

        blinkSequence.OnComplete(() =>
        {
            _canvasGroup.alpha = _originAlpha;
        });

        blinkSequence.Play();

        await blinkSequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// 登录按钮点击回调
    /// </summary>
    private async void OnLoginButtonClicked()
    {
        // TODO: 在此处实现登录逻辑
        GameFrameworkManager.Instance.Login();
        
        _buttonLogin.targetGraphic.raycastTarget = false;

        await PlayBlinkAsync();

        Debug.LogWarning("点击登录按钮");

        await HuntingAppFlow.Instance.EnterPrepareAsync();

        Debug.LogWarning("关闭登录界面");

        GameFrameLauncher.Instance.HideLoading();
    }
}
