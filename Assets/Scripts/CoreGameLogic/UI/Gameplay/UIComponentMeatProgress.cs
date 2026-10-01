using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// 肉条进度组件
/// </summary>
public class UIComponentMeatProgress : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 肉图像
    /// </summary>
    [SerializeField] private Image _imageMeat;

    /// <summary>
    /// 肉条填充图像
    /// </summary>
    [SerializeField] private Image _imageMeatBarFill;

    /// <summary>
    /// 进度文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textProgress;

    /// <summary>
    /// 肉图像矩形变换组件
    /// </summary>
    private RectTransform _rectTransformMeatImage;
    public RectTransform RectTransformMeatImage => _rectTransformMeatImage;

    /// <summary>
    /// 填充动画
    /// </summary>
    private Tween _fillTween;

    private EventManager _eventManager;
    private MeatProgressManager _meatProgressManager;

    private void Awake()
    {
        BindServices();
        _rectTransformMeatImage = _imageMeat.GetComponent<RectTransform>();
    }

    public void Init()
    {
        SyncStatus(false);
        _eventManager.AddListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);
    }

    public void CleanUp()
    {
        _fillTween?.Kill();
        _eventManager.RemoveListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
    }

    /// <summary>
    /// 同步管理器状态
    /// </summary>
    private void SyncStatus(bool animated)
    {
        float ratio = _meatProgressManager.TotalProgressRatio;
        RefreshMeatProgressFill(ratio, animated);
        RefreshMeatProgressText(ratio);
    }

    /// <summary>
    /// 刷新肉条进度填充
    /// </summary>
    private void RefreshMeatProgressFill(float progressRatio, bool animated)
    {
        _fillTween?.Kill();
        if (!animated)
        {
            _imageMeatBarFill.fillAmount = progressRatio;
            return;
        }

        _fillTween = _imageMeatBarFill
            .DOFillAmount(progressRatio, 0.3f)
            .SetLink(gameObject);
    }

    /// <summary>
    /// 刷新肉条进度文本
    /// </summary>
    private void RefreshMeatProgressText(float progressRatio)
    {
        _textProgress.text = $"{(int)(progressRatio * 100)}%";
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 肉度值变化事件回调
    /// </summary>
    private void OnMeatValueChanged(MeatValueChangedEventArgs args)
    {
        RefreshMeatProgressFill(args.TotalProgressRatio, true);
        RefreshMeatProgressText(args.TotalProgressRatio);
    }
    #endregion
}
