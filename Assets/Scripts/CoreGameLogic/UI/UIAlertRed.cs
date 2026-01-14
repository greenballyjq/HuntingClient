using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 红色警报界面
/// </summary>
public class UIAlertRed : UIBase
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
        public bool UseUnscaledTime = true;
    }

    /// <summary>
    /// 闪烁图片
    /// </summary>
    [SerializeField] private Image _imageFlash;

    /// <summary>
    /// 闪烁配置
    /// </summary>
    [SerializeField] private FlashConfig _flashConfig;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _imageFlash.color = new Color(_flashConfig.FlashColor.r, _flashConfig.FlashColor.g, _flashConfig.FlashColor.b, 0f);
    }

    /// <summary>
    /// 播放闪烁动画
    /// </summary>
    public async UniTask PlayFlashAsync()
    {
        Color originColor = _imageFlash.color;
        float halfPeriod = 0.5f / _flashConfig.Frequency;

        Tween flashTween = _imageFlash
            .DOColor(_flashConfig.FlashColor, halfPeriod)
            .SetLoops(Mathf.CeilToInt(_flashConfig.Duration / halfPeriod), LoopType.Yoyo)
            .SetUpdate(_flashConfig.UseUnscaledTime)
            .OnComplete(() =>
            {
                _imageFlash.color = originColor;
            });

        while (flashTween.IsActive())
            await UniTask.Yield();
    }
}

