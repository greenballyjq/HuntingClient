using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 倒计时界面
/// </summary>
public class UICountdown : UIBase
{
    /// <summary>
    /// 倒计时配置
    /// </summary>
    [System.Serializable]
    private class CountdownConfig
    {
        /// <summary>
        /// 总时长
        /// </summary>
        public float TotalDuration = 3f;

        /// <summary>
        /// 文本序列
        /// </summary>
        public string[] TextSequence = { "准备", "开始", "战斗" };

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime = true;
    }

    /// <summary>
    /// 文本组件
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCountdown;

    /// <summary>
    /// 倒计时配置
    /// </summary>
    [SerializeField] private CountdownConfig _countDownConfig;

    /// <summary>
    /// 播放倒计时
    /// </summary>
    public async UniTask PlayCountdownAsync()
    {
        int count = _countDownConfig.TextSequence.Length;
        float segmentDuration = _countDownConfig.TotalDuration / count;

        for (int i = 0; i < count; i++)
        {
            _textCountdown.text = _countDownConfig.TextSequence[i];
            
            if (_countDownConfig.UseUnscaledTime)
                await UniTask.Delay(System.TimeSpan.FromSeconds(segmentDuration), DelayType.UnscaledDeltaTime, cancellationToken: this.GetCancellationTokenOnDestroy());
            else
                await UniTask.Delay(System.TimeSpan.FromSeconds(segmentDuration), cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        Close();
    }
}

