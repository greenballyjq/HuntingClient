using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// 肉度条展示组件
/// </summary>
public class UIComponentMeatProgress : MonoBehaviour, IUIComponent
{
    [SerializeField] private Image _imageMeat;
    /// <summary>
    /// 肉条填充图像
    /// </summary>
    [SerializeField] private Image _imageMeatBarFill;

    /// <summary>
    /// 肉条进度文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textProgress;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    public RectTransform MeatTransform => _imageMeat.GetComponent<RectTransform>();

    public void Init()
    {
        _eventManager.AddListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);

        UpdateMeatProgressFill(0);

        UpdateMeatProgressText(0);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    #region 私有方法
    /// <summary>
    /// 更新肉条进度填充
    /// </summary>
    /// <param name="progressRatio">进度比例</param>
    private void UpdateMeatProgressFill(float progressRatio)
    {
        _imageMeatBarFill.DOFillAmount(progressRatio, 0.3f);
    }

    /// <summary>
    /// 更新肉条进度文本
    /// </summary>
    /// <param name="progressRatio">进度比例</param>
    private void UpdateMeatProgressText(float progressRatio)
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
        UpdateMeatProgressFill(args.TotalProgressRatio);
        UpdateMeatProgressText(args.TotalProgressRatio);
    }
    #endregion
}

