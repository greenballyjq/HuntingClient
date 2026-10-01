using System;
using DG.Tweening;
using GameFramework.Audio;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务面板
/// </summary>
[UIForm(UILayer.Hud)]
public class UIQuest : UIForm
{
    /// <summary>
    /// 滑入滑出动画配置
    /// </summary>
    [Serializable]
    private class SlideConfig
    {
        /// <summary>
        /// 动画时长（秒）
        /// </summary>
        public float Duration = 0.35f;

        /// <summary>
        /// 缓动类型
        /// </summary>
        public Ease Ease = Ease.OutCubic;

        /// <summary>
        /// 是否不受 TimeScale 影响
        /// </summary>
        public bool UseUnscaledTime;

        /// <summary>
        /// 显示时距屏幕顶部的间距（像素）
        /// </summary>
        public float TopPadding = 100f;

        /// <summary>
        /// 显示时距屏幕左边的间距（像素）
        /// </summary>
        public float LeftPadding = 24f;

        /// <summary>
        /// 任务完成后延迟退场时长（秒）
        /// </summary>
        public float CompleteExitDelay = 1.5f;
    }

    /// <summary>
    /// 任务卡片根节点
    /// </summary>
    [SerializeField] private GameObject _panelQuest;

    /// <summary>
    /// 背景图像
    /// </summary>
    [SerializeField] private Image _imageBackground;

    /// <summary>
    /// 任务描述组件
    /// </summary>
    [SerializeField] private UIComponentQuestDescription _uiComponentQuestDescription;

    /// <summary>
    /// 任务进度组件
    /// </summary>
    [SerializeField] private UIComponentQuestProgress _uiComponentQuestProgress;

    /// <summary>
    /// 任务奖励组件
    /// </summary>
    [SerializeField] private UIComponentQuestReward _uiComponentQuestReward;

    /// <summary>
    /// 任务倒计时组件
    /// </summary>
    [SerializeField] private UIComponentQuestCountdown _uiComponentQuestCountdown;

    /// <summary>
    /// 滑入滑出配置
    /// </summary>
    [SerializeField] private SlideConfig _slideConfig = new SlideConfig();

    /// <summary>
    /// 任务卡片矩形变换
    /// </summary>
    private RectTransform _panelRectTransform;

    /// <summary>
    /// 显示时的锚点 X（面板左缘贴父左缘）
    /// </summary>
    private float _shownX;

    /// <summary>
    /// 隐藏时的锚点 X（整块在父左缘外侧）
    /// </summary>
    private float _hiddenX;

    /// <summary>
    /// 显示时的锚点 Y（偏上）
    /// </summary>
    private float _shownY;

    /// <summary>
    /// 当前滑入滑出动画
    /// </summary>
    private Tween _slideTween;

    /// <summary>
    /// 完成后延迟退场
    /// </summary>
    private Tween _exitDelayTween;

    private EventManager _eventManager;
    private AudioManager _audioManager;
    private HuntingConfigManager _configManager;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _audioManager = GameServiceLocator.AudioManager;
        _configManager = GameServiceLocator.ConfigManager;

        if (_panelQuest != null)
            _panelRectTransform = _panelQuest.GetComponent<RectTransform>();
    }

    protected override void OnOpen()
    {

        CacheSlidePositions();
        SnapToHidden();

        _eventManager.AddListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        _eventManager.AddListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        _eventManager.AddListener(QuestEvents.QuestTimeout, OnQuestTimeout);

        _uiComponentQuestDescription.Init();
        _uiComponentQuestProgress.Init();
        _uiComponentQuestReward.Init();
        _uiComponentQuestCountdown.Init();
    }

    protected override void OnClose()
    {
        _uiComponentQuestDescription.CleanUp();
        _uiComponentQuestProgress.CleanUp();
        _uiComponentQuestReward.CleanUp();
        _uiComponentQuestCountdown.CleanUp();

        _eventManager.RemoveListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        _eventManager.RemoveListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        _eventManager.RemoveListener(QuestEvents.QuestTimeout, OnQuestTimeout);

        KillExitDelay();
        KillSlideTween();
        SnapToHidden();

        base.OnClose();
    }

    #region 私有方法
    /// <summary>
    /// 按父 Rect 与面板尺寸缓存显示/隐藏位置
    /// </summary>
    private void CacheSlidePositions()
    {
        if (_panelRectTransform == null)
            return;

        RectTransform parentRect = _panelRectTransform.parent as RectTransform;
        float parentWidth = parentRect != null ? parentRect.rect.width : Screen.width;
        float parentHeight = parentRect != null ? parentRect.rect.height : Screen.height;
        float panelWidth = _panelRectTransform.rect.width;
        float panelHeight = _panelRectTransform.rect.height;

        // 左缘距屏幕左 LeftPadding；整块再向左移一个面板宽即完全藏在外侧
        _shownX = -parentWidth / 2f + panelWidth / 2f + _slideConfig.LeftPadding;
        _hiddenX = -parentWidth / 2f - panelWidth / 2f;
        // 上缘距屏幕顶 TopPadding，整体偏上而不是垂直居中
        _shownY = parentHeight / 2f - panelHeight / 2f - _slideConfig.TopPadding;
    }

    /// <summary>
    /// 瞬移到屏幕左外侧
    /// </summary>
    private void SnapToHidden()
    {
        if (_panelRectTransform == null)
            return;

        _panelQuest.SetActive(true);
        _panelRectTransform.anchoredPosition = new Vector2(_hiddenX, _shownY);
    }

    /// <summary>
    /// 滑入到显示位置
    /// </summary>
    private void SlideIn()
    {
        if (_panelRectTransform == null)
            return;

        CacheSlidePositions();
        KillExitDelay();
        KillSlideTween();

        _panelQuest.SetActive(true);
        _panelRectTransform.anchoredPosition = new Vector2(_panelRectTransform.anchoredPosition.x, _shownY);
        _slideTween = _panelRectTransform
            .DOAnchorPosX(_shownX, _slideConfig.Duration)
            .SetEase(_slideConfig.Ease)
            .SetUpdate(_slideConfig.UseUnscaledTime)
            .SetLink(gameObject);
    }

    /// <summary>
    /// 滑出到屏幕左外侧
    /// </summary>
    private void SlideOut()
    {
        if (_panelRectTransform == null)
            return;

        CacheSlidePositions();
        KillSlideTween();

        _slideTween = _panelRectTransform
            .DOAnchorPosX(_hiddenX, _slideConfig.Duration)
            .SetEase(_slideConfig.Ease)
            .SetUpdate(_slideConfig.UseUnscaledTime)
            .SetLink(gameObject);
    }

    /// <summary>
    /// 打断当前滑入滑出动画
    /// </summary>
    private void KillSlideTween()
    {
        if (_slideTween != null && _slideTween.IsActive())
            _slideTween.Kill();
        _slideTween = null;
    }

    /// <summary>
    /// 打断完成后的延迟退场
    /// </summary>
    private void KillExitDelay()
    {
        if (_exitDelayTween != null && _exitDelayTween.IsActive())
            _exitDelayTween.Kill();
        _exitDelayTween = null;
    }

    /// <summary>
    /// 延迟后再滑出
    /// </summary>
    private void ScheduleSlideOut(float delay)
    {
        KillExitDelay();

        if (delay <= 0f)
        {
            SlideOut();
            return;
        }

        _exitDelayTween = DOVirtual
            .DelayedCall(delay, SlideOut, _slideConfig.UseUnscaledTime)
            .SetLink(gameObject);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 任务派发事件回调
    /// </summary>
    private void OnQuestDispatched(QuestDispatchedEventArgs args)
    {
        _audioManager.Play(_configManager.UiAudioRefSo.QuestDispatched);
        SlideIn();
    }

    /// <summary>
    /// 任务完成事件回调
    /// </summary>
    private void OnQuestCompleted(QuestCompletedEventArgs args)
    {
        _audioManager.Play(_configManager.UiAudioRefSo.QuestCompleted);
        ScheduleSlideOut(_slideConfig.CompleteExitDelay);
    }

    /// <summary>
    /// 任务超时事件回调
    /// </summary>
    private void OnQuestTimeout()
    {
        KillExitDelay();
        SlideOut();
    }
    #endregion
}
