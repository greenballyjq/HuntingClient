using cfg.HuntingConfig.Enum;
using GameFramework.Core;

/// <summary>
/// 击杀大型动物任务处理器
/// </summary>
public class QuestKillLargeAnimalHandler : IQuestHandler
{
    /// <summary>
    /// 当前击杀的大型动物数量
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

        // 订阅动物死亡事件
        _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
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
        // 取消订阅动物死亡事件
        _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
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
    /// 动物死亡事件回调
    /// </summary>
    private void OnAnimalDied(AnimalDiedEventArgs args)
    {
        // 检查是否为大型动物
        if (args.SpecieData.VolumeType == EVolumeType.Large)
            _currentProgress++;
    }
    #endregion
}