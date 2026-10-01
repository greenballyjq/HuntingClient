using System;
using Cysharp.Threading.Tasks;
using GameFramework.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 倒计时界面
/// </summary>
[UIForm(UILayer.Fixed)]
public class UICountdown : UIForm
{
    /// <summary>
    /// 文本组件
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCountdown;

    /// <summary>
    /// 播放倒计时
    /// </summary>
    /// <param name="textSequence">文本序列</param>
    /// <param name="totalDuration">总时长（秒）</param>
    public async UniTask PlayCountdownAsync(string[] textSequence, float totalDuration)
    {
        int count = textSequence.Length;
        float segmentDuration = totalDuration / count;

        for (int i = 0; i < count; i++)
        {
            _textCountdown.text = textSequence[i];
            await UniTask.Delay(TimeSpan.FromSeconds(segmentDuration), cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        Close();
    }
}

