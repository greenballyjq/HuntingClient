using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 异步场景加载测试 - 测试异步加载时其他脚本是否会执行
/// </summary>
public class AsyncSceneLoadTest : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "LoadSceneEnd";
    [SerializeField] private string loadingUIName = "UILoadingTest02";
    private bool hasLoaded = false;
    private AsyncOperation asyncOperation;
    private UILoadingTest2 _uiTestLoading;
    private Coroutine _monitorCoroutine;

    #region 场景切换核心逻辑
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        UpdateDebugInfo();

        if (Input.GetKeyDown(KeyCode.Space) && !hasLoaded)
        {
            LoadScene();
        }
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    private void LoadScene()
    {
        hasLoaded = true;
        RecordLoadStartInfo();
        
        _uiTestLoading = TestUIManager.Instance.OpenUI<UILoadingTest2>(loadingUIName, TestUIManager.UILayer.Loading);
        asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
        asyncOperation.allowSceneActivation = false;
        
        // 启动进度协程
        _monitorCoroutine = StartCoroutine(ProgressCoroutine());
    }
    #endregion

    #region 调试相关
    private string statusText = "按空格键异步加载场景";
    private int lastUpdateFrame = 0;
    private float lastUpdateTime = 0f;
    private int loadBeforeFrame = 0;
    private float loadBeforeTime = 0f;
    private int loadAfterFrame = 0;
    private float loadAfterTime = 0f;
    private float loadProgress = 0f;

    /// <summary>
    /// 更新调试信息
    /// </summary>
    private void UpdateDebugInfo()
    {
        lastUpdateFrame = Time.frameCount;
        lastUpdateTime = Time.time;

        if (asyncOperation != null && !asyncOperation.isDone)
        {
            loadProgress = asyncOperation.progress;
        }
    }

    /// <summary>
    /// 记录加载开始信息
    /// </summary>
    private void RecordLoadStartInfo()
    {
        loadBeforeFrame = Time.frameCount;
        loadBeforeTime = Time.time;
        statusText = "正在异步加载场景...";
    }

    /// <summary>
    /// 记录加载结束信息
    /// </summary>
    private void RecordLoadEndInfo()
    {
        loadAfterFrame = Time.frameCount;
        loadAfterTime = Time.time;
        statusText = "场景加载完成";
        loadProgress = 1f;
    }

    /// <summary>
    /// 进度协程
    /// </summary>
    private IEnumerator ProgressCoroutine()
    {
        // 第一阶段：Unity场景加载 0-0.2
        _uiTestLoading.SetStatusText("正在加载场景资源...");
        while (asyncOperation.progress < 0.9f)
        {
            float realProgress = asyncOperation.progress / 0.9f * 0.2f;
            _uiTestLoading.SetProgress(realProgress);
            yield return null;
        }
        
        _uiTestLoading.SetProgress(0.2f);
        
        // 第二阶段：执行后续步骤
        _uiTestLoading.SetStatusText("正在初始化场景...");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        float startProgress = 0.2f;
        float targetProgress = 0.4f;
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Lerp(startProgress, targetProgress, elapsed / duration);
            _uiTestLoading.SetProgress(progress);
            yield return null;
        }
        
        _uiTestLoading.SetProgress(0.4f);
        _uiTestLoading.SetStatusText("正在预加载动态资源...");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        
        startProgress = 0.4f;
        targetProgress = 0.6f;
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Lerp(startProgress, targetProgress, elapsed / duration);
            _uiTestLoading.SetProgress(progress);
            yield return null;
        }
        
        _uiTestLoading.SetProgress(0.6f);
        asyncOperation.allowSceneActivation = true;
        _uiTestLoading.SetStatusText("正在初始化本局管理器...");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        
        startProgress = 0.6f;
        targetProgress = 1.0f;
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Lerp(startProgress, targetProgress, elapsed / duration);
            _uiTestLoading.SetProgress(progress);
            yield return null;
        }
        
        _uiTestLoading.SetProgress(1.0f);
        
        RecordLoadEndInfo();
        TestUIManager.Instance.CloseUI(loadingUIName);
    }

    /// <summary>
    /// 显示调试GUI
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 350));
        GUILayout.Label("=== 异步场景加载测试 ===", GUI.skin.box);
        GUILayout.Label($"目标场景: {targetSceneName}");
        GUILayout.Label($"状态: {statusText}");
        if (hasLoaded && asyncOperation != null && !asyncOperation.isDone)
        {
            GUILayout.Label($"加载进度: {loadProgress:P0}");
        }
        GUILayout.Space(10);
        
        GUILayout.Label($"Update执行 - 帧:{lastUpdateFrame} 时间:{lastUpdateTime:F3}");
        if (hasLoaded)
        {
            if (lastUpdateFrame > loadBeforeFrame)
            {
                GUILayout.Label("✓ Update仍在执行（异步不阻塞）", GUI.skin.box);
            }
        }
        
        GUILayout.Space(10);
        if (hasLoaded)
        {
            GUILayout.Label($"加载前 - 帧:{loadBeforeFrame} 时间:{loadBeforeTime:F3}");
            if (loadAfterFrame > 0)
            {
                GUILayout.Label($"加载后 - 帧:{loadAfterFrame} 时间:{loadAfterTime:F3}");
                int frameDiff = loadAfterFrame - loadBeforeFrame;
                GUILayout.Label($"帧数差: {frameDiff} (异步加载不阻塞，帧数会持续增加)");
            }
        }
        GUILayout.EndArea();
    }
    #endregion
}

