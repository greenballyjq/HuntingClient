using UnityEngine;

/// <summary>
/// 测试脚本：使用 TestUIManager 打开 UILoadingTest01 预制体
/// </summary>
public class LoadingTestOpenUILoading : MonoBehaviour
{
    /// <summary>
    /// 要打开的UI名称
    /// </summary>
    [SerializeField] private string _uiName = "UILoadingTest01";

    /// <summary>
    /// 使用的UI层级
    /// </summary>
    [SerializeField] private TestUIManager.UILayer _uiLayer = TestUIManager.UILayer.Loading;

    private void Start()
    {
        if (TestUIManager.Instance == null)
        {
            Debug.LogError("[LoadingTestOpenUILoading] TestUIManager.Instance 为 null，请先在场景中放置 TestUIManager。");
            return;
        }

        // 打开UILoadingTest01
        TestUIManager.Instance.OpenUI<TestUIBase>(_uiName, _uiLayer);
    }
}


