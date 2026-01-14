using GameFramework.Core;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// EffectManager测试脚本
/// 测试预制体方式的API
/// </summary>
public class EffectManagerTest : MonoBehaviour
{
    /// <summary>
    /// 一次性特效预制体
    /// </summary>
    [SerializeField] private GameObject _oneShotPrefab;

    /// <summary>
    /// 循环特效预制体
    /// </summary>
    [SerializeField] private GameObject _loopPrefab;

    /// <summary>
    /// 持续时间特效预制体
    /// </summary>
    [SerializeField] private GameObject _durationPrefab;

    /// <summary>
    /// 特效生成位置
    /// </summary>
    [SerializeField] private Vector3 _spawnPosition = Vector3.zero;

    /// <summary>
    /// 特效方向
    /// </summary>
    [SerializeField] private Vector3 _spawnDirection = Vector3.forward;

    /// <summary>
    /// 持续时间（秒）
    /// </summary>
    [SerializeField] private float _duration = 3f;

    /// <summary>
    /// 当前播放的循环特效引用
    /// </summary>
    private GameObject _currentLoopEffect;

    /// <summary>
    /// 当前播放的持续时间特效引用
    /// </summary>
    private GameObject _currentDurationEffect;

    /// <summary>
    /// 特效管理器
    /// </summary>
    // private EffectManager _effectManager;

    private void Start()
    {
        GameServiceLocator.WaitForInitializationAsync();
        // _effectManager = GameServiceLocator.GetFrameworkManager<EffectManager>();
        Debug.Log("[EffectManagerTest] 测试脚本已启动");
        Debug.Log("[EffectManagerTest] 按键说明：");
        Debug.Log("[EffectManagerTest] 1 - 播放一次性特效");
        Debug.Log("[EffectManagerTest] 2 - 播放循环特效");
        Debug.Log("[EffectManagerTest] 3 - 播放持续时间特效");
        Debug.Log("[EffectManagerTest] S - 软停止当前循环特效");
        Debug.Log("[EffectManagerTest] I - 立即停止当前循环特效");
        Debug.Log("[EffectManagerTest] R - 回收当前循环特效");
        Debug.Log("[EffectManagerTest] D - 软停止当前持续时间特效");
        Debug.Log("[EffectManagerTest] F - 立即停止当前持续时间特效");
    }

    // private void Update()
    // {
    //     // 测试一次性特效
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         TestPlayOneShot();
    //     }
    //
    //     // 测试循环特效
    //     if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         TestPlayLoop();
    //     }
    //
    //     // 测试持续时间特效
    //     if (Input.GetKeyDown(KeyCode.Alpha3))
    //     {
    //         TestPlayForDuration();
    //     }
    //
    //     // 软停止循环特效
    //     if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         TestStopLoopSoft();
    //     }
    //
    //     // 立即停止循环特效
    //     if (Input.GetKeyDown(KeyCode.I))
    //     {
    //         TestStopLoopImmediate();
    //     }
    //
    //     // 回收循环特效
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         TestRecycleLoop();
    //     }
    //
    //     // 软停止持续时间特效
    //     if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         TestStopDurationSoft();
    //     }
    //
    //     // 立即停止持续时间特效
    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         TestStopDurationImmediate();
    //     }
    // }

    // #region 测试方法
    //
    // /// <summary>
    // /// 测试播放一次性特效
    // /// </summary>
    // private void TestPlayOneShot()
    // {
    //     if (_oneShotPrefab == null)
    //     {
    //         Debug.LogError("[EffectManagerTest] 一次性特效预制体未设置");
    //         return;
    //     }
    //
    //     var rot = _spawnDirection == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(_spawnDirection);
    //     var effect = _effectManager.PlayOneShot(_oneShotPrefab, _spawnPosition, rot);
    //     Debug.Log($"[EffectManagerTest] 播放一次性特效: {_oneShotPrefab.name}, 位置: {_spawnPosition}, 方向: {_spawnDirection}");
    // }
    //
    // /// <summary>
    // /// 测试播放循环特效
    // /// </summary>
    // private void TestPlayLoop()
    // {
    //     if (_loopPrefab == null)
    //     {
    //         Debug.LogError("[EffectManagerTest] 循环特效预制体未设置");
    //         return;
    //     }
    //
    //     var rot = _spawnDirection == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(_spawnDirection);
    //     _currentLoopEffect = _effectManager.PlayLoop(_loopPrefab, _spawnPosition, rot);
    //     Debug.Log($"[EffectManagerTest] 播放循环特效: {_loopPrefab.name}, 位置: {_spawnPosition}, 方向: {_spawnDirection}");
    // }
    //
    // /// <summary>
    // /// 测试播放持续时间特效
    // /// </summary>
    // private void TestPlayForDuration()
    // {
    //     if (_durationPrefab == null)
    //     {
    //         Debug.LogError("[EffectManagerTest] 持续时间特效预制体未设置");
    //         return;
    //     }
    //
    //     var rot = _spawnDirection == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(_spawnDirection);
    //     _currentDurationEffect = _effectManager.PlayForDuration(_durationPrefab, _spawnPosition, rot, _duration);
    //     Debug.Log($"[EffectManagerTest] 播放持续时间特效: {_durationPrefab.name}, 位置: {_spawnPosition}, 方向: {_spawnDirection}, 时长: {_duration}秒");
    // }
    //
    // /// <summary>
    // /// 测试软停止循环特效
    // /// </summary>
    // private void TestStopLoopSoft()
    // {
    //     if (_currentLoopEffect == null)
    //     {
    //         Debug.LogWarning("[EffectManagerTest] 没有正在播放的循环特效");
    //         return;
    //     }
    //
    //     _effectManager.Stop(_currentLoopEffect, EffectStopMode.Soft);
    //     Debug.Log($"[EffectManagerTest] 软停止循环特效: {_currentLoopEffect.name}");
    //     _currentLoopEffect = null;
    // }
    //
    // /// <summary>
    // /// 测试立即停止循环特效
    // /// </summary>
    // private void TestStopLoopImmediate()
    // {
    //     if (_currentLoopEffect == null)
    //     {
    //         Debug.LogWarning("[EffectManagerTest] 没有正在播放的循环特效");
    //         return;
    //     }
    //
    //     _effectManager.Stop(_currentLoopEffect, EffectStopMode.Immediate);
    //     Debug.Log($"[EffectManagerTest] 立即停止循环特效: {_currentLoopEffect.name}");
    //     _currentLoopEffect = null;
    // }
    //
    // /// <summary>
    // /// 测试回收循环特效
    // /// </summary>
    // private void TestRecycleLoop()
    // {
    //     if (_currentLoopEffect == null)
    //     {
    //         Debug.LogWarning("[EffectManagerTest] 没有正在播放的循环特效");
    //         return;
    //     }
    //
    //     _effectManager.Recycle(_currentLoopEffect);
    //     Debug.Log($"[EffectManagerTest] 回收循环特效: {_currentLoopEffect.name}");
    //     _currentLoopEffect = null;
    // }
    //
    // /// <summary>
    // /// 测试软停止持续时间特效
    // /// </summary>
    // private void TestStopDurationSoft()
    // {
    //     if (_currentDurationEffect == null)
    //     {
    //         Debug.LogWarning("[EffectManagerTest] 没有正在播放的持续时间特效");
    //         return;
    //     }
    //
    //     _effectManager.Stop(_currentDurationEffect, EffectStopMode.Soft);
    //     Debug.Log($"[EffectManagerTest] 软停止持续时间特效: {_currentDurationEffect.name}");
    //     _currentDurationEffect = null;
    // }
    //
    // /// <summary>
    // /// 测试立即停止持续时间特效
    // /// </summary>
    // private void TestStopDurationImmediate()
    // {
    //     if (_currentDurationEffect == null)
    //     {
    //         Debug.LogWarning("[EffectManagerTest] 没有正在播放的持续时间特效");
    //         return;
    //     }
    //
    //     _effectManager.Stop(_currentDurationEffect, EffectStopMode.Immediate);
    //     Debug.Log($"[EffectManagerTest] 立即停止持续时间特效: {_currentDurationEffect.name}");
    //     _currentDurationEffect = null;
    // }
    //
    // #endregion
}

