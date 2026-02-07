using cfg.HuntingConfig.Enum;
using GameFramework.Game;

/// <summary>
/// 收集肉类任务处理器
/// </summary>
public class QuestCollectMeatHandler : IQuestHandler
{
    /// <summary>
    /// 当前收集的肉类数量
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
        _currentProgress = 0;

        _eventManager.AddListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
    }

    /// <summary>
    /// 任务更新
    /// </summary>
    public void OnQuestUpdate(QuestContext context, float deltaTime)
    {

    }

    /// <summary>
    /// 任务结束
    /// </summary>
    public void OnQuestEnd(QuestContext context)
    {
        _eventManager.RemoveListener(AnimalEvents.DropRewardArrived, OnDropRewardArrived);
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
    /// 掉落奖励生效事件回调
    /// </summary>
    private void OnDropRewardArrived(RewardArrivedEventArgs args)
    {
        if (args.DropType == EDropType.Meat)
            _currentProgress += args.DropCount;
    }
    #endregion
}