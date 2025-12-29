using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Screen 类可视化测试脚本
/// 展示 Screen 类的所有属性和方法，并提供可视化界面和交互测试
/// </summary>
public class ScreenTest : MonoBehaviour
{
    [Header("显示设置")]
    [SerializeField] private bool _showGUI = true;
    [SerializeField] private bool _autoRefresh = true;
    [SerializeField] private float _refreshInterval = 0.5f;
    
    [Header("测试设置")]
    [SerializeField] private bool _showSafeArea = true;
    [SerializeField] private bool _showCutouts = true;
    [SerializeField] private Color _safeAreaColor = new Color(0, 1, 0, 0.3f);
    [SerializeField] private Color _cutoutColor = new Color(1, 0, 0, 0.5f);
    
    [Header("分辨率测试")]
    [SerializeField] private int _selectedResolutionIndex = -1;
    [SerializeField] private bool _showResolutionList = true;
    
    [Header("全屏测试")]
    [SerializeField] private FullScreenMode _testFullScreenMode = FullScreenMode.FullScreenWindow;
    
    private Texture2D _whiteTexture;
    private float _lastRefreshTime;
    private Resolution[] _resolutions;
    private Vector2 _scrollPosition;
    private bool _isChangingResolution = false;

    private void Start()
    {
        // 创建白色纹理用于绘制
        _whiteTexture = new Texture2D(1, 1);
        _whiteTexture.SetPixel(0, 0, Color.white);
        _whiteTexture.Apply();
        
        // 获取所有分辨率
        RefreshResolutions();
        
        // 找到当前分辨率索引
        FindCurrentResolutionIndex();
    }

    private void Update()
    {
        if (_autoRefresh && Time.time - _lastRefreshTime > _refreshInterval)
        {
            RefreshResolutions();
            FindCurrentResolutionIndex();
            _lastRefreshTime = Time.time;
        }
    }

    private void RefreshResolutions()
    {
        _resolutions = Screen.resolutions;
    }

    private void FindCurrentResolutionIndex()
    {
        if (_resolutions == null || _resolutions.Length == 0) return;
        
        Resolution current = Screen.currentResolution;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].width == current.width &&
                _resolutions[i].height == current.height &&
                _resolutions[i].refreshRate == current.refreshRate)
            {
                _selectedResolutionIndex = i;
                return;
            }
        }
    }

    private void OnGUI()
    {
        if (!_showGUI) return;

        // 绘制安全区域和缺口
        if (_showSafeArea)
        {
            DrawSafeArea();
        }
        
        if (_showCutouts)
        {
            DrawCutouts();
        }

        // 显示信息面板
        DrawInfoPanel();
    }

    private void DrawSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        
        // 转换到GUI坐标系（左上角原点）
        Rect guiSafeArea = new Rect(
            safeArea.x,
            Screen.height - safeArea.y - safeArea.height,
            safeArea.width,
            safeArea.height
        );
        
        // 绘制安全区域
        GUI.color = _safeAreaColor;
        GUI.DrawTexture(guiSafeArea, _whiteTexture);
        GUI.color = Color.white;
        
        // 绘制边框
        DrawRectBorder(guiSafeArea, Color.green, 2);
    }

    private void DrawCutouts()
    {
        Rect[] cutouts = Screen.cutouts;
        if (cutouts == null || cutouts.Length == 0) return;
        
        foreach (Rect cutout in cutouts)
        {
            // 转换到GUI坐标系
            Rect guiCutout = new Rect(
                cutout.x,
                Screen.height - cutout.y - cutout.height,
                cutout.width,
                cutout.height
            );
            
            GUI.color = _cutoutColor;
            GUI.DrawTexture(guiCutout, _whiteTexture);
            GUI.color = Color.white;
            
            // 绘制边框
            DrawRectBorder(guiCutout, Color.red, 2);
        }
    }

    private void DrawRectBorder(Rect rect, Color color, float thickness)
    {
        GUI.color = color;
        // 上边
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), _whiteTexture);
        // 下边
        GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), _whiteTexture);
        // 左边
        GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), _whiteTexture);
        // 右边
        GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), _whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawInfoPanel()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        style.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.85f));
        style.padding = new RectOffset(20, 20, 20, 20);

        float panelWidth = 580;
        float panelHeight = Screen.height - 20;
        Rect panelRect = new Rect(10, 10, panelWidth, panelHeight);
        
        GUI.Box(panelRect, "", style);
        
        float yPos = 400;
        float lineHeight = 1280;
        float scrollViewHeight = panelHeight - 800;
        
        // 标题
        GUIStyle titleStyle = new GUIStyle(style);
        titleStyle.fontSize = 22;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.cyan;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, 400), "=== Screen 类测试 ===", titleStyle);
        yPos += 2000;

        // 滚动视图
        _scrollPosition = GUI.BeginScrollView(
            new Rect(panelRect.x + 15, yPos, panelWidth - 30, scrollViewHeight),
            _scrollPosition,
            new Rect(0, 0, panelWidth - 50, 4000)
        );
        
        float contentY = 0;

        // 基本属性
        contentY = DrawSection("【基本属性】", contentY, lineHeight, style, () => {
            DrawProperty("Screen.width", Screen.width.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("Screen.height", Screen.height.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("Screen.dpi", Screen.dpi > 0 ? Screen.dpi.ToString("F2") : "不支持", contentY, lineHeight, style);
            contentY += lineHeight + 200;
        });

        // 当前分辨率
        contentY = DrawSection("【当前分辨率】", contentY, lineHeight, style, () => {
            Resolution current = Screen.currentResolution;
            DrawProperty("width", current.width.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("height", current.height.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("refreshRate", current.refreshRate.ToString() + " Hz", contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("width (像素)", current.width.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
        });

        // 全屏设置
        contentY = DrawSection("【全屏设置】", contentY, lineHeight, style, () => {
            DrawProperty("Screen.fullScreen", Screen.fullScreen.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("Screen.fullScreenMode", Screen.fullScreenMode.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            
            // 全屏模式选择
            contentY += 600;
            GUI.Label(new Rect(10, contentY, 280, lineHeight), "测试全屏模式:", style);
            contentY += lineHeight + 400;
            
            if (GUI.Button(new Rect(10, contentY, 180, 1600), "全屏窗口"))
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 180, 1600), "独占全屏"))
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 180, 1600), "窗口模式"))
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 180, 1600), "最大化窗口"))
            {
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            }
            contentY += 600;
        });

        // 屏幕方向
        contentY = DrawSection("【屏幕方向】", contentY, lineHeight, style, () => {
            DrawProperty("Screen.orientation", Screen.orientation.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            
            // 方向控制按钮
            contentY += 600;
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "横屏"))
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "竖屏"))
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "自动旋转"))
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            contentY += 600;
        });

        // 光标设置
        contentY = DrawSection("【光标设置】", contentY, lineHeight, style, () => {
            DrawProperty("Screen.showCursor", Cursor.visible.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("Cursor.lockState", Cursor.lockState.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("Cursor.visible", Cursor.visible.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            
            // 光标控制按钮
            contentY += 600;
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "显示光标"))
            {
                Cursor.visible = true;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "隐藏光标"))
            {
                Cursor.visible = false;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "锁定光标"))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 160, 1600), "解锁光标"))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            contentY += 600;
        });

        // 其他设置
        contentY = DrawSection("【其他设置】", contentY, lineHeight, style, () => {
            DrawProperty("Screen.sleepTimeout", Screen.sleepTimeout == SleepTimeout.NeverSleep ? 
                "NeverSleep" : Screen.sleepTimeout.ToString(), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            
            DrawProperty("Screen.brightness", Screen.brightness.ToString("F2"), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            
            // 睡眠超时控制
            contentY += 600;
            if (GUI.Button(new Rect(10, contentY, 180, 1600), "永不睡眠"))
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
            contentY += 2000;
            
            if (GUI.Button(new Rect(10, contentY, 180, 1600), "系统默认"))
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            contentY += 600;
        });

        // 安全区域
        contentY = DrawSection("【安全区域】", contentY, lineHeight, style, () => {
            Rect safeArea = Screen.safeArea;
            DrawProperty("x", safeArea.x.ToString("F1"), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("y", safeArea.y.ToString("F1"), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("width", safeArea.width.ToString("F1"), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("height", safeArea.height.ToString("F1"), contentY, lineHeight, style);
            contentY += lineHeight + 200;
            DrawProperty("center", $"({safeArea.center.x:F1}, {safeArea.center.y:F1})", contentY, lineHeight, style);
            contentY += lineHeight + 200;
        });

        // 屏幕缺口
        contentY = DrawSection("【屏幕缺口 (Cutouts)】", contentY, lineHeight, style, () => {
            Rect[] cutouts = Screen.cutouts;
            if (cutouts == null || cutouts.Length == 0)
            {
                DrawProperty("数量", "0 (无缺口)", contentY, lineHeight, style);
                contentY += lineHeight;
            }
            else
            {
                DrawProperty("数量", cutouts.Length.ToString(), contentY, lineHeight, style);
                contentY += lineHeight;
                
                for (int i = 0; i < cutouts.Length; i++)
                {
                    Rect cutout = cutouts[i];
                    GUI.Label(new Rect(25, contentY, 500, lineHeight), 
                        $"  [{i}] ({cutout.x:F0}, {cutout.y:F0}, {cutout.width:F0}, {cutout.height:F0})", style);
                    contentY += lineHeight + 320;
                }
            }
            contentY += lineHeight;
        });

        // 支持的分辨率列表
        if (_showResolutionList)
        {
            contentY = DrawSection("【支持的分辨率】", contentY, lineHeight, style, () => {
                if (_resolutions == null || _resolutions.Length == 0)
                {
                    DrawProperty("数量", "0", contentY, lineHeight, style);
                    contentY += lineHeight;
                }
                else
                {
                    DrawProperty("总数", _resolutions.Length.ToString(), contentY, lineHeight, style);
                    contentY += lineHeight;
                    
                    // 显示分辨率列表（最多20个）
                    int displayCount = Mathf.Min(_resolutions.Length, 20);
                    for (int i = 0; i < displayCount; i++)
                    {
                        Resolution res = _resolutions[i];
                        bool isCurrent = i == _selectedResolutionIndex;
                        Color textColor = isCurrent ? Color.green : Color.white;
                        
                        GUIStyle resStyle = new GUIStyle(style);
                        resStyle.normal.textColor = textColor;
                        
                        string marker = isCurrent ? " ← 当前" : "";
                        GUI.Label(new Rect(25, contentY, 500, lineHeight), 
                            $"[{i}] {res.width}x{res.height} @ {res.refreshRate}Hz{marker}", resStyle);
                        contentY += lineHeight + 320;
                    }
                    
                    if (_resolutions.Length > 20)
                    {
                        GUI.Label(new Rect(25, contentY, 500, lineHeight), 
                            $"... 还有 {_resolutions.Length - 20} 个分辨率", style);
                        contentY += lineHeight + 400;
                    }
                    
                    // 分辨率切换按钮
                    contentY += 800;
                    if (_selectedResolutionIndex >= 0 && _selectedResolutionIndex < _resolutions.Length)
                    {
                        Resolution currentRes = _resolutions[_selectedResolutionIndex];
                        GUI.Label(new Rect(10, contentY, 400, lineHeight), 
                            $"当前: {currentRes.width}x{currentRes.height} @ {currentRes.refreshRate}Hz", style);
                        contentY += lineHeight + 600;
                        
                        if (GUI.Button(new Rect(10, contentY, 200, 1600), "切换到下一个"))
                        {
                            int nextIndex = (_selectedResolutionIndex + 1) % _resolutions.Length;
                            SetResolution(_resolutions[nextIndex]);
                        }
                        contentY += 600;
                    }
                }
                contentY += lineHeight;
            });
        }

        GUI.EndScrollView();
    }

    private float DrawSection(string title, float yPos, float lineHeight, GUIStyle style, System.Action drawContent)
    {
        GUIStyle titleStyle = new GUIStyle(style);
        titleStyle.fontSize = 18;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.cyan;
        
        GUI.Label(new Rect(10, yPos, 500, lineHeight + 400), title, titleStyle);
        yPos += lineHeight + 720;
        
        drawContent?.Invoke();
        yPos += 1000;
        
        return yPos;
    }

    private void DrawProperty(string name, string value, float yPos, float lineHeight, GUIStyle style)
    {
        GUI.Label(new Rect(25, yPos, 240, lineHeight), name + ":", style);
        GUIStyle valueStyle = new GUIStyle(style);
        valueStyle.normal.textColor = Color.yellow;
        GUI.Label(new Rect(220, yPos, 350, lineHeight), value, valueStyle);
    }

    private void SetResolution(Resolution resolution)
    {
        if (_isChangingResolution) return;
        
        _isChangingResolution = true;
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);
        
        // 延迟重置标志
        Invoke(nameof(ResetResolutionFlag), 1f);
    }

    private void ResetResolutionFlag()
    {
        _isChangingResolution = false;
        RefreshResolutions();
        FindCurrentResolutionIndex();
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

    private void OnDestroy()
    {
        if (_whiteTexture != null)
        {
            Destroy(_whiteTexture);
        }
    }

    #region Context Menu 方法
    [ContextMenu("打印所有 Screen 属性")]
    private void LogAllScreenProperties()
    {
        Debug.Log("=== Screen 类所有属性 ===");
        Debug.Log($"Screen.width: {Screen.width}");
        Debug.Log($"Screen.height: {Screen.height}");
        Debug.Log($"Screen.dpi: {Screen.dpi}");
        Debug.Log($"Screen.fullScreen: {Screen.fullScreen}");
        Debug.Log($"Screen.fullScreenMode: {Screen.fullScreenMode}");
        Debug.Log($"Screen.orientation: {Screen.orientation}");
        Debug.Log($"Screen.sleepTimeout: {Screen.sleepTimeout}");
        Debug.Log($"Screen.brightness: {Screen.brightness}");
        Debug.Log($"Screen.showCursor: {Cursor.visible}");
        
        Resolution current = Screen.currentResolution;
        Debug.Log($"Screen.currentResolution: {current.width}x{current.height} @ {current.refreshRate}Hz");
        
        Rect safeArea = Screen.safeArea;
        Debug.Log($"Screen.safeArea: ({safeArea.x}, {safeArea.y}, {safeArea.width}, {safeArea.height})");
        
        Rect[] cutouts = Screen.cutouts;
        Debug.Log($"Screen.cutouts: {cutouts?.Length ?? 0} 个");
        
        Resolution[] resolutions = Screen.resolutions;
        Debug.Log($"Screen.resolutions: {resolutions?.Length ?? 0} 个");
    }

    [ContextMenu("切换到下一个分辨率")]
    private void SwitchToNextResolution()
    {
        if (_resolutions == null || _resolutions.Length == 0)
        {
            RefreshResolutions();
        }
        
        if (_resolutions != null && _resolutions.Length > 0)
        {
            int nextIndex = (_selectedResolutionIndex + 1) % _resolutions.Length;
            SetResolution(_resolutions[nextIndex]);
        }
    }

    [ContextMenu("切换全屏模式")]
    private void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    #endregion
}

