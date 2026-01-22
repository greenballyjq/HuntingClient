using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 肉度条展示组件
/// </summary>
public class UIComponentMeatProgress : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 肉条填充图像
    /// </summary>
    [SerializeField] private Image _imageMeatBarFill;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 肉条进度管理器
    /// </summary>
    private MeatProgressManager _meatProgressManager => GameServiceLocator.GetRoundManager<MeatProgressManager>();

    public void Init()
    {
        _eventManager.AddListener(MeatEvents.MeatValueChanged, OnMeatValueChanged);

        _imageMeatBarFill.fillAmount = _meatProgressManager.GetTotalProgressRatio();
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
    /// 更新肉度进度显示
    /// </summary>
    /// <param name="progressRatio">进度比例（0-1之间）</param>
    private void UpdateMeatProgress(float progressRatio)
    {
        _imageMeatBarFill.DOFillAmount(progressRatio, 0.3f);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 肉度值变化事件回调
    /// </summary>
    private void OnMeatValueChanged(MeatValueChangedEventArgs args)
    {
        UpdateMeatProgress(args.TotalProgressRatio);
    }
    #endregion
}

