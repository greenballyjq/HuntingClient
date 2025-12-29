using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 任务管理器
/// </summary>
public class QuestManager : IRoundManager, IRoundUpdatable
{
    /// <summary>
    /// 任务起始派发时间（秒）
    /// </summary>
    private const float QuestStartTime = 0f;

    /// <summary>
    /// 任务派发间隔（秒）
    /// </summary>
    private const float QuestDispatchInterval = 20f;

    /// <summary>
    /// 任务持续时间（秒）
    /// </summary>
    private const float QuestDuration = 10f;

    /// <summary>
    /// 当前任务处理器
    /// </summary>
    private IQuestHandler _currentHandler;

    /// <summary>
    /// 当前任务上下文
    /// </summary>
    private QuestContext _currentQuestContext;

    /// <summary>
    /// 处理器缓存字典
    /// </summary>
    private Dictionary<EQuestType, IQuestHandler> _handlerCache;

    /// <summary>
    /// 单局开始时间
    /// </summary>
    private float _roundStartTime;

    /// <summary>
    /// 下次派发时间
    /// </summary>
    private float _nextDispatchTime;

    /// <summary>
    /// 当前任务剩余时间
    /// </summary>
    private float _currentQuestRemainingTime;

    /// <summary>
    /// 当前任务目标值
    /// </summary>
    private int _currentTargetValue;

    /// <summary>
    /// 当前任务奖励值
    /// </summary>
    private int _currentRewardValue;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    public void Init(RoundContext context)
    {
        _handlerCache = new Dictionary<EQuestType, IQuestHandler>();
        _roundStartTime = Time.time;
        _nextDispatchTime = QuestStartTime;
        Debug.Log("[QuestManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        if (_currentQuestContext == null)
        {
            // 检查是否需要派发新任务
            float currentTime = Time.time - _roundStartTime;
            if (currentTime >= _nextDispatchTime)
                DispatchQuest();

            return;
        }

        // 更新当前任务剩余时间
        _currentQuestRemainingTime -= deltaTime;

        // 调用处理器更新
        _currentHandler?.OnQuestUpdate(_currentQuestContext, deltaTime);

        // 检查任务是否完成
        int currentProgress = _currentHandler.GetCurrentProgress(_currentQuestContext);
        if (currentProgress >= _currentTargetValue)
        {
            // 任务完成
            EndCurrentQuest(isTimeout: false);
            return;
        }

        // 检查任务是否超时
        if (_currentQuestRemainingTime <= 0f)
        {
            // 任务超时
            EndCurrentQuest(isTimeout: true);
            return;
        }

        // 触发进度更新事件
        TriggerProgressUpdated(new QuestProgressUpdatedEventArgs
        {
            Sender = this,
            QuestData = _currentQuestContext.QuestData,
            CurrentProgress = currentProgress,
            TargetValue = _currentTargetValue,
            RemainingTime = _currentQuestRemainingTime
        });
    }

    public void Dispose()
    {
        EndCurrentQuest(isTimeout: false);
        Debug.Log("[QuestManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 获取当前任务上下文
    /// </summary>
    /// <returns>当前任务上下文</returns>
    public QuestContext GetCurrentQuest()
    {
        return _currentQuestContext;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 派发新任务
    /// </summary>
    private void DispatchQuest()
    {
        // 随机获取任务配置
        Quest questData = _configManager.GetRandomQuest();
        if (questData == null)
        {
            Debug.LogWarning("[QuestManager] 未找到任务配置");
            return;
        }

        // 获取或创建处理器
        if (!_handlerCache.TryGetValue(questData.QuestType, out _currentHandler))
        {
            _currentHandler = QuestHandlerFactory.CreateQuestHandler(questData.QuestType);
            if (_currentHandler == null)
            {
                Debug.LogWarning($"[QuestManager] 未实现的任务类型: {questData.QuestType}");
                return;
            }
            _handlerCache[questData.QuestType] = _currentHandler;
        }

        // 获取随机目标值和奖励值
        _currentTargetValue = _configManager.GetRandomTargetValue(questData);
        _currentRewardValue = _configManager.GetRandomRewardValue(questData);

        // 构造任务上下文
        _currentQuestContext = new QuestContext
        {
            QuestData = questData
        };

        // 设置任务剩余时间
        _currentQuestRemainingTime = QuestDuration;

        // 调用处理器开始
        _currentHandler?.OnQuestStart(_currentQuestContext);

        // 计算下次派发时间
        float currentTime = Time.time - _roundStartTime;
        _nextDispatchTime = currentTime + QuestDispatchInterval;

        // 触发任务派发事件
        TriggerQuestDispatched(new QuestDispatchedEventArgs
        {
            Sender = this,
            QuestData = questData,
            TargetValue = _currentTargetValue,
            RewardValue = _currentRewardValue,
        });

        Debug.Log($"[QuestManager] 任务已派发: {questData.QuestType}，目标值:{_currentTargetValue}，奖励值:{_currentRewardValue}");
    }

    /// <summary>
    /// 结束当前任务
    /// </summary>
    /// <param name="isTimeout">是否超时</param>
    private void EndCurrentQuest(bool isTimeout)
    {
        if (_currentQuestContext == null)
            return;

        // 调用处理器结束
        _currentHandler?.OnQuestEnd(_currentQuestContext);

        if (isTimeout)
        {
            // 触发任务超时事件
            TriggerQuestTimeout(new QuestTimeoutEventArgs
            {
                Sender = this,
                QuestData = _currentQuestContext.QuestData
            });
            Debug.Log("[QuestManager] 任务已超时");
        }
        else
        {
            // 触发任务完成事件
            TriggerQuestCompleted(new QuestCompletedEventArgs
            {
                Sender = this,
                QuestData = _currentQuestContext.QuestData,
                RewardCoin = _currentRewardValue
            });
            Debug.Log($"[QuestManager] 任务已完成，奖励:{_currentRewardValue}");
        }

        // 清理当前任务
        _currentQuestContext = null;
        _currentHandler = null;
        _currentQuestRemainingTime = 0f;
        _currentTargetValue = 0;
        _currentRewardValue = 0;
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
    private void TriggerQuestTimeout(QuestTimeoutEventArgs args)
    {
        _eventManager.Trigger(QuestEvents.QuestTimeout, args);
    }

        
    #endregion
}

