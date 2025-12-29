using GameFramework.Core;

/// <summary>
/// 使用付费道具任务处理器
/// </summary>
public class QuestUsePaidItemHandler : IQuestHandler
{
    /// <summary>
    /// 当前使用的付费道具数量
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

        // 订阅道具使用成功事件
        _eventManager.AddListener(PropEvents.PropUseSucceeded, OnPropUseSucceeded);
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
        // 取消订阅道具使用成功事件
        _eventManager.RemoveListener(PropEvents.PropUseSucceeded, OnPropUseSucceeded);
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
    /// 道具使用成功事件回调
    /// </summary>
    private void OnPropUseSucceeded(PropUseSucceededEventArgs args)
    {
        _currentProgress++;
    }
    #endregion
}

