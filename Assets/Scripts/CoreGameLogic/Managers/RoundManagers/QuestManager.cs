using cfg.HuntingConfig;
using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using GameFramework.Utility;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// 任务管理器
/// </summary>
public class QuestManager : IMapWorld, IRoundUpdatable
{
    /// <summary>
    /// 当前处理器
    /// </summary>
    private IQuestHandler _currentHandler;

    /// <summary>
    /// 当前剩余时间（秒）
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

    public UniTask InitAsync(RoundContext context)
    {
        BindServices();
        CacheHandlers();

        _questGlobal = _configManager.GetQuestGlobal();
        ResetDispatchTimer(_questGlobal.StartDispatchTime);

        Log.Info("[QuestManager] 初始化完成");
        return UniTask.CompletedTask;
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
        if (_currentRemainingTime < 0f)
            _currentRemainingTime = 0f;

        TriggerQuestTimeUpdated(new QuestTimeUpdatedEventArgs
        {
            RemainingTime = _currentRemainingTime
        });

        if (_currentRemainingTime <= 0f)
            EndQuest(isCompleted: false);
    }

    public void Dispose()
    {
        EndQuest(isCompleted: false);
        Log.Info("[QuestManager] 已释放");
    }

    public void Unbind()
    {
        EndQuest(isCompleted: false);
    }

    public void Bind(Map mapData)
    {
        ResetDispatchTimer(_questGlobal.StartDispatchTime);
    }

    /// <summary>
    /// 中止当前任务
    /// </summary>
    public void Abort()
    {
        EndQuest(isCompleted: false);
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
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
            IQuestHandler handler = QuestHandlerFactory.CreateQuestHandler(questType);
            if (handler != null)
                _questHandlers[questType] = handler;
        }
    }

    /// <summary>
    /// 重置派发计时
    /// </summary>
    private void ResetDispatchTimer(float interval)
    {
        _dispatchTimer = 0f;
        _dispatchInterval = interval;
    }

    /// <summary>
    /// 派发任务
    /// </summary>
    private void DispatchQuest()
    {
        Quest questData = _configManager.GetRandomQuest();
        if (questData == null || !_questHandlers.TryGetValue(questData.QuestType, out IQuestHandler handler) || handler == null)
        {
            ResetDispatchTimer(_dispatchInterval);
            return;
        }

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
        handler.OnQuestProgressUpdated = TriggerProgressFromHandler;
        handler.OnQuestCompleted = rewardValue => EndQuest(isCompleted: true, rewardValue);

        handler.StartQuest();

        _currentHandler = handler;
        _currentRemainingTime = _questGlobal.Duration;
        _dispatchTimer = 0f;
        _dispatchInterval = _questGlobal.DispatchInterval;
    }

    /// <summary>
    /// 结束当前任务
    /// </summary>
    private void EndQuest(bool isCompleted, int rewardValue = 0)
    {
        if (_currentHandler == null)
            return;

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
        }
        else
            TriggerQuestTimeout();

        _currentHandler = null;
        _currentRemainingTime = 0f;
        ResetDispatchTimer(_questGlobal.DispatchInterval);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 处理器进度回调
    /// </summary>
    private void TriggerProgressFromHandler(int currentProgress)
    {
        TriggerProgressUpdated(new QuestProgressUpdatedEventArgs
        {
            CurrentProgress = currentProgress
        });
    }

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
    /// 触发剩余时间更新事件
    /// </summary>
    private void TriggerQuestTimeUpdated(QuestTimeUpdatedEventArgs args)
    {
        _eventManager.Trigger(QuestEvents.QuestTimeUpdated, args);
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
