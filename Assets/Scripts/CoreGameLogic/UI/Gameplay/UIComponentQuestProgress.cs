using DG.Tweening;
using GameFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务进度组件（进度条 + 进度文本）
/// </summary>
public class UIComponentQuestProgress : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 进度填充时长（秒）
    /// </summary>
    private const float FillDuration = 0.3f;

    /// <summary>
    /// 进度填充图像
    /// </summary>
    [SerializeField] private Image _imageProgressFill;

    /// <summary>
    /// 进度文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textProgress;

    /// <summary>
    /// 目标值缓存
    /// </summary>
    private int _targetValue;

    /// <summary>
    /// 填充动画
    /// </summary>
    private Tween _fillTween;

    private EventManager _eventManager;

    private void Awake()
    {
        BindServices();
    }

    public void Init()
    {
        _eventManager.AddListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        _eventManager.AddListener(QuestEvents.QuestProgressUpdated, OnQuestProgressUpdated);
    }

    public void CleanUp()
    {
        _fillTween?.Kill();
        _fillTween = null;

        _eventManager.RemoveListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        _eventManager.RemoveListener(QuestEvents.QuestProgressUpdated, OnQuestProgressUpdated);
        _targetValue = 0;
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
    }

    /// <summary>
    /// 刷新进度显示
    /// </summary>
    /// <param name="currentProgress">当前进度</param>
    private void RefreshProgress(int currentProgress)
    {
        float ratio = _targetValue > 0
            ? Mathf.Clamp01((float)currentProgress / _targetValue)
            : 0f;

        _fillTween?.Kill();
        _fillTween = _imageProgressFill
            .DOFillAmount(ratio, FillDuration)
            .SetLink(gameObject);

        _textProgress.text = $"{currentProgress}/{_targetValue}";
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 任务派发事件回调
    /// </summary>
    private void OnQuestDispatched(QuestDispatchedEventArgs args)
    {
        _targetValue = args.TargetValue;
        RefreshProgress(0);
    }

    /// <summary>
    /// 任务进度更新事件回调
    /// </summary>
    private void OnQuestProgressUpdated(QuestProgressUpdatedEventArgs args)
    {
        RefreshProgress(args.CurrentProgress);
    }
    #endregion
}
