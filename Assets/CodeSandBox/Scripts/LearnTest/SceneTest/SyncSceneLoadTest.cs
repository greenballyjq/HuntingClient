using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 同步场景加载测试 - 测试同步加载时其他脚本是否会执行
/// </summary>
public class SyncSceneLoadTest : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "LoadSceneTest2";
    private bool hasLoaded = false;
    private string statusText = "按空格键同步加载场景";
    private int lastUpdateFrame = 0;
    private float lastUpdateTime = 0f;
    private int loadBeforeFrame = 0;
    private float loadBeforeTime = 0f;
    private int loadAfterFrame = 0;
    private float loadAfterTime = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // 每帧都更新，用于观察是否执行
        lastUpdateFrame = Time.frameCount;
        lastUpdateTime = Time.time;

        if (Input.GetKeyDown(KeyCode.Space) && !hasLoaded)
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        hasLoaded = true;
        loadBeforeFrame = Time.frameCount;
        loadBeforeTime = Time.time;
        statusText = "正在同步加载场景...";
        SceneManager.LoadScene(targetSceneName);
        loadAfterFrame = Time.frameCount;
        loadAfterTime = Time.time;
        statusText = "场景加载完成";
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 350));
        GUILayout.Label("=== 同步场景加载测试 ===", GUI.skin.box);
        GUILayout.Label($"目标场景: {targetSceneName}");
        GUILayout.Label($"状态: {statusText}");
        GUILayout.Space(10);
        
        // 实时显示Update是否在执行
        GUILayout.Label($"Update执行 - 帧:{lastUpdateFrame} 时间:{lastUpdateTime:F3}");
        if (hasLoaded)
        {
            float timeSinceLoad = Time.time - loadBeforeTime;
            GUILayout.Label($"加载耗时: {timeSinceLoad:F3}秒");
            if (lastUpdateFrame == loadBeforeFrame)
            {
                GUILayout.Label("⚠️ Update已停止（同步阻塞）", GUI.skin.box);
            }
            else
            {
                GUILayout.Label("✓ Update仍在执行", GUI.skin.box);
            }
        }
        
        GUILayout.Space(10);
        if (hasLoaded)
        {
            GUILayout.Label($"加载前 - 帧:{loadBeforeFrame} 时间:{loadBeforeTime:F3}");
            GUILayout.Label($"加载后 - 帧:{loadAfterFrame} 时间:{loadAfterTime:F3}");
            int frameDiff = loadAfterFrame - loadBeforeFrame;
            GUILayout.Label($"帧数差: {frameDiff} (同步加载会阻塞，帧数差应该很小)");
        }
        GUILayout.EndArea();
    }
}

