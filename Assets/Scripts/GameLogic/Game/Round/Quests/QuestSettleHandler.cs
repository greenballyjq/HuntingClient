using GameFramework.Core;

/// <summary>
/// 结算任务处理器
/// </summary>
public class QuestSettleHandler : IQuestHandler
{
    /// <summary>
    /// 当前结算次数
    /// </summary>
    private int _currentProgress;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 任务开始
    /// </summary>
    public void OnQuestStart(QuestContext context)
    {
        // 重置进度
        _currentProgress = 0;

        // 订阅结算开始事件
        _eventManager.AddListener(SettlementEvents.SettlementStarted, OnSettlementStarted);
    }

    /// <summary>
    /// 任务更新
    /// </summary>
    public void OnQuestUpdate(QuestContext context, float deltaTime)
    {
        // 无需更新逻辑
    }

    /// <summary>
    /// 任务结束
    /// </summary>
    public void OnQuestEnd(QuestContext context)
    {
        // 取消订阅结算开始事件
        _eventManager.RemoveListener(SettlementEvents.SettlementStarted, OnSettlementStarted);
    }

    /// <summary>
    /// 获取当前进度值
    /// </summary>
    public int GetCurrentProgress(QuestContext context)
    {
        return _currentProgress;
    }

    #region 事件回调
    /// <summary>
    /// 结算开始事件回调
    /// </summary>
    private void OnSettlementStarted()
    {
        _currentProgress++;
    }
    #endregion
}

