using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;

/// <summary>
/// 加载界面
/// </summary>
public class UILoading : UIBase
{
    /// <summary>
    /// 淡入淡出动画配置
    /// </summary>
    [System.Serializable]
    private class FadeConfig
    {
        /// <summary>
        /// 淡入动画时长（秒）
        /// </summary>
        public float FadeInDuration = 1f;

        /// <summary>
        /// 淡出动画时长（秒）
        /// </summary>
        public float FadeOutDuration = 1f;

        /// <summary>
        /// 总持续时长（秒）
        /// </summary>
        public float TotalDuration = 5f;

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// CanvasGroup组件
    /// </summary>
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// 淡入淡出配置
    /// </summary>
    [SerializeField] private FadeConfig _fadeConfig;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// 播放淡入动画
    /// </summary>
    public async UniTask PlayFadeInAsync()
    {
        float elapsed = 0f;
        float duration = _fadeConfig.FadeInDuration;
        bool useUnscaled = _fadeConfig.UseUnscaledTime;

        while (elapsed < duration)
        {
            float dt = useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += dt;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            await UniTask.Yield();
        }

        _canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 播放淡出动画
    /// </summary>
    public async UniTask PlayFadeOutAsync()
    {
        bool useUnscaled = _fadeConfig.UseUnscaledTime;

        float waitTime = _fadeConfig.TotalDuration - _fadeConfig.FadeInDuration - _fadeConfig.FadeOutDuration;
        if (waitTime > 0f)
        {
            float waitElapsed = 0f;
            while (waitElapsed < waitTime)
            {
                float dt = useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                waitElapsed += dt;
                await UniTask.Yield();
            }
        }

        float elapsed = 0f;
        float duration = _fadeConfig.FadeOutDuration;

        while (elapsed < duration)
        {
            float dt = useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += dt;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            await UniTask.Yield();
        }

        _canvasGroup.alpha = 0f;
    }
}

