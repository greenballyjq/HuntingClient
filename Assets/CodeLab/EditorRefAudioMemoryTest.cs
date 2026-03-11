using GameFramework.Utility;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// 编辑器拖入引用的音频资源是否直接占用内存测试
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class EditorRefAudioMemoryTest : MonoBehaviour
{
    /// <summary>
    /// 拖入的音频（从编辑器引用）
    /// </summary>
    [SerializeField] private AudioClip _audioClip;

    /// <summary>
    /// 播放按键
    /// </summary>
    [SerializeField] private KeyCode _playKey = KeyCode.P;

    private AudioSource _audioSource;

    #region Unity 生命周期

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        long total = Profiler.GetTotalAllocatedMemoryLong();
        Log.Info($"[EditorRefAudioMemoryTest] 当前总分配 {total / 1024} KB");

        if (_audioClip == null)
        {
            Log.Info("[EditorRefAudioMemoryTest] 未拖入音频");
            return;
        }

        long clipMemory = Profiler.GetRuntimeMemorySizeLong(_audioClip);
        Log.Info($"[EditorRefAudioMemoryTest] 音频已引用，RuntimeMemorySize {clipMemory / 1024} KB");
    }

    private void Update()
    {
        if (!Input.GetKeyDown(_playKey)) return;

        long before = Profiler.GetTotalAllocatedMemoryLong();
        _audioSource.PlayOneShot(_audioClip);
        long after = Profiler.GetTotalAllocatedMemoryLong();
        Log.Info($"[EditorRefAudioMemoryTest] 播放后内存增量 {(after - before) / 1024} KB");
    }

    #endregion
}
