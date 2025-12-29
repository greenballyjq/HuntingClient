using UnityEngine;

/// <summary>
/// TimeScale 测试脚本
/// 用于测试当 Time.timeScale = 0 时，Update 是否还会运行
/// </summary>
public class TimeScaleTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool _autoStartTest = false;
    [SerializeField] private float _testDuration = 5f;
    [SerializeField] private float _targetTimeScale = 0f;

    [Header("显示信息")]
    [SerializeField] private bool _showDebugLog = true;

    private float _startTime;
    private float _lastUpdateTime;
    private int _updateCount;
    private bool _isTesting;
    private float _originalTimeScale;

    private void Start()
    {
        _originalTimeScale = Time.timeScale;
        
        if (_autoStartTest)
        {
            StartTest();
        }
    }

    private void Update()
    {
        _updateCount++;
        _lastUpdateTime = Time.realtimeSinceStartup;

        if (_isTesting)
        {
            // 检查测试是否应该结束
            if (Time.realtimeSinceStartup - _startTime >= _testDuration)
            {
                EndTest();
            }
        }

        // 显示当前状态
        if (_showDebugLog && _updateCount % 60 == 0) // 每秒显示一次（假设60fps）
        {
            Debug.Log($"[TimeScaleTest] Update 运行中 - Count: {_updateCount}, TimeScale: {Time.timeScale}, RealTime: {Time.realtimeSinceStartup:F2}, DeltaTime: {Time.deltaTime:F4}");
        }
    }

    /// <summary>
    /// 开始测试
    /// </summary>
    [ContextMenu("开始测试")]
    public void StartTest()
    {
        if (_isTesting)
        {
            Debug.LogWarning("[TimeScaleTest] 测试已在进行中");
            return;
        }

        _isTesting = true;
        _startTime = Time.realtimeSinceStartup;
        _updateCount = 0;
        _originalTimeScale = Time.timeScale;

        Time.timeScale = _targetTimeScale;

        Debug.Log($"[TimeScaleTest] ===== 测试开始 =====");
        Debug.Log($"[TimeScaleTest] 原始 TimeScale: {_originalTimeScale}");
        Debug.Log($"[TimeScaleTest] 目标 TimeScale: {_targetTimeScale}");
        Debug.Log($"[TimeScaleTest] 测试时长: {_testDuration} 秒");
        Debug.Log($"[TimeScaleTest] Update 将继续运行，但 Time.deltaTime 会受影响");
    }

    /// <summary>
    /// 结束测试
    /// </summary>
    [ContextMenu("结束测试")]
    public void EndTest()
    {
        if (!_isTesting)
        {
            Debug.LogWarning("[TimeScaleTest] 没有正在进行的测试");
            return;
        }

        _isTesting = false;
        Time.timeScale = _originalTimeScale;

        float elapsedTime = Time.realtimeSinceStartup - _startTime;

        Debug.Log($"[TimeScaleTest] ===== 测试结束 =====");
        Debug.Log($"[TimeScaleTest] 实际耗时: {elapsedTime:F2} 秒");
        Debug.Log($"[TimeScaleTest] Update 调用次数: {_updateCount}");
        Debug.Log($"[TimeScaleTest] 平均 FPS: {_updateCount / elapsedTime:F2}");
        Debug.Log($"[TimeScaleTest] TimeScale 已恢复为: {Time.timeScale}");
        Debug.Log($"[TimeScaleTest] 结论: Update 方法在 TimeScale = 0 时仍然会运行！");
    }

    /// <summary>
    /// 设置 TimeScale
    /// </summary>
    [ContextMenu("设置 TimeScale = 0")]
    public void SetTimeScaleToZero()
    {
        _originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Debug.Log($"[TimeScaleTest] TimeScale 设置为 0，Update 仍会运行");
    }

    /// <summary>
    /// 恢复 TimeScale
    /// </summary>
    [ContextMenu("恢复 TimeScale")]
    public void RestoreTimeScale()
    {
        Time.timeScale = _originalTimeScale;
        Debug.Log($"[TimeScaleTest] TimeScale 恢复为 {Time.timeScale}");
    }

    private void OnDestroy()
    {
        // 确保在销毁时恢复 TimeScale
        if (_isTesting)
        {
            Time.timeScale = _originalTimeScale;
        }
    }

    private void OnGUI()
    {
        if (!_showDebugLog) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;

        float yPos = 10f;
        GUI.Label(new Rect(10, yPos, 500, 30), $"TimeScale: {Time.timeScale}", style);
        yPos += 30;
        GUI.Label(new Rect(10, yPos, 500, 30), $"Update Count: {_updateCount}", style);
        yPos += 30;
        GUI.Label(new Rect(10, yPos, 500, 30), $"DeltaTime: {Time.deltaTime:F6}", style);
        yPos += 30;
        GUI.Label(new Rect(10, yPos, 500, 30), $"UnscaledDeltaTime: {Time.unscaledDeltaTime:F6}", style);
        yPos += 30;
        GUI.Label(new Rect(10, yPos, 500, 30), $"RealTime: {Time.realtimeSinceStartup:F2}", style);
        yPos += 30;
        
        if (_isTesting)
        {
            style.normal.textColor = Color.yellow;
            float remaining = _testDuration - (Time.realtimeSinceStartup - _startTime);
            GUI.Label(new Rect(10, yPos, 500, 30), $"测试中... 剩余: {remaining:F1}秒", style);
        }
    }
}

