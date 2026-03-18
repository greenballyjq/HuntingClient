using DG.Tweening;
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
    /// 是否处于按下/拖拽状态
    /// </summary>
    private bool _isActive;
    public bool IsActive => _isActive;

    /// <summary>
    /// 水平输入值，范围 -1~1，摇杆中心为0
    /// </summary>
    private float _horizontalInput;
    public float HorizontalInput => _horizontalInput;

    /// <summary>
    /// 摇杆弹簧式归位动画
    /// </summary>
    private Tween _springTween;

    private const float DRAG_RANGE_RATIO = 0.48f;
    private const float SPRING_DURATION = 0.25f;

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
        KillSpringTween();
        ResetKnobPosition();
        Joystick = null;
        Debug.Log("[UIComponentJoystick] 已清理");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        KillSpringTween();
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
        StartSpringReturn();
    }

    #region 公共方法
    /// <summary>
    /// 应用外部输入（如屏幕滑动）
    /// </summary>
    /// <param name="horizontalInput">水平输入值，范围 -1~1</param>
    /// <param name="isActive">是否激活</param>
    public void ApplyExternalInput(float horizontalInput, bool isActive)
    {
        if (isActive)
        {
            KillSpringTween();
            _isActive = true;
            _horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);

            float dragRange = GetDragRange();
            var anchoredPos = _rectKnob.anchoredPosition;
            anchoredPos.x = _horizontalInput * dragRange;
            _rectKnob.anchoredPosition = anchoredPos;
        }
        else
        {
            _isActive = false;
            StartSpringReturn();
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 获取拖拽范围
    /// </summary>
    private float GetDragRange()
    {
        var rect = _rectBase.rect;
        return rect.width * 0.5f * DRAG_RANGE_RATIO;
    }

    /// <summary>
    /// 刷新摇杆位置
    /// </summary>
    private void RefreshKnobPosition(PointerEventData eventData)
    {
        Vector2 localPoint = PointConverter.ScreenPointToUiLocalPoint(_rectBase, eventData.position, _uiCamera);

        var rect = _rectBase.rect;
        float halfWidth = rect.width * 0.5f;
        float dragRange = halfWidth * DRAG_RANGE_RATIO;

        float knobX = Mathf.Clamp(localPoint.x, -dragRange, dragRange);
        _horizontalInput = knobX / dragRange;

        var anchoredPos = _rectKnob.anchoredPosition;
        anchoredPos.x = knobX;
        _rectKnob.anchoredPosition = anchoredPos;
    }

    /// <summary>
    /// 摇杆弹簧式归位，松手后平滑回到中心
    /// </summary>
    private void StartSpringReturn()
    {
        KillSpringTween();

        float dragRange = GetDragRange();
        _springTween = _rectKnob.DOAnchorPosX(0f, SPRING_DURATION)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                _horizontalInput = _rectKnob.anchoredPosition.x / dragRange;
            })
            .OnKill(() => _springTween = null);
    }

    /// <summary>
    /// 停止弹簧归位动画
    /// </summary>
    private void KillSpringTween()
    {
        if (_springTween != null && _springTween.IsActive())
        {
            _springTween.Kill();
            _springTween = null;
        }
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
