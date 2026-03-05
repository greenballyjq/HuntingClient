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
    /// UI摄像机（Overlay模式为null）
    /// </summary>
    private Camera _uiCamera;

    /// <summary>
    /// 死区阈值
    /// </summary>
    private const float DEAD_ZONE_THRESHOLD = 0.125f;

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
        UpdateKnobPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateKnobPosition(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isActive = false;
        ResetKnobPosition();
    }

    #region 私有方法
    /// <summary>
    /// 更新摇杆位置
    /// </summary>
    private void UpdateKnobPosition(PointerEventData eventData)
    {
        Vector2 localPoint = PointConverter.ScreenPointToUiLocalPoint(_rectBase, eventData.position, _uiCamera);

        var rect = _rectBase.rect;
        float halfWidth = rect.width * 0.5f;
        float thresholdX = halfWidth * DEAD_ZONE_THRESHOLD;

        float knobX = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
        if (localPoint.x > thresholdX)
            _horizontalInput = 1f;
        else if (localPoint.x < -thresholdX)
            _horizontalInput = -1f;
        else
            _horizontalInput = 0f;

        var anchoredPos = _rectKnob.anchoredPosition;
        anchoredPos.x = knobX;
        _rectKnob.anchoredPosition = anchoredPos;
    }

    /// <summary>
    /// 重置摇杆位置
    /// </summary>
    private void ResetKnobPosition()
    {
        _horizontalInput = 0f;
        var anchoredPos = _rectKnob.anchoredPosition;
        anchoredPos.x = 0f;
        _rectKnob.anchoredPosition = anchoredPos;
    }
    #endregion
}
