using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 测试UI管理器 - 管理测试UI界面（不依赖框架）
/// </summary>
public class TestUIManager : MonoBehaviour
{
    /// <summary>
    /// UI界面层级
    /// </summary>
    public enum UILayer
    {
        Background, // 背景层
        Normal,     // 普通层
        Fixed,      // 固定层
        PopUp,      // 弹窗层
        Guide,      // 引导层
        Tips,       // 提示层
        Loading,    // 加载层
        Game        // 游戏层
    }

    /// <summary>
    /// 单例实例
    /// </summary>
    public static TestUIManager Instance { get; private set; }

    /// <summary>
    /// Resources 加载前缀，例如 \"Lab/UI/\"，最终路径 = 前缀 + uiName
    /// </summary>
    [SerializeField] private string _uiResourcePathPrefix = "Lab/";

    /// <summary>
    /// UI根节点
    /// </summary>
    private GameObject _uiRoot;

    /// <summary>
    /// 各层级的父节点
    /// </summary>
    private readonly Dictionary<UILayer, Transform> _layerParents = new Dictionary<UILayer, Transform>();

    /// <summary>
    /// 所有已打开的界面
    /// </summary>
    private readonly Dictionary<string, TestUIBase> _openedUIs = new Dictionary<string, TestUIBase>();

    /// <summary>
    /// UI摄像机
    /// </summary>
    private Camera _uiCamera;

    /// <summary>
    /// UI事件系统
    /// </summary>
    private EventSystem _uiEventSystem;

    /// <summary>
    /// UI摄像机
    /// </summary>
    public Camera UICamera => _uiCamera;

    /// <summary>
    /// UI事件系统
    /// </summary>
    public EventSystem UIEventSystem => _uiEventSystem;

    /// <summary>
    /// UI根节点
    /// </summary>
    public GameObject UIRoot => _uiRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateUIRoot();
    }

    private void Update()
    {
        // 更新所有已打开的界面
        foreach (var ui in _openedUIs.Values)
        {
            ui.OnUpdate();
        }
    }

    #region 私有方法
    /// <summary>
    /// 创建UI根节点
    /// </summary>
    private void CreateUIRoot()
    {
        // 创建UI根节点
        _uiRoot = new GameObject("UIRoot");
        int uiLayer = LayerMask.NameToLayer("UI");
        _uiRoot.layer = uiLayer;
        _uiRoot.transform.SetParent(transform);
        _uiRoot.transform.localPosition = Vector3.zero;
        DontDestroyOnLoad(_uiRoot);

        // 创建UI摄像机
        GameObject uiCameraGameObject = new GameObject("UICamera");
        uiCameraGameObject.transform.SetParent(_uiRoot.transform);
        uiCameraGameObject.transform.localPosition = new Vector3(0, 0, -120);
        _uiCamera = uiCameraGameObject.AddComponent<Camera>();
        _uiCamera.orthographic = true;
        LayerMask uiMask = LayerMask.GetMask("UI");
        _uiCamera.cullingMask = uiMask;
        _uiCamera.clearFlags = CameraClearFlags.Depth;

        // 创建EventSystem
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.transform.SetParent(_uiRoot.transform);
        _uiEventSystem = eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();

        // 添加Canvas和CanvasScaler组件
        var canvas = _uiRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = _uiCamera;
        canvas.sortingOrder = 100;

        var scaler = _uiRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.65f;

        _uiRoot.AddComponent<GraphicRaycaster>();
        _uiRoot.transform.position = Vector3.zero;

        // 创建各层级父节点
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            var layerObj = new GameObject(layer.ToString());
            layerObj.transform.SetParent(_uiRoot.transform, false);
            layerObj.layer = uiLayer;

            // 添加RectTransform并设置为全屏
            var rectTransform = layerObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            // 添加Canvas组件，用于控制层级顺序
            var layerCanvas = layerObj.AddComponent<Canvas>();
            layerCanvas.overrideSorting = true;
            layerCanvas.sortingOrder = 100 + (int)layer * 10;

            // 添加GraphicRaycaster组件
            layerObj.AddComponent<GraphicRaycaster>();

            _layerParents.Add(layer, layerObj.transform);
        }
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 打开UI界面
    /// </summary>
    public T OpenUI<T>(string uiName, UILayer layer = UILayer.Normal, object userData = null) where T : TestUIBase
    {
        // 检查界面是否已经打开
        if (_openedUIs.TryGetValue(uiName, out var existingUI))
        {
            Debug.Log($"[TestUIManager] UI界面 {uiName} 已经打开");
            return existingUI as T;
        }

        // 从 Resources 中加载预制体
        string path = string.IsNullOrEmpty(_uiResourcePathPrefix)
            ? uiName
            : _uiResourcePathPrefix + uiName;

        GameObject uiPrefab = Resources.Load<GameObject>(path);

        if (uiPrefab == null)
        {
            Debug.LogError($"[TestUIManager] 未在 Resources 中找到 UI 预制体，路径: {path}");
            return null;
        }

        // 获取层级父节点
        if (!_layerParents.TryGetValue(layer, out var layerParent))
        {
            Debug.LogError($"[TestUIManager] 无效的UI层级: {layer}");
            return null;
        }

        // 实例化界面
        var uiObj = Instantiate(uiPrefab, layerParent);
        uiObj.name = uiName;

        // 获取TestUIBase组件
        var uiComponent = uiObj.GetComponent<T>();
        if (uiComponent == null)
        {
            Debug.LogError($"[TestUIManager] UI界面 {uiName} 没有 {typeof(T).Name} 组件");
            Destroy(uiObj);
            return null;
        }

        // 初始化界面
        _openedUIs.Add(uiName, uiComponent);
        uiComponent.OnInit(userData);

        Debug.Log($"[TestUIManager] 打开UI界面: {uiName}");
        return uiComponent;
    }

    /// <summary>
    /// 关闭UI界面
    /// </summary>
    public void CloseUI(string uiName)
    {
        if (!_openedUIs.TryGetValue(uiName, out var ui))
        {
            Debug.LogWarning($"[TestUIManager] UI界面 {uiName} 未打开");
            return;
        }

        // 调用界面的关闭方法
        ui.OnClose();

        // 销毁界面对象
        Destroy(ui.gameObject);

        // 从已打开界面中移除
        _openedUIs.Remove(uiName);

        Debug.Log($"[TestUIManager] 关闭UI界面: {uiName}");
    }

    /// <summary>
    /// 关闭所有UI界面
    /// </summary>
    public void CloseAllUI()
    {
        foreach (var ui in _openedUIs.Values)
        {
            ui.OnClose();
            Destroy(ui.gameObject);
        }

        _openedUIs.Clear();

        Debug.Log("[TestUIManager] 关闭所有UI界面");
    }

    /// <summary>
    /// 获取已打开的UI界面
    /// </summary>
    public T GetUI<T>(string uiName) where T : TestUIBase
    {
        if (_openedUIs.TryGetValue(uiName, out var ui))
        {
            return ui as T;
        }

        return null;
    }

    /// <summary>
    /// 判断UI界面是否已打开
    /// </summary>
    public bool IsUIOpened(string uiName)
    {
        return _openedUIs.ContainsKey(uiName);
    }
    #endregion
}

