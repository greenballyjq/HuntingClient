using UnityEngine;

/// <summary>
/// Cursor 类测试脚本
/// 
/// 【概念解释】
/// 1. 光标图片：你想显示的自定义光标图片（比如箭头、手型等）
/// 2. 热点位置：光标图片上哪个点是"点击点"
///    - 比如箭头图片，箭头尖尖的位置就是热点
///    - 如果图片是 32x32 像素，中心点就是 (16, 16)
///    - 如果图片是 64x64 像素，中心点就是 (32, 32)
///    - (0, 0) 是图片左上角
/// 
/// 【使用方法】
/// 1. 在 Inspector 中设置参数
/// 2. 右键脚本 → 选择"应用光标设置"
/// </summary>
public class CursorTest : MonoBehaviour
{
    [Header("光标锁定模式")]
    [Tooltip("None=自由移动, Locked=锁定在屏幕中心, Confined=限制在游戏窗口内")]
    [SerializeField] private CursorLockMode _lockMode = CursorLockMode.None;
    
    [Header("光标可见性")]
    [Tooltip("是否显示光标")]
    [SerializeField] private bool _cursorVisible = true;
    
    [Header("光标图片设置")]
    [Tooltip("拖入你想使用的光标图片（Texture2D），如果不设置就使用系统默认光标")]
    [SerializeField] private Texture2D _cursorTexture;
    
    [Tooltip("热点位置：光标图片上哪个点是点击点。比如图片中心就是 (宽度/2, 高度/2)")]
    [SerializeField] private Vector2 _hotspot = Vector2.zero;
    
    [Tooltip("光标模式：Auto=自动选择最佳模式, ForceSoftware=强制软件渲染")]
    [SerializeField] private CursorMode _cursorMode = CursorMode.Auto;

    private void Start()
    {
        // 启动时自动应用设置
        ApplyCursorSettings();
    }

    /// <summary>
    /// 应用所有光标设置
    /// </summary>
    [ContextMenu("应用光标设置")]
    public void ApplyCursorSettings()
    {
        // 设置锁定模式
        Cursor.lockState = _lockMode;
        
        // 设置可见性
        Cursor.visible = _cursorVisible;
        
        // 设置光标图片（如果有的话）
        if (_cursorTexture != null)
        {
            Cursor.SetCursor(_cursorTexture, _hotspot, _cursorMode);
            Debug.Log($"[CursorTest] ✓ 已设置光标图片: {_cursorTexture.name}");
            Debug.Log($"[CursorTest]   热点位置: ({_hotspot.x}, {_hotspot.y})");
        }
        else
        {
            Debug.Log("[CursorTest] 使用系统默认光标（未设置自定义图片）");
        }
        
        Debug.Log($"[CursorTest] 锁定模式: {_lockMode}, 可见性: {_cursorVisible}");
    }

    /// <summary>
    /// 设置锁定模式：None（自由移动）
    /// </summary>
    [ContextMenu("锁定模式: None（自由移动）")]
    public void SetLockModeNone()
    {
        _lockMode = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("[CursorTest] 锁定模式: None（光标可以自由移动）");
    }

    /// <summary>
    /// 设置锁定模式：Locked（锁定在屏幕中心）
    /// </summary>
    [ContextMenu("锁定模式: Locked（锁定在屏幕中心）")]
    public void SetLockModeLocked()
    {
        _lockMode = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("[CursorTest] 锁定模式: Locked（光标锁定在屏幕中心，适合第一人称游戏）");
    }

    /// <summary>
    /// 设置锁定模式：Confined（限制在游戏窗口内）
    /// </summary>
    [ContextMenu("锁定模式: Confined（限制在窗口内）")]
    public void SetLockModeConfined()
    {
        _lockMode = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("[CursorTest] 锁定模式: Confined（光标限制在游戏窗口内）");
    }

    /// <summary>
    /// 显示光标
    /// </summary>
    [ContextMenu("显示光标")]
    public void ShowCursor()
    {
        _cursorVisible = true;
        Cursor.visible = true;
        Debug.Log("[CursorTest] 光标已显示");
    }

    /// <summary>
    /// 隐藏光标
    /// </summary>
    [ContextMenu("隐藏光标")]
    public void HideCursor()
    {
        _cursorVisible = false;
        Cursor.visible = false;
        Debug.Log("[CursorTest] 光标已隐藏");
    }

    /// <summary>
    /// 重置为系统默认光标
    /// </summary>
    [ContextMenu("重置为系统默认光标")]
    public void ResetToDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _cursorTexture = null;
        Debug.Log("[CursorTest] 已重置为系统默认光标");
    }

    /// <summary>
    /// 打印当前光标状态
    /// </summary>
    [ContextMenu("打印当前光标状态")]
    public void LogCurrentCursorState()
    {
        Debug.Log("=== Cursor 当前状态 ===");
        Debug.Log($"锁定模式: {Cursor.lockState}");
        Debug.Log($"可见性: {Cursor.visible}");
        Debug.Log($"光标图片: {(_cursorTexture != null ? _cursorTexture.name : "系统默认")}");
        Debug.Log($"热点位置: ({_hotspot.x}, {_hotspot.y})");
    }

    /// <summary>
    /// 在 Inspector 中值改变时自动应用（仅运行时）
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            // 只在运行时自动应用锁定模式和可见性
            Cursor.lockState = _lockMode;
            Cursor.visible = _cursorVisible;
        }
    }
}
