using GameFramework.Utility;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// HuntingAudioRefSo 动态加载与音频播放内存测试
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class HuntingAudioRefSoLoadTest : MonoBehaviour
{
    /// <summary>
    /// 资源路径
    /// </summary>
    [SerializeField] private string _resourcePath = "HuntingAudioRefSo";

    /// <summary>
    /// 加载按键
    /// </summary>
    [SerializeField] private KeyCode _loadKey = KeyCode.A;

    private HuntingAudioRefSo _loadedAsset;
    private AudioSource _audioSource;

    /// <summary>
    /// 数字键1-9对应的音频类型
    /// </summary>
    private static readonly HuntingAudioRefSo.HuntingGameAudioType[] _audioTypes = new HuntingAudioRefSo.HuntingGameAudioType[]
    {
        HuntingAudioRefSo.HuntingGameAudioType.MapEnv_RoyalForest,
        HuntingAudioRefSo.HuntingGameAudioType.GunShoot_Default,
        HuntingAudioRefSo.HuntingGameAudioType.BulletHit_Default,
        HuntingAudioRefSo.HuntingGameAudioType.Animal_Hit,
        HuntingAudioRefSo.HuntingGameAudioType.Animal_Death,
        HuntingAudioRefSo.HuntingGameAudioType.Skill_Use,
        HuntingAudioRefSo.HuntingGameAudioType.Skill_JinZhuangYuan,
        HuntingAudioRefSo.HuntingGameAudioType.Props_Bombardment,
        HuntingAudioRefSo.HuntingGameAudioType.Effect_Explosion_SettlementPanel
    };

    #region Unity 生命周期

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_loadKey))
        {
            LoadAsset();
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;

            long before = Profiler.GetTotalAllocatedMemoryLong();
            _audioSource.PlayOneShot(_loadedAsset.GetAudioFromType(_audioTypes[i]));
            long after = Profiler.GetTotalAllocatedMemoryLong();
            Log.Info($"[HuntingAudioRefSoLoadTest] 播放 {_audioTypes[i]}，内存增量 {(after - before) / 1024} KB");
            return;
        }
    }

    #endregion

    #region 私有方法

    private void LoadAsset()
    {
        long before = Profiler.GetTotalAllocatedMemoryLong();
        _loadedAsset = Resources.Load<HuntingAudioRefSo>(_resourcePath);
        long after = Profiler.GetTotalAllocatedMemoryLong();
        Log.Info($"[HuntingAudioRefSoLoadTest] 加载完成，内存增量 {(after - before) / 1024} KB，当前总分配 {after / 1024 / 1024} MB");
    }

    #endregion
}
