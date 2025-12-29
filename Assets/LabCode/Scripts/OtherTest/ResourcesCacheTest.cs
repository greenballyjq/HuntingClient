using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Resources 缓存测试脚本
/// 测试 Resources.Load 加载的资源是否缓存、过场景是否移除、是否需要手动卸载
/// </summary>
public class ResourcesCacheTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private string _resourcePath = "TestTexture"; // Resources 文件夹下的资源路径
    [SerializeField] private bool _autoTest = false;
    
    private Object _loadedResource;
    private int _loadCount = 0;

    private void Start()
    {
        TestResourcesCache();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestResourcesCache();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadResource();
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnloadUnusedAssets();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckResourceStatus();
        }
    }

    /// <summary>
    /// 测试 Resources 缓存
    /// </summary>
    [ContextMenu("测试 Resources 缓存")]
    private void TestResourcesCache()
    {
        Debug.Log("=== 开始测试 Resources 缓存 ===");
        
        // 第一次加载
        _loadCount++;
        Debug.Log($"第 {_loadCount} 次加载资源: {_resourcePath}");
        
        Object resource1 = Resources.Load(_resourcePath);
        
        if (resource1 == null)
        {
            Debug.LogWarning($"资源 {_resourcePath} 不存在！请确保 Resources 文件夹下有这个资源");
            return;
        }
        
        Debug.Log($"资源加载成功: {resource1.name}, 类型: {resource1.GetType()}");
        Debug.Log($"资源实例ID: {resource1.GetInstanceID()}");
        
        // 第二次加载相同资源
        Object resource2 = Resources.Load(_resourcePath);
        Debug.Log($"第二次加载相同资源，实例ID: {resource2.GetInstanceID()}");
        
        // 检查是否是同一个实例（缓存）
        if (resource1 == resource2)
        {
            Debug.Log("✓ 资源被缓存了！两次加载返回的是同一个实例");
        }
        else
        {
            Debug.LogWarning("✗ 资源没有被缓存！两次加载返回的是不同实例");
        }
        
        _loadedResource = resource1;
        
        // 检查内存
        LogMemoryInfo();
    }

    /// <summary>
    /// 重新加载资源
    /// </summary>
    [ContextMenu("重新加载资源")]
    private void ReloadResource()
    {
        Debug.Log("=== 重新加载资源 ===");
        
        Object oldResource = _loadedResource;
        Object newResource = Resources.Load(_resourcePath);
        
        if (oldResource != null && newResource != null)
        {
            if (oldResource == newResource)
            {
                Debug.Log("✓ 重新加载返回的是同一个实例（缓存）");
            }
            else
            {
                Debug.LogWarning("✗ 重新加载返回的是不同实例");
            }
        }
        
        _loadedResource = newResource;
    }

    /// <summary>
    /// 卸载未使用的资源
    /// </summary>
    [ContextMenu("卸载未使用的资源")]
    private void UnloadUnusedAssets()
    {
        Debug.Log("=== 卸载未使用的资源 ===");
        
        // 先释放引用
        Object oldResource = _loadedResource;
        _loadedResource = null;
        
        Debug.Log("已释放资源引用");
        
        // 卸载未使用的资源
        Resources.UnloadUnusedAssets();
        
        Debug.Log("已调用 Resources.UnloadUnusedAssets()");
        
        // 检查资源是否还在
        Object checkResource = Resources.Load(_resourcePath);
        if (checkResource == null)
        {
            Debug.LogWarning("✗ 资源已被卸载（可能资源路径不存在或真的被卸载了）");
        }
        else
        {
            if (checkResource == oldResource)
            {
                Debug.Log("资源还在（可能还有引用或资源没有被卸载）");
            }
            else
            {
                Debug.Log("资源被卸载后重新加载了新的实例");
            }
        }
        
        LogMemoryInfo();
    }

    /// <summary>
    /// 检查资源状态
    /// </summary>
    [ContextMenu("检查资源状态")]
    private void CheckResourceStatus()
    {
        Debug.Log("=== 检查资源状态 ===");
        
        if (_loadedResource == null)
        {
            Debug.Log("当前没有加载的资源");
            return;
        }
        
        Debug.Log($"资源名称: {_loadedResource.name}");
        Debug.Log($"资源类型: {_loadedResource.GetType()}");
        Debug.Log($"资源实例ID: {_loadedResource.GetInstanceID()}");
        Debug.Log($"资源是否为空: {_loadedResource == null}");
        
        // 尝试重新加载看看是否是同一个
        Object reloaded = Resources.Load(_resourcePath);
        if (reloaded == _loadedResource)
        {
            Debug.Log("✓ 资源仍在缓存中");
        }
        else
        {
            Debug.LogWarning("✗ 资源不在缓存中（可能是新实例）");
        }
        
        LogMemoryInfo();
    }

    /// <think>
    /// 用户想要测试过场景是否移除资源。我需要添加一个方法来测试场景切换。
    /// </think>
    /// <summary>
    /// 测试过场景后资源是否还在
    /// </summary>
    [ContextMenu("测试过场景（需要两个场景）")]
    private void TestSceneTransition()
    {
        Debug.Log("=== 测试过场景 ===");
        
        if (_loadedResource == null)
        {
            Debug.LogWarning("请先加载资源");
            return;
        }
        
        Object resourceBefore = _loadedResource;
        int instanceIdBefore = resourceBefore.GetInstanceID();
        
        Debug.Log($"场景切换前 - 资源实例ID: {instanceIdBefore}");
        
        // 注意：实际场景切换需要调用 SceneManager.LoadScene
        // 这里只是演示，实际测试需要在两个场景中都挂载这个脚本
        Debug.Log("提示：要测试过场景，需要在另一个场景中也挂载此脚本，然后切换场景");
        Debug.Log("切换场景后，检查资源是否还在缓存中");
    }

    /// <summary>
    /// 记录内存信息
    /// </summary>
    private void LogMemoryInfo()
    {
        long totalMemory = System.GC.GetTotalMemory(false);
        Debug.Log($"当前 GC 内存: {totalMemory / 1024 / 1024} MB");
    }

    private void OnDestroy()
    {
        Debug.Log("=== 脚本销毁 ===");
        
        if (_loadedResource != null)
        {
            Debug.Log($"脚本销毁时，资源引用还在: {_loadedResource.name}");
            Debug.Log("提示：脚本销毁不会自动卸载 Resources 加载的资源");
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.7f));
        style.padding = new RectOffset(10, 10, 10, 10);
        
        Rect rect = new Rect(10, 10, 300, 200);
        GUI.Box(rect, "", style);
        
        float y = 20;
        GUI.Label(new Rect(20, y, 280, 20), "Resources 缓存测试", style);
        y += 25;
        GUI.Label(new Rect(20, y, 280, 20), "Space - 测试缓存", style);
        y += 20;
        GUI.Label(new Rect(20, y, 280, 20), "R - 重新加载", style);
        y += 20;
        GUI.Label(new Rect(20, y, 280, 20), "U - 卸载未使用资源", style);
        y += 20;
        GUI.Label(new Rect(20, y, 280, 20), "C - 检查资源状态", style);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
