using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using GameFramework.Manager;
using GameFramework.Utility;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 道具管理器
/// </summary>
public class PropManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 活跃道具上下文
    /// </summary>
    private class ActiveProp
    {
        /// <summary>
        /// 道具处理器
        /// </summary>
        public IPropHandler Handler;

        /// <summary>
        /// 道具上下文
        /// </summary>
        public PropContext Context;

        /// <summary>
        /// 道具配置
        /// </summary>
        public Prop PropData;

        /// <summary>
        /// 剩余时间
        /// </summary>
        public float RemainingTime;
    }

    /// <summary>
    /// 活跃道具列表
    /// </summary>
    private readonly List<ActiveProp> _activeProps = new List<ActiveProp>();

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;
    private PlayerDataManager _playerDataManager;

    public void Init(RoundContext context)
    {
        RegisterServices();
        Log.Info("[PropManager] 初始化完成");
    }

    public void DoUpdate(float deltaTime)
    {
        UpdateActiveProps(deltaTime);
    }

    public void Dispose()
    {
        StopAllProps();
        Log.Info("[PropManager] 已释放");
    }

    public void Cleanup()
    {
        StopAllProps();
    }

    public void ReInit(RoundContext context){}

    #region 公共方法
    /// <summary>
    /// 尝试使用道具
    /// </summary>
    /// <param name="propType">道具类型</param>
    /// <returns>是否使用成功</returns>
    public bool TryUseProp(EPropType propType)
    {
        Prop propData = _configManager.GetProp(propType);

        if (_playerDataManager.GetPropCount(propType) < 1)
        {
            Log.Warning($"[PropManager] 道具数量不足，PropType:{propType}");
            return false;
        }

        if (!CanUseProp(propType, propData))
        {
            Log.Warning($"[PropManager] 道具正在使用中，无法重复使用，PropType:{propType}");
            return false;
        }

        _playerDataManager.UpdatePropCount(propType, -1);

        // 创建处理器
        IPropHandler handler = PropHandlerFactory.CreatePropHandler(propType);

        // 构造上下文
        PropContext context = new PropContext
        {
            PropData = propData
        };

        // 开始道具效果
        BeginProp(propData, handler, context);
        
        // 触发道具使用成功事件
        TriggerPropUseSucceeded(new PropUseSucceededEventArgs
        {
            Sender = this,
            PropData = propData
        });
        
        return true;
    }
    #endregion

    #region 私有方法
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
    }

    /// <summary>
    /// 检查道具是否可以使用
    /// </summary>
    /// <param name="propType">道具类型</param>
    /// <param name="propData">道具配置</param>
    /// <returns>是否可以使用</returns>
    private bool CanUseProp(EPropType propType, Prop propData)
    {
        // 有持续时间的道具，持续期间不能重复使用
        if (propData.Duration > 0f)
        {
            // 检查同类型道具是否正在使用
            foreach (var activeProp in _activeProps)
            {
                if (activeProp.PropData.PropType == propType)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 开始道具效果
    /// </summary>
    /// <param name="propData">道具配置</param>
    /// <param name="handler">道具处理器</param>
    /// <param name="context">道具上下文</param>
    private void BeginProp(Prop propData, IPropHandler handler, PropContext context)
    {
        // 创建活跃道具实例
        ActiveProp activeProp = new ActiveProp
        {
            Handler = handler,
            Context = context,
            PropData = propData,
            RemainingTime = propData.Duration
        };

        // 加入活跃列表
        _activeProps.Add(activeProp);

        // 通知处理器执行开始逻辑
        handler?.OnPropStart(context);



        // 触发道具开始事件
        TriggerPropStarted(new PropStartedEventArgs
        {
            Sender = this,
            PropData = propData
        });

        Log.Info($"[PropManager] 道具开始，类型:{propData.PropType}，持续时间:{propData.Duration:F2}秒");

        // 无持续时间的道具，立即结束
        if (propData.Duration <= 0f)
            EndProp(activeProp);
    }

    /// <summary>
    /// 结束道具效果
    /// </summary>
    /// <param name="activeProp">活跃道具实例</param>
    private void EndProp(ActiveProp activeProp)
    {
        // 通知处理器执行结束逻辑
        activeProp.Handler?.OnPropEnd(activeProp.Context);

        // 从活跃列表移除
        _activeProps.Remove(activeProp);

        // 触发道具结束事件
        TriggerPropEnded(new PropEndedEventArgs
        {
            Sender = this,
            PropData = activeProp.PropData
        });

        Log.Info($"[PropManager] 道具结束，类型:{activeProp.PropData.PropType}");
    }

    /// <summary>
    /// 更新所有活跃道具
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    private void UpdateActiveProps(float deltaTime)
    {
        for (int i = _activeProps.Count - 1; i >= 0; i--)
        {
            ActiveProp activeProp = _activeProps[i];

            // 更新剩余时间
            activeProp.RemainingTime -= deltaTime;

            // 通知处理器执行更新逻辑
            activeProp.Handler?.OnPropUpdate(activeProp.Context, deltaTime);

            // 触发道具更新事件
            TriggerPropUpdated(new PropUpdatedEventArgs
            {
                Sender = this,
                PropData = activeProp.PropData,
                RemainingTime = activeProp.RemainingTime
            });

            // 检查是否时间到
            if (activeProp.RemainingTime <= 0f)
                EndProp(activeProp);
        }
    }

    /// <summary>
    /// 停止所有道具
    /// </summary>
    private void StopAllProps()
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

