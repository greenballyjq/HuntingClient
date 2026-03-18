using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using GameFramework.Manager;
using GameFramework.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务管理器
/// </summary>
public class QuestManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 当前处理器
    /// </summary>
    private IQuestHandler _currentHandler;

    /// <summary>
    /// 当前当前剩余时间（秒）
    /// </summary>
    private float _currentRemainingTime;

    /// <summary>
    /// 任务派发计时器
    /// </summary>
    private float _dispatchTimer;

    /// <summary>
    /// 任务派发间隔（秒）
    /// </summary>
    private float _dispatchInterval;

    /// <summary>
    /// 任务全局配置缓存
    /// </summary>
    private QuestGlobal _questGlobal;

    /// <summary>
    /// 任务处理器缓存
    /// </summary>
    private readonly Dictionary<EQuestType, IQuestHandler> _questHandlers = new Dictionary<EQuestType, IQuestHandler>();

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        CacheHandlers();

        _questGlobal = _configManager.GetQuestGlobal();

        _dispatchTimer = 0f;
        _dispatchInterval = _questGlobal.StartDispatchTime;

        Log.Info("[QuestManager] 初始化完成");
    }

    public void DoUpdate(float dt)
    {
        if (_currentHandler == null)
        {
            _dispatchTimer += dt;

            if (_dispatchTimer >= _dispatchInterval)
                DispatchQuest();

            return;
        }

        _currentRemainingTime -= dt;
        if (_currentRemainingTime <= 0f)
            EndQuest(isCompleted: false);
    }

    public void Dispose()
    {
        EndQuest(isCompleted: false);
        Log.Info("[QuestManager] 已释放");
    }

    public void Cleanup()
    {
        EndQuest(isCompleted: false);
    }

    public void ReInit(Map mapData)
    {
        _dispatchTimer = 0f;
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
    }

    /// <summary>
    /// 缓存任务处理器
    /// </summary>
    private void CacheHandlers()
    {
        foreach (EQuestType questType in Enum.GetValues(typeof(EQuestType)))
        {
            var handler = QuestHandlerFactory.CreateQuestHandler(questType);
            _questHandlers[questType] = handler;
        }
    }

    /// <summary>
    /// 派发任务
    /// </summary>
    private void DispatchQuest()
    {
        var questData = _configManager.GetRandomQuest();

        var handler = _questHandlers[questData.QuestType];
        
        handler.Init(questData);

        handler.OnQuestDispatched = (description, targetValue, rewardValue) =>
        {
            TriggerQuestDispatched(new QuestDispatchedEventArgs
            {
                Description = description,
                TargetValue = targetValue,
                RewardValue = rewardValue,
                Duration = _questGlobal.Duration
            });
        };

        handler.OnQuestProgressUpdated = (currentProgress) =>
        {
            TriggerProgressUpdated(new QuestProgressUpdatedEventArgs
            {
                CurrentProgress = currentProgress,
                RemainingTime = _currentRemainingTime
            });
        };

        handler.OnQuestCompleted = (rewardValue) =>
        {
            EndQuest(isCompleted: true, rewardValue);
        };

        handler.StartQuest();

        _currentHandler = handler;
        _currentRemainingTime = _configManager.GetQuestGlobal().Duration;
        _dispatchTimer = 0f;
        _dispatchInterval = _configManager.GetQuestGlobal().DispatchInterval;
    }

    /// <summary>
    /// 结束当前任务
    /// </summary>
    /// <param name="isCompleted">是否完成</param>
    /// <param name="rewardValue">奖励值</param>
    private void EndQuest(bool isCompleted, int rewardValue = 0)
    {
        if (_currentHandler != null)
        {
            _currentHandler.OnQuestDispatched = null;
            _currentHandler.OnQuestProgressUpdated = null;
            _currentHandler.OnQuestCompleted = null;
            _currentHandler.EndQuest();

            if (isCompleted)
            {
                TriggerQuestCompleted(new QuestCompletedEventArgs
                {
                    RewardValue = rewardValue
                });
                Debug.LogWarning(rewardValue);
            }
            else
                TriggerQuestTimeout();
        }

        _currentHandler = null;
        _currentRemainingTime = 0f;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 触发任务派发事件
    /// </summary>
    private void TriggerQuestDispatched(QuestDispatchedEventArgs args)
    {
        _eventManager.Trigger(QuestEvents.QuestDispatched, args);
    }

    /// <summary>
    /// 触发进度更新事件
    /// </summary>
    private void TriggerProgressUpdated(QuestProgressUpdatedEventArgs args)
    {
        _eventManager.Trigger(QuestEvents.QuestProgressUpdated, args);
    }

    /// <summary>
    /// 触发任务完成事件
    /// </summary>
    private void TriggerQuestCompleted(QuestCompletedEventArgs args)
    {
        _eventManager.Trigger(QuestEvents.QuestCompleted, args);
    }

    /// <summary>
    /// 触发任务超时事件
    /// </summary>
    private void TriggerQuestTimeout()
    {
        _eventManager.Trigger(QuestEvents.QuestTimeout);
    }
    #endregion
}
