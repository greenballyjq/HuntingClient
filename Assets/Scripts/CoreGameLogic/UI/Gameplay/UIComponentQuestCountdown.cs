using GameFramework.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 任务倒计时组件
/// </summary>
public class UIComponentQuestCountdown : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 倒计时文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCountdown;

    private EventManager _eventManager;

    private void Awake()
    {
        BindServices();
    }

    public void Init()
    {
        _eventManager.AddListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        _eventManager.AddListener(QuestEvents.QuestTimeUpdated, OnQuestTimeUpdated);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        _eventManager.RemoveListener(QuestEvents.QuestTimeUpdated, OnQuestTimeUpdated);
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
    /// 刷新倒计时显示
    /// </summary>
    /// <param name="remainingSeconds">剩余秒数</param>
    private void RefreshCountdown(float remainingSeconds)
    {
        int seconds = Mathf.Max(0, Mathf.FloorToInt(remainingSeconds));
        _textCountdown.text = seconds.ToString();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 任务派发事件回调
    /// </summary>
    private void OnQuestDispatched(QuestDispatchedEventArgs args)
    {
        RefreshCountdown(args.Duration);
    }

    /// <summary>
    /// 任务剩余时间更新事件回调
    /// </summary>
    private void OnQuestTimeUpdated(QuestTimeUpdatedEventArgs args)
    {
        RefreshCountdown(args.RemainingTime);
    }
    #endregion
}
