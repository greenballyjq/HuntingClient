using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 缓存命中测试脚本
/// 测试缓存命中时是否是同步执行
/// </summary>
public class CacheHitTest : MonoBehaviour
{
    /// <summary>
    /// 模拟缓存字典
    /// </summary>
    private Dictionary<string, GameObject> _cache = new Dictionary<string, GameObject>();

    /// <summary>
    /// 调用次数
    /// </summary>
    private int _callCount = 0;

    private void Start()
    {
        Debug.Log($"[第 {Time.frameCount} 帧] Start - 开始测试");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _callCount++;
            Debug.Log($"[第 {Time.frameCount} 帧] 第 {_callCount} 次调用 LoadAsync");
            LoadAsync("TestPrefab").Forget();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestLoop().Forget();
        }
    }

    /// <summary>
    /// 循环测试
    /// </summary>
    private async UniTask TestLoop()
    {
        Debug.Log($"[第 {Time.frameCount} 帧] ===== 开始循环测试 =====");
        
        // 先加载一次，确保缓存建立
        Debug.Log($"[第 {Time.frameCount} 帧] 第一次加载（建立缓存）");
        await LoadAsync("TestPrefab");
        
        // 等待一帧，确保第一次加载完成
        await UniTask.DelayFrame(1);
        
        // 循环调用十次
        Debug.Log($"[第 {Time.frameCount} 帧] ===== 开始循环调用 10 次 =====");
        for (int i = 1; i <= 10; i++)
        {
            _callCount++;
            int frameBeforeLoop = Time.frameCount;
            Debug.Log($"[第 {frameBeforeLoop} 帧] ----- 循环第 {i} 次调用 -----");
            await LoadAsync("TestPrefab");
            int frameAfterLoop = Time.frameCount;
            Debug.Log($"[第 {frameAfterLoop} 帧] 循环第 {i} 次完成，帧数变化: {frameAfterLoop - frameBeforeLoop}");
        }
        
        Debug.Log($"[第 {Time.frameCount} 帧] ===== 循环测试完成 =====");
    }

    /// <summary>
    /// 模拟异步加载方法
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>加载的对象</returns>
    private async UniTask<GameObject> LoadAsync(string path)
    {
        int frameBefore = Time.frameCount;
        Debug.Log($"[第 {frameBefore} 帧] LoadAsync 开始执行，路径: {path}");

        // 检查缓存
        if (_cache.TryGetValue(path, out var cachedObj))
        {
            int frameAfter = Time.frameCount;
            Debug.Log($"[第 {frameAfter} 帧] ✓ 缓存命中！同步返回，帧数变化: {frameAfter - frameBefore}");
            return cachedObj;
        }

        // 模拟异步加载（延迟几帧）
        Debug.Log($"[第 {Time.frameCount} 帧] 缓存未命中，开始异步加载...");
        await UniTask.DelayFrame(5);
        
        int frameAfterLoad = Time.frameCount;
        Debug.Log($"[第 {frameAfterLoad} 帧] 异步加载完成，帧数变化: {frameAfterLoad - frameBefore}");

        // 创建对象并缓存
        GameObject obj = new GameObject($"Loaded_{path}_{_callCount}");
        _cache[path] = obj;

        Debug.Log($"[第 {Time.frameCount} 帧] 对象已创建并缓存: {obj.name}");
        return obj;
    }
}

