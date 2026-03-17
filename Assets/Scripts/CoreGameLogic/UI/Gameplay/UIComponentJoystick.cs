using GameFramework.Core.UI;
using GameFramework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI摇杆组件
/// </summary>
public class UIComponentJoystick : MonoBehaviour, IUIComponent, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    /// <summary>
    /// 摇杆组件
    /// </summary>
    public static UIComponentJoystick Joystick { get; private set; }

    /// <summary>
    /// 底图图片
    /// </summary>
    [SerializeField] private Image _imageBase;

    /// <summary>
    /// 摇杆图片
    /// </summary>
    [SerializeField] private Image _imageKnob;

    /// <summary>
    /// 底图矩形变换
    /// </summary>
    private RectTransform _rectBase;

    /// <summary>
    /// 摇杆矩形变换
    /// </summary>
    private RectTransform _rectKnob;

    /// <summary>
    /// 所在画布
    /// </summary>
    private Canvas _canvas;

    /// <summary>
    /// UI摄像机
    /// </summary>
    private Camera _uiCamera;

    /// <summary>
    /// 左右可拖动范围占总宽度的比例
    /// </summary>
    private const float DRAG_RANGE_RATIO = 0.33f;

    /// <summary>
    /// 死区占可拖动范围的比例
    /// </summary>
    private const float DEAD_ZONE_RATIO = 0.25f;

    /// <summary>
    /// 死区摇杆颜色（淡红色）
    /// </summary>
    private static readonly Color DEAD_ZONE_COLOR = new Color(1f, 0.75f, 0.75f, 1f);

    /// <summary>
    /// 可拖动范围摇杆颜色（淡绿色）
    /// </summary>
    private static readonly Color DRAG_RANGE_COLOR = new Color(0.75f, 1f, 0.75f, 1f);

    /// <summary>
    /// 默认摇杆颜色（淡红色）
    /// </summary>
    private static readonly Color DEFAULT_COLOR = new Color(1f, 0.75f, 0.75f, 1f);

    /// <summary>
    /// 是否处于按下/拖拽状态
    /// </summary>
    private bool _isActive;
    public bool IsActive => _isActive;

    /// <summary>
    /// 水平输入值
    /// </summary>
    private float _horizontalInput;
    public float HorizontalInput => _horizontalInput;

    private void Awake()
    {
        _rectBase = _imageBase.GetComponent<RectTransform>();
        _rectKnob = _imageKnob.GetComponent<RectTransform>();

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            _uiCamera = _canvas.worldCamera;
        else
            _uiCamera = null;
    }

    public void Init()
    {
        Joystick = this;
        SetKnobColor(DEFAULT_COLOR);
        Debug.Log("[UIComponentJoystick] 初始化完成");
    }

    public void CleanUp()
    {
        _isActive = false;
        _horizontalInput = 0f;
        ResetKnobPosition();
        Joystick = null;
        Debug.Log("[UIComponentJoystick] 已清理");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isActive = true;
        RefreshKnobPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RefreshKnobPosition(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isActive = false;
        ResetKnobPosition();
    }

    #region 私有方法
    /// <summary>
    /// 刷新摇杆位置
    /// </summary>
    private void RefreshKnobPosition(PointerEventData eventData)
    {
        Vector2 localPoint = PointConverter.ScreenPointToUiLocalPoint(_rectBase, eventData.position, _uiCamera);

        var rect = _rectBase.rect;
        float halfWidth = rect.width * 0.5f;
        float dragRange = halfWidth * DRAG_RANGE_RATIO;
        float deadZoneSize = dragRange * DEAD_ZONE_RATIO;

        float knobX = Mathf.Clamp(localPoint.x, -dragRange, dragRange);
        bool inDeadZone = localPoint.x >= -deadZoneSize && localPoint.x <= deadZoneSize;
        if (localPoint.x > deadZoneSize)
            _horizontalInput = 1f;
        else if (localPoint.x < -deadZoneSize)
            _horizontalInput = -1f;
        else
            _horizontalInput = 0f;

        SetKnobColor(inDeadZone ? DEAD_ZONE_COLOR : DRAG_RANGE_COLOR);

        var anchoredPos = _rectKnob.anchoredPosition;
        anchoredPos.x = knobX;
        _rectKnob.anchoredPosition = anchoredPos;
    }

    /// <summary>
    /// 设置摇杆颜色，保持当前透明度
    /// </summary>
    private void SetKnobColor(Color color)
    {
        var c = color;
        c.a = _imageKnob.color.a;
        _imageKnob.color = c;
    }

    /// <summary>
    /// 重置摇杆位置
    /// </summary>
    private void ResetKnobPosition()
    {
        _horizontalInput = 0f;
        SetKnobColor(DEFAULT_COLOR);
        var anchoredPos = _rectKnob.anchoredPosition;
        anchoredPos.x = 0f;
        _rectKnob.anchoredPosition = anchoredPos;
    }
    #endregion
}
