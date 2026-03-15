using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using GameFramework.Manager;

/// <summary>
/// 肉度条组件
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

    private EventManager _eventManager;
    private MeatProgressManager _meatProgressManager;

    private void Awake()
    {
        _rectTransformMeatImage = _imageMeat.GetComponent<RectTransform>();
        _eventManager = GameServiceLocator.EventManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
    }

    public void Init()
    {
        SyncFromManager();

        _eventManager.AddListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);
    }

    #region 私有方法
    /// <summary>
    /// 同步管理器状态
    /// </summary>
    private void SyncFromManager()
    {
        float ratio = _meatProgressManager.TotalProgressRatio;
        RefreshMeatProgressFill(ratio);
        RefreshMeatProgressText(ratio);
    }

    /// <summary>
    /// 刷新肉条进度填充
    /// </summary>
    /// <param name="progressRatio">进度比例</param>
    private void RefreshMeatProgressFill(float progressRatio)
    {
        _imageMeatBarFill.DOFillAmount(progressRatio, 0.3f);
    }

    /// <summary>
    /// 刷新肉条进度文本
    /// </summary>
    /// <param name="progressRatio">进度比例</param>
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
        RefreshMeatProgressFill(args.TotalProgressRatio);
        RefreshMeatProgressText(args.TotalProgressRatio);
    }
    #endregion
}
