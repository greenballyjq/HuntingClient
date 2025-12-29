using UnityEngine;

/// <summary>
/// Rect 结构体可视化测试脚本
/// 演示 Rect 的常用属性和方法，并提供可视化界面
/// </summary>
public class RectTest : MonoBehaviour
{
    [Header("测试矩形")]
    [SerializeField] private Rect _testRect = new Rect(100, 100, 200, 150);
    
    [Header("测试点")]
    [SerializeField] private Vector2 _testPoint = new Vector2(200, 175);
    
    [Header("第二个矩形（用于 Overlaps 测试）")]
    [SerializeField] private Rect _secondRect = new Rect(150, 150, 100, 100);
    
    [Header("坐标系说明")]
    [SerializeField] private CoordinateSystem _coordinateSystem = CoordinateSystem.Rect;
    
    public enum CoordinateSystem
    {
        Rect,       // Rect坐标系：原点在左下角，Y向上（Rect结构体默认）
        GUI         // GUI坐标系：原点在左上角，Y向下（OnGUI显示时需要转换）
    }
    
    [Header("显示设置")]
    [SerializeField] private bool _showGUI = true;
    [SerializeField] private bool _showRectBorder = true;
    [SerializeField] private bool _showRectCenter = true;
    [SerializeField] private bool _showCorners = true;
    [SerializeField] private bool _showTestPoint = true;
    [SerializeField] private bool _showSecondRect = true;
    [SerializeField] private bool _showLabels = true; // 显示属性标注
    [SerializeField] private bool _showRelationLines = true; // 显示关系线
    [SerializeField] private bool _showCoordinateInfo = true; // 显示坐标系说明
    
    [Header("颜色设置")]
    [SerializeField] private Color _rectColor = Color.cyan;
    [SerializeField] private Color _secondRectColor = Color.yellow;
    [SerializeField] private Color _pointInsideColor = Color.green;
    [SerializeField] private Color _pointOutsideColor = Color.red;
    [SerializeField] private Color _centerColor = Color.magenta;
    [SerializeField] private Color _cornerColor = Color.white;

    private Texture2D _whiteTexture;
    private bool _isDraggingRect = false;
    private bool _isDraggingPoint = false;
    private bool _isDraggingSecondRect = false;
    private Vector2 _dragOffset;

    private void Start()
    {
        // 创建白色纹理用于绘制
        _whiteTexture = new Texture2D(1, 1);
        _whiteTexture.SetPixel(0, 0, Color.white);
        _whiteTexture.Apply();
    }

    private void OnGUI()
    {
        if (!_showGUI) return;

        // 绘制矩形和点
        DrawRect(_testRect, _rectColor);
        if (_showSecondRect)
        {
            DrawRect(_secondRect, _secondRectColor);
        }
        
        // 绘制关系线和标注
        if (_showRelationLines)
        {
            DrawRelationLines();
        }
        
        if (_showRectCenter)
        {
            DrawPoint(_testRect.center, _centerColor, 8);
            if (_showLabels)
            {
                DrawLabel(_testRect.center, "center", _centerColor);
            }
        }
        
        if (_showCorners)
        {
            // 在 Rect 坐标系中：min 是左下角，max 是右上角
            Vector2 bottomLeft = new Vector2(_testRect.xMin, _testRect.yMin);  // min = (x, y)
            Vector2 bottomRight = new Vector2(_testRect.xMax, _testRect.yMin);
            Vector2 topRight = new Vector2(_testRect.xMax, _testRect.yMax);   // max = (x+width, y+height)
            Vector2 topLeft = new Vector2(_testRect.xMin, _testRect.yMax);
            
            DrawPoint(bottomLeft, _cornerColor, 6);
            DrawPoint(bottomRight, _cornerColor, 6);
            DrawPoint(topRight, _cornerColor, 6);
            DrawPoint(topLeft, _cornerColor, 6);
            
            if (_showLabels)
            {
                DrawLabel(bottomLeft, "min\n(xMin,yMin)", _cornerColor);
                DrawLabel(topRight, "max\n(xMax,yMax)", _cornerColor);
            }
        }
        
        // 绘制位置标注
        if (_showLabels)
        {
            DrawPositionLabels();
        }
        
        if (_showTestPoint)
        {
            bool contains = _testRect.Contains(_testPoint);
            DrawPoint(_testPoint, contains ? _pointInsideColor : _pointOutsideColor, 10);
        }

        // 处理鼠标拖拽
        HandleMouseDrag();

        // 显示信息面板
        DrawInfoPanel();
        
        // 显示坐标系说明
        if (_showCoordinateInfo)
        {
            DrawCoordinateInfo();
        }
    }

    private void DrawRect(Rect rect, Color color)
    {
        // 根据坐标系转换到屏幕坐标（OnGUI使用左上角原点，Y向下）
        Rect screenRect;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            // Rect坐标系：原点在左下角，Y向上
            // 转换到GUI坐标系（左上角原点，Y向下）
            screenRect = new Rect(rect.x, Screen.height - rect.y - rect.height, rect.width, rect.height);
        }
        else
        {
            // GUI坐标系：原点在左上角，Y向下（已经是GUI坐标系，直接使用）
            screenRect = rect;
        }
        
        // 绘制矩形边框
        if (_showRectBorder)
        {
            GUI.color = color;
            GUI.DrawTexture(new Rect(screenRect.x, screenRect.y, screenRect.width, 2), _whiteTexture); // 上边
            GUI.DrawTexture(new Rect(screenRect.x, screenRect.y + screenRect.height - 2, screenRect.width, 2), _whiteTexture); // 下边
            GUI.DrawTexture(new Rect(screenRect.x, screenRect.y, 2, screenRect.height), _whiteTexture); // 左边
            GUI.DrawTexture(new Rect(screenRect.x + screenRect.width - 2, screenRect.y, 2, screenRect.height), _whiteTexture); // 右边
            GUI.color = Color.white;
        }
        
        // 绘制半透明填充
        Color fillColor = color;
        fillColor.a = 0.2f;
        GUI.color = fillColor;
        GUI.DrawTexture(screenRect, _whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawPoint(Vector2 point, Color color, float size)
    {
        Rect pointRect;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            // Rect坐标系：转换到GUI坐标系
            pointRect = new Rect(
                point.x - size / 2,
                Screen.height - point.y - size / 2,
                size,
                size
            );
        }
        else
        {
            // GUI坐标系：直接使用
            pointRect = new Rect(
                point.x - size / 2,
                point.y - size / 2,
                size,
                size
            );
        }
        
        GUI.color = color;
        GUI.DrawTexture(pointRect, _whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawLabel(Vector2 point, string text, Color color)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 12;
        style.normal.textColor = color;
        style.alignment = TextAnchor.MiddleCenter;
        
        Vector2 screenPos = new Vector2(point.x, Screen.height - point.y);
        Rect labelRect = new Rect(screenPos.x - 50, screenPos.y - 30, 100, 20);
        GUI.Label(labelRect, text, style);
    }

    private void DrawRelationLines()
    {
        // 绘制从 x, y 到 xMin, yMin 的关系线
        Vector2 origin = new Vector2(_testRect.x, _testRect.y);
        Vector2 minPoint = new Vector2(_testRect.xMin, _testRect.yMin);
        
        // 绘制 x 轴标注线
        DrawLine(new Vector2(_testRect.xMin, _testRect.yMin - 20), new Vector2(_testRect.xMax, _testRect.yMin - 20), Color.yellow, 1);
        DrawLine(new Vector2(_testRect.x, _testRect.yMin - 20), new Vector2(_testRect.x, _testRect.yMin), Color.yellow, 2);
        
        // 绘制 y 轴标注线
        DrawLine(new Vector2(_testRect.xMin - 20, _testRect.yMin), new Vector2(_testRect.xMin - 20, _testRect.yMax), Color.yellow, 1);
        DrawLine(new Vector2(_testRect.xMin - 20, _testRect.y), new Vector2(_testRect.xMin, _testRect.y), Color.yellow, 2);
        
        // 绘制 width 标注
        DrawLine(new Vector2(_testRect.xMin, _testRect.yMax + 10), new Vector2(_testRect.xMax, _testRect.yMax + 10), Color.green, 1);
        DrawLine(new Vector2(_testRect.xMin, _testRect.yMax + 10), new Vector2(_testRect.xMin, _testRect.yMax + 5), Color.green, 1);
        DrawLine(new Vector2(_testRect.xMax, _testRect.yMax + 10), new Vector2(_testRect.xMax, _testRect.yMax + 5), Color.green, 1);
        
        // 绘制 height 标注
        DrawLine(new Vector2(_testRect.xMax + 10, _testRect.yMin), new Vector2(_testRect.xMax + 10, _testRect.yMax), Color.green, 1);
        DrawLine(new Vector2(_testRect.xMax + 10, _testRect.yMin), new Vector2(_testRect.xMax + 5, _testRect.yMin), Color.green, 1);
        DrawLine(new Vector2(_testRect.xMax + 10, _testRect.yMax), new Vector2(_testRect.xMax + 5, _testRect.yMax), Color.green, 1);
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Vector2 screenStart = new Vector2(start.x, Screen.height - start.y);
        Vector2 screenEnd = new Vector2(end.x, Screen.height - end.y);
        
        // 计算线条的方向和长度
        Vector2 direction = screenEnd - screenStart;
        float length = direction.magnitude;
        
        if (length < 0.01f) return;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 保存当前矩阵
        Matrix4x4 matrixBackup = GUI.matrix;
        
        // 旋转并绘制
        GUI.color = color;
        GUIUtility.RotateAroundPivot(angle, screenStart);
        GUI.DrawTexture(new Rect(screenStart.x, screenStart.y - width / 2, length, width), _whiteTexture);
        
        // 恢复矩阵
        GUI.matrix = matrixBackup;
        GUI.color = Color.white;
    }

    private void DrawPositionLabels()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 11;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleLeft;
        
        // x, y 标注
        Vector2 xyPos = new Vector2(_testRect.x, _testRect.y);
        Vector2 screenXY = new Vector2(xyPos.x + 5, Screen.height - xyPos.y - 15);
        GUI.Label(new Rect(screenXY.x, screenXY.y, 150, 20), $"x={_testRect.x:F0}, y={_testRect.y:F0}", style);
        
        // xMin, xMax 标注
        Vector2 xMinPos = new Vector2(_testRect.xMin, _testRect.yMin - 25);
        Vector2 screenXMin = new Vector2(xMinPos.x, Screen.height - xMinPos.y);
        GUI.Label(new Rect(screenXMin.x - 30, screenXMin.y, 60, 20), $"xMin={_testRect.xMin:F0}", style);
        
        Vector2 xMaxPos = new Vector2(_testRect.xMax, _testRect.yMin - 25);
        Vector2 screenXMax = new Vector2(xMaxPos.x, Screen.height - xMaxPos.y);
        GUI.Label(new Rect(screenXMax.x - 30, screenXMax.y, 60, 20), $"xMax={_testRect.xMax:F0}", style);
        
        // yMin, yMax 标注
        Vector2 yMinPos = new Vector2(_testRect.xMin - 25, _testRect.yMin);
        Vector2 screenYMin = new Vector2(yMinPos.x, Screen.height - yMinPos.y);
        GUI.Label(new Rect(screenYMin.x - 50, screenYMin.y - 10, 50, 20), $"yMin={_testRect.yMin:F0}", style);
        
        Vector2 yMaxPos = new Vector2(_testRect.xMin - 25, _testRect.yMax);
        Vector2 screenYMax = new Vector2(yMaxPos.x, Screen.height - yMaxPos.y);
        GUI.Label(new Rect(screenYMax.x - 50, screenYMax.y - 10, 50, 20), $"yMax={_testRect.yMax:F0}", style);
        
        // width, height 标注
        Vector2 widthPos = new Vector2(_testRect.center.x, _testRect.yMax + 15);
        Vector2 screenWidth = new Vector2(widthPos.x, Screen.height - widthPos.y);
        GUI.Label(new Rect(screenWidth.x - 40, screenWidth.y, 80, 20), $"width={_testRect.width:F0}", style);
        
        Vector2 heightPos = new Vector2(_testRect.xMax + 15, _testRect.center.y);
        Vector2 screenHeight = new Vector2(heightPos.x, Screen.height - heightPos.y);
        GUI.Label(new Rect(screenHeight.x, screenHeight.y - 10, 70, 20), $"height={_testRect.height:F0}", style);
    }

    private void HandleMouseDrag()
    {
        Event e = Event.current;
        Vector2 mousePos;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            // Rect坐标系：转换鼠标位置
            mousePos = new Vector2(e.mousePosition.x, Screen.height - e.mousePosition.y);
        }
        else
        {
            // GUI坐标系：直接使用
            mousePos = e.mousePosition;
        }

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // 检查是否点击了测试矩形
            if (_testRect.Contains(mousePos))
            {
                _isDraggingRect = true;
                _dragOffset = mousePos - new Vector2(_testRect.x, _testRect.y);
            }
            // 检查是否点击了测试点
            else if (Vector2.Distance(mousePos, _testPoint) < 15)
            {
                _isDraggingPoint = true;
                _dragOffset = mousePos - _testPoint;
            }
            // 检查是否点击了第二个矩形
            else if (_showSecondRect && _secondRect.Contains(mousePos))
            {
                _isDraggingSecondRect = true;
                _dragOffset = mousePos - new Vector2(_secondRect.x, _secondRect.y);
            }
        }
        else if (e.type == EventType.MouseDrag && e.button == 0)
        {
            if (_isDraggingRect)
            {
                _testRect.x = mousePos.x - _dragOffset.x;
                _testRect.y = mousePos.y - _dragOffset.y;
            }
            else if (_isDraggingPoint)
            {
                _testPoint = mousePos - _dragOffset;
            }
            else if (_isDraggingSecondRect)
            {
                _secondRect.x = mousePos.x - _dragOffset.x;
                _secondRect.y = mousePos.y - _dragOffset.y;
            }
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            _isDraggingRect = false;
            _isDraggingPoint = false;
            _isDraggingSecondRect = false;
        }
    }

    private void DrawInfoPanel()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.7f));
        style.padding = new RectOffset(10, 10, 10, 10);

        float panelWidth = 400;
        float panelHeight = 600;
        Rect panelRect = new Rect(Screen.width - panelWidth - 10, 10, panelWidth, panelHeight);
        
        GUI.Box(panelRect, "", style);
        
        float yPos = 20;
        float lineHeight = 22;
        
        // 标题
        GUIStyle titleStyle = new GUIStyle(style);
        titleStyle.fontSize = 18;
        titleStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, 25), "=== Rect 属性测试 ===", titleStyle);
        yPos += 30;

        // 测试矩形信息 - 详细说明
        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【位置和大小】", style);
        yPos += lineHeight;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"x (左边缘): {_testRect.x:F1}  ← 矩形左下角的 X 坐标", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"y (下边缘): {_testRect.y:F1}  ← 矩形左下角的 Y 坐标", style);
        }
        else
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"x (左边缘): {_testRect.x:F1}  ← 矩形左上角的 X 坐标", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"y (上边缘): {_testRect.y:F1}  ← 矩形左上角的 Y 坐标", style);
        }
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"width: {_testRect.width:F1}  ← 矩形宽度", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"height: {_testRect.height:F1}  ← 矩形高度", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"position: ({_testRect.position.x:F1}, {_testRect.position.y:F1})  ← 等于 (x, y)", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"size: ({_testRect.size.x:F1}, {_testRect.size.y:F1})  ← 等于 (width, height)", style);
        yPos += 20;

        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【边界】", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"xMin: {_testRect.xMin:F1}  ← 等于 x (左边缘)", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"xMax: {_testRect.xMax:F1}  ← 等于 x + width (右边缘)", style);
        yPos += lineHeight;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"yMin: {_testRect.yMin:F1}  ← 等于 y (下边缘)", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"yMax: {_testRect.yMax:F1}  ← 等于 y + height (上边缘)", style);
        }
        else
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"yMin: {_testRect.yMin:F1}  ← 等于 y (上边缘)", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"yMax: {_testRect.yMax:F1}  ← 等于 y + height (下边缘)", style);
        }
        yPos += 20;

        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【中心和角点】", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"center: ({_testRect.center.x:F1}, {_testRect.center.y:F1})  ← (x+width/2, y+height/2)", style);
        yPos += lineHeight;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"min: ({_testRect.min.x:F1}, {_testRect.min.y:F1})  ← 等于 (xMin, yMin) = (x, y) [左下角]", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"max: ({_testRect.max.x:F1}, {_testRect.max.y:F1})  ← 等于 (xMax, yMax) = (x+width, y+height) [右上角]", style);
        }
        else
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"min: ({_testRect.min.x:F1}, {_testRect.min.y:F1})  ← 等于 (xMin, yMin) = (x, y) [左上角]", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"max: ({_testRect.max.x:F1}, {_testRect.max.y:F1})  ← 等于 (xMax, yMax) = (x+width, y+height) [右下角]", style);
        }
        yPos += 20;

        // 数学关系说明
        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【数学关系】", style);
        yPos += lineHeight;
        style.fontSize = 11;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• xMin = x", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• xMax = x + width", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• yMin = y", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• yMax = y + height", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• position = (x, y)", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• size = (width, height)", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• center = (x + width/2, y + height/2)", style);
        yPos += lineHeight;
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• min = (xMin, yMin) = (x, y) [左下角]", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• max = (xMax, yMax) = (x+width, y+height) [右上角]", style);
        }
        else
        {
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• min = (xMin, yMin) = (x, y) [左上角]", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• max = (xMax, yMax) = (x+width, y+height) [右下角]", style);
        }
        style.fontSize = 14;
        yPos += 20;

        // Contains 测试
        bool contains = _testRect.Contains(_testPoint);
        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【Contains 测试】", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"测试点: ({_testPoint.x:F1}, {_testPoint.y:F1})", style);
        yPos += lineHeight;
        style.normal.textColor = contains ? Color.green : Color.red;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"结果: {(contains ? "✓ 点在矩形内" : "✗ 点在矩形外")}", style);
        style.normal.textColor = Color.white;
        yPos += 20;

        // Overlaps 测试
        if (_showSecondRect)
        {
            bool overlaps = _testRect.Overlaps(_secondRect);
            GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【Overlaps 测试】", style);
            yPos += lineHeight;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"第二个矩形: ({_secondRect.x:F1}, {_secondRect.y:F1}, {_secondRect.width:F1}, {_secondRect.height:F1})", style);
            yPos += lineHeight;
            style.normal.textColor = overlaps ? Color.yellow : Color.white;
            GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), $"结果: {(overlaps ? "✓ 矩形重叠" : "✗ 矩形不重叠")}", style);
            style.normal.textColor = Color.white;
            yPos += 20;
        }

        // 操作提示
        GUI.Label(new Rect(panelRect.x + 10, yPos, panelWidth, lineHeight), "【操作提示】", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• 点击并拖拽矩形可移动", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• 点击并拖拽点可移动测试点", style);
        yPos += lineHeight;
        GUI.Label(new Rect(panelRect.x + 20, yPos, panelWidth, lineHeight), "• 在 Inspector 中可调整参数", style);
    }

    private void DrawCoordinateInfo()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 13;
        style.normal.textColor = Color.yellow;
        style.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.8f));
        style.padding = new RectOffset(10, 10, 10, 10);
        style.alignment = TextAnchor.UpperLeft;

        float panelWidth = 500;
        float panelHeight = 180;
        Rect infoRect = new Rect(10, Screen.height - panelHeight - 10, panelWidth, panelHeight);
        
        GUI.Box(infoRect, "", style);
        
        float yPos = infoRect.y + 15;
        float lineHeight = 20;
        
        GUIStyle titleStyle = new GUIStyle(style);
        titleStyle.fontSize = 16;
        titleStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(infoRect.x + 10, yPos, panelWidth, 25), "⚠️ 重要：Rect 的坐标系说明", titleStyle);
        yPos += 25;
        
        style.fontSize = 12;
        style.normal.textColor = Color.white;
        
        if (_coordinateSystem == CoordinateSystem.Rect)
        {
            GUI.Label(new Rect(infoRect.x + 10, yPos, panelWidth, lineHeight), "当前：Rect 坐标系（Rect 结构体默认）", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• 原点在左下角 (0, 0) ← 这是 Rect 的默认坐标系！", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• Y 轴向上（Y 值越大，位置越靠上）", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• x, y 表示左下角坐标", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• 使用场景：Camera.rect, 2D碰撞检测, 纹理坐标等", style);
        }
        else
        {
            GUI.Label(new Rect(infoRect.x + 10, yPos, panelWidth, lineHeight), "当前：GUI 坐标系（OnGUI 显示用）", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• 原点在左上角 (0, 0)", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• Y 轴向下（Y 值越大，位置越靠下）", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• x, y 表示左上角坐标", style);
            yPos += lineHeight;
            GUI.Label(new Rect(infoRect.x + 20, yPos, panelWidth, lineHeight), "• 使用场景：GUI.Button(), GUI.Label() 等", style);
        }
        
        yPos += lineHeight;
        style.normal.textColor = Color.cyan;
        GUI.Label(new Rect(infoRect.x + 10, yPos, panelWidth, lineHeight), "💡 Rect 本身只是数据结构，坐标系取决于使用它的上下文！", style);
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

    private void Update()
    {
        // 实时更新鼠标位置到测试点（按住空格键）
        if (Input.GetKey(KeyCode.Space))
        {
            Vector2 mousePos = Input.mousePosition;
            _testPoint = new Vector2(mousePos.x, Screen.height - mousePos.y);
        }
    }

    #region Context Menu 方法
    [ContextMenu("测试 Contains")]
    private void TestContains()
    {
        bool result = _testRect.Contains(_testPoint);
        Debug.Log($"[RectTest] 点 {_testPoint} 在矩形 {_testRect} 内: {result}");
    }

    [ContextMenu("测试 Overlaps")]
    private void TestOverlaps()
    {
        bool result = _testRect.Overlaps(_secondRect);
        Debug.Log($"[RectTest] 矩形 {_testRect} 与 {_secondRect} 重叠: {result}");
    }

    [ContextMenu("显示所有属性")]
    private void ShowAllProperties()
    {
        Debug.Log($"=== Rect 所有属性 ===");
        Debug.Log($"x: {_testRect.x}, y: {_testRect.y}");
        Debug.Log($"width: {_testRect.width}, height: {_testRect.height}");
        Debug.Log($"position: {_testRect.position}");
        Debug.Log($"size: {_testRect.size}");
        Debug.Log($"xMin: {_testRect.xMin}, xMax: {_testRect.xMax}");
        Debug.Log($"yMin: {_testRect.yMin}, yMax: {_testRect.yMax}");
        Debug.Log($"center: {_testRect.center}");
        Debug.Log($"min: {_testRect.min}");
        Debug.Log($"max: {_testRect.max}");
    }

    [ContextMenu("重置为默认值")]
    private void ResetToDefault()
    {
        _testRect = new Rect(100, 100, 200, 150);
        _testPoint = new Vector2(200, 175);
        _secondRect = new Rect(150, 150, 100, 100);
    }
    #endregion

    private void OnDestroy()
    {
        if (_whiteTexture != null)
        {
            Destroy(_whiteTexture);
        }
    }
}

