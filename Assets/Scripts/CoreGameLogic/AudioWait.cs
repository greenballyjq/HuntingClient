using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using UnityEngine;

/// <summary>
/// 等待一次短时播放自然结束。被拒绝、循环播放、Stop 打断时都不会完成等待中的结束通知。
/// </summary>
public static class AudioWait
{
    /// <summary>
    /// 播放一条音效并等到它自然结束。定义为 null、管理器为 null 或请求被拒绝时立即返回。
    /// </summary>
    /// <param name="audioManager">音频管理器。为 null 时什么都不做。</param>
    /// <param name="cue">播放定义。为 null 时什么都不做。</param>
    public static UniTask UntilEnd(AudioManager audioManager, SfxCue cue)
    {
        if (audioManager == null || cue == null)
            return UniTask.CompletedTask;

        var tcs = new UniTaskCompletionSource();
        PlayResult result = audioManager.Play(cue, _ => tcs.TrySetResult());
        return result.Succeeded ? tcs.Task : UniTask.CompletedTask;
    }

    /// <summary>
    /// 在指定世界坐标播放一条音效并等到它自然结束。定义为 null、管理器为 null 或请求被拒绝时立即返回。
    /// 坐标只在三维定义上生效。
    /// </summary>
    /// <param name="audioManager">音频管理器。为 null 时什么都不做。</param>
    /// <param name="cue">播放定义。为 null 时什么都不做。</param>
    /// <param name="position">世界坐标。</param>
    public static UniTask UntilEnd(AudioManager audioManager, SfxCue cue, Vector3 position)
    {
        if (audioManager == null || cue == null)
            return UniTask.CompletedTask;

        var tcs = new UniTaskCompletionSource();
        PlayResult result = audioManager.Play(cue, position, _ => tcs.TrySetResult());
        return result.Succeeded ? tcs.Task : UniTask.CompletedTask;
    }

    /// <summary>
    /// 播放一条音效、播放期间跟随指定对象，并等到它自然结束。
    /// 定义为 null、管理器为 null 或请求被拒绝时立即返回。
    /// </summary>
    /// <param name="audioManager">音频管理器。为 null 时什么都不做。</param>
    /// <param name="cue">播放定义。为 null 时什么都不做。</param>
    /// <param name="target">被跟随的变换组件。</param>
    public static UniTask UntilEndAttached(AudioManager audioManager, SfxCue cue, Transform target)
    {
        if (audioManager == null || cue == null)
            return UniTask.CompletedTask;

        var tcs = new UniTaskCompletionSource();
        PlayResult result = audioManager.Play(cue, target, _ => tcs.TrySetResult());
        return result.Succeeded ? tcs.Task : UniTask.CompletedTask;
    }
}
