using System;
using cfg.HuntingConfig;

/// <summary>
/// 任务处理器接口
/// </summary>
public interface IQuestHandler
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="questData">技能数据</param>
    void Init(Quest questData);

    /// <summary>
    /// 开始任务
    /// </summary>
    void StartQuest();

    /// <summary>
    /// 结束任务
    /// </summary>
    void EndQuest();

    /// <summary>
    /// 派发任务回调
    /// </summary>
    Action<string, int, int> OnQuestDispatched { get; set; }

    /// <summary>
    /// 进度更新回调
    /// </summary>
    Action<int> OnQuestProgressUpdated { get; set; }

    /// <summary>
    /// 任务完成回调
    /// </summary>
    Action OnQuestCompleted { get; set; }
}
