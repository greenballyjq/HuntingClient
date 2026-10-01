using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 红色警报界面
/// </summary>
[UIForm(UILayer.Overlay)]
public class UIAlertRed : UIForm
{
    /// <summary>
    /// 闪烁配置
    /// </summary>
    [System.Serializable]
    private class FlashConfig
    {
        /// <summary>
        /// 频率（每秒闪烁次数）
        /// </summary>
        public float Frequency = 2f;

        /// <summary>
        /// 持续总时间（秒）
        /// </summary>
        public float Duration = 3f;

        /// <summary>
        /// 闪烁的颜色
        /// </summary>
        public Color FlashColor = Color.red;

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// 闪烁图片
    /// </summary>
    [SerializeField] private Image _imageFlash;

    /// <summary>
    /// 闪烁配置
    /// </summary>
    [SerializeField] private FlashConfig _flashConfig;

    protected override void OnOpen()
    {
        _imageFlash.color = new Color(_flashConfig.FlashColor.r, _flashConfig.FlashColor.g, _flashConfig.FlashColor.b, 0f);
    }

    /// <summary>
    /// 播放闪烁动画
    /// </summary>
    public async UniTask PlayFlashAsync()
    {
        Color originColor = _imageFlash.color;
        float halfPeriod = 0.5f / _flashConfig.Frequency;

        await _imageFlash
            .DOColor(_flashConfig.FlashColor, halfPeriod)
            .SetLink(gameObject)
            .SetUpdate(_flashConfig.UseUnscaledTime)
            .SetLoops(Mathf.CeilToInt(_flashConfig.Duration / halfPeriod), LoopType.Yoyo)
            .OnComplete(() => _imageFlash.color = originColor)
            .ToUniTask();
    }
}

