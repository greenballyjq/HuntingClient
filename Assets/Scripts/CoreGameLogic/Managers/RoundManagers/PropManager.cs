using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using GameFramework.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具管理器
/// </summary>
public class PropManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 活跃道具
    /// </summary>
    private class ActiveProp
    {
        /// <summary>
        /// 道具处理器
        /// </summary>
        public BasePropHandler Handler;

        /// <summary>
        /// 道具配置
        /// </summary>
        public Prop PropData;
    }

    /// <summary>
    /// 活跃道具列表
    /// </summary>
    private readonly List<ActiveProp> _activeProps = new List<ActiveProp>();

    /// <summary>
    /// 道具处理器缓存
    /// </summary>
    private readonly Dictionary<EPropType, BasePropHandler> _handlerCache = new Dictionary<EPropType, BasePropHandler>();

    /// <summary>
    /// 配置缓存
    /// </summary>
    private readonly Dictionary<EPropType, Prop> _propDataCache = new Dictionary<EPropType, Prop>();

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;
    private PlayerDataManager _playerDataManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        CacheHandler();
        CachePropConfig();
        Log.Info("[PropManager] 初始化完成");
    }

    public void DoUpdate(float dt)
    {
        UpdateActiveProps(dt);
    }

    public void Dispose()
    {
        EndAllProps();

        Log.Info("[PropManager] 已释放");
    }

    public void Cleanup()
    {
        EndAllProps();
    }

    public void ReInit(RoundContext context) { }

    #region 公共方法
    /// <summary>
    /// 尝试开始道具
    /// </summary>
    /// <param name="propType">道具类型</param>
    public void TryStartProp(EPropType propType)
    {
        var handler = _handlerCache[propType];
        if (handler.PropPhase != PropPhase.None)
            return;

        if (_playerDataManager.GetPropCount(propType) < 1)
            return;

        var propData = _propDataCache[propType];
        StartProp(handler, propData);

        _playerDataManager.UpdatePropCount(propType, -1);

        TriggerPropUseSucceeded(new PropUseSucceededEventArgs 
        {
            Sender = this, 
            PropData = propData 
        });
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
    }

    /// <summary>
    /// 缓存处理器
    /// </summary>
    private void CacheHandler()
    {
        foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
        {
            IPropHandler handler = PropHandlerFactory.CreatePropHandler(propType);
            if (handler is BasePropHandler baseHandler)
                _handlerCache[propType] = baseHandler;
        }
    }

    /// <summary>
    /// 缓存道具配置
    /// </summary>
    private void CachePropConfig()
    {
        foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
        {
            Prop propData = _configManager.GetProp(propType);
            if (propData != null)
                _propDataCache[propType] = propData;
        }
    }

    /// <summary>
    /// 开始道具
    /// </summary>
    private void StartProp(BasePropHandler handler, Prop propData)
    {
        ActiveProp activeProp = new ActiveProp
        {
            Handler = handler,
            PropData = propData
        };

        _activeProps.Add(activeProp);

        handler.Init(propData);
        handler.StartProp().Forget();

        TriggerPropStarted(new PropStartedEventArgs 
        { 
            Sender = this, 
            PropData = propData 
        });
    }

    /// <summary>
    /// 结束道具
    /// </summary>
    private void EndProp(ActiveProp activeProp)
    {
        activeProp.Handler.EndProp();

        _activeProps.Remove(activeProp);

        TriggerPropEnded(new PropEndedEventArgs 
        { 
            Sender = this, 
            PropData = activeProp.PropData
        });
    }

    /// <summary>
    /// 更新所有活跃道具
    /// </summary>
    private void UpdateActiveProps(float dt)
    {
        ActiveProp activeProp;
        for (int i = _activeProps.Count - 1; i >= 0; i--)
        {
            activeProp = _activeProps[i];

            if (activeProp.Handler.PropPhase == PropPhase.Finished)
            {
                EndProp(activeProp);
                continue;
            }

            if (activeProp.Handler.PropPhase == PropPhase.Running)
            {
                activeProp.Handler.DoUpdate(dt);

                TriggerPropUpdated(new PropUpdatedEventArgs
                {
                    Sender = this,
                    PropData = activeProp.PropData,
                    RemainingTime = activeProp.Handler.RemainingTime
                });
            }
        }
    }

    /// <summary>
    /// 停止所有道具
    /// </summary>
    private void EndAllProps()
    {
        for (int i = _activeProps.Count - 1; i >= 0; i--)
            EndProp(_activeProps[i]);
    }
    #endregion

    #region 事件相关

    /// <summary>
    /// 触发道具开始事件
    /// </summary>
    private void TriggerPropStarted(PropStartedEventArgs args)
    {
        _eventManager.Trigger(PropEvents.PropStarted, args);
    }

    /// <summary>
    /// 触发道具更新事件
    /// </summary>
    private void TriggerPropUpdated(PropUpdatedEventArgs args)
    {
        _eventManager.Trigger(PropEvents.PropUpdated, args);
    }

    /// <summary>
    /// 触发道具结束事件
    /// </summary>
    private void TriggerPropEnded(PropEndedEventArgs args)
    {
        _eventManager.Trigger(PropEvents.PropEnded, args);
    }

    /// <summary>
    /// 触发道具使用成功事件
    /// </summary>
    private void TriggerPropUseSucceeded(PropUseSucceededEventArgs args)
    {
        _eventManager.Trigger(PropEvents.PropUseSucceeded, args);
    }

    #endregion
}
