using UnityEngine;

/// <summary>
/// 场景生命周期测试脚本
/// 用于测试场景切换时 OnDisable 和 OnDestroy 是否会执行
/// </summary>
public class SceneLifecycleTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"[SceneLifecycleTest] Awake - GameObject: {gameObject.name}, Scene: {gameObject.scene.name}");
    }

    private void OnEnable()
    {
        Debug.Log($"[SceneLifecycleTest] OnEnable - GameObject: {gameObject.name}, Scene: {gameObject.scene.name}");
    }

    private void Start()
    {
        Debug.Log($"[SceneLifecycleTest] Start - GameObject: {gameObject.name}, Scene: {gameObject.scene.name}");
    }

    private void OnDisable()
    {
        Debug.Log($"[SceneLifecycleTest] OnDisable - GameObject: {gameObject.name}, Scene: {gameObject.scene.name}");
    }

    private void OnDestroy()
    {
        Debug.Log($"[SceneLifecycleTest] OnDestroy - GameObject: {gameObject.name}, Scene: {gameObject.scene.name}");
    }
}

