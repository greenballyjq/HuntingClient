using System.Collections.Generic;
﻿using System.Collections.Generic;
using cfg;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.App;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 肉度条管理器
    /// </summary>
    public class MeatProgressManager : IAppManager
    {
        /// <summary>
        /// 当前肉度值
        /// </summary>
        private float _currentMeatValue;

        /// <summary>
        /// 当前肉度条条数
        /// </summary>
        private int _currentMeatBars;

        /// <summary>
        /// 单条所需肉度值
        /// </summary>
        private float _requiredPerBar;

        /// <summary>
        /// 肉度条上限
        /// </summary>
        private int _maxMeatBars;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        public void Init()
        {
            LoadConfig();
            RegisterEvents();
            ResetProgress();
            Debug.Log("[MeatProgressManager] 初始化完成");
        }

        public void Dispose()
        {
            UnregisterEvents();
            ResetProgress();
            Debug.Log("[MeatProgressManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 获取当前肉度值
        /// </summary>
        public float GetCurrentMeatValue() => _currentMeatValue;

        /// <summary>
        /// 获取当前肉度条条数
        /// </summary>
        public int GetCurrentMeatBars() => _currentMeatBars;

        /// <summary>
        /// 获取单条所需肉度值
        /// </summary>
        public float GetRequiredPerBar() => _requiredPerBar;

        /// <summary>
        /// 获取肉度条上限
        /// </summary>
        public int GetMaxMeatBars() => _maxMeatBars;

        /// <summary>
        /// 增加肉度
        /// </summary>
        public void AddMeat(float amount)
        {
            if (amount <= 0f || _currentMeatBars >= _maxMeatBars)
                return;

            // 累加肉度值
            _currentMeatValue += amount;
            bool barIncreased = false;

            // 检查是否跨越条数阈值
            while (_currentMeatValue >= _requiredPerBar && _currentMeatBars < _maxMeatBars)
            {
                _currentMeatValue -= _requiredPerBar;
                _currentMeatBars++;
                barIncreased = true;
            }

            if (_currentMeatBars >= _maxMeatBars)
            {
                // 达到条数上限时，清空肉度值并封顶条数
                _currentMeatBars = _maxMeatBars;
                _currentMeatValue = 0f;
            }

            TriggerProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentMeat = _currentMeatValue,
                CurrentBars = _currentMeatBars
            });
            if (barIncreased)
            {
                TriggerBarCountChanged(new MeatBarCountChangedEventArgs
                {
                    Sender = this,
                    CurrentBars = _currentMeatBars
                });
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 加载肉度配置
        /// </summary>
        private void LoadConfig()
        {
            var meatProgress = Config.GetMeatProgress(1);
            _requiredPerBar = meatProgress.RequiredPerBar;
            _maxMeatBars = meatProgress.MaxBar;
        }

        /// <summary>
        /// 重置进度
        /// </summary>
        private void ResetProgress()
        {
            _currentMeatValue = 0f;
            _currentMeatBars = 0;
            TriggerProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentMeat = _currentMeatValue,
                CurrentBars = _currentMeatBars
            });
            TriggerBarCountChanged(new MeatBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = _currentMeatBars
            });
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(RoundFlowEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundFlowEvents.RoundEnded, OnRoundEnded);
            Event.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(RoundFlowEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundFlowEvents.RoundEnded, OnRoundEnded);
            Event.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }
        
        /// <summary>
        /// 本局开始回调
        /// </summary>
        private void OnRoundStarted(RoundStartedEventArgs args)
        {
            ResetProgress();
        }

        /// <summary>
        /// 本局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            ResetProgress();
        }

        /// <summary>
        /// 掉落奖励回调
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            if (!args.DropRewards.TryGetValue(EDropType.Meat, out var meatAmount) || meatAmount <= 0)
                return;

            AddMeat(meatAmount);
        }

        /// <summary>
        /// 触发肉度值变化事件
        /// </summary>
        private void TriggerProgressChanged(MeatProgressChangedEventArgs args)
        {
            Event.Trigger(MeatEvents.MeatProgressChanged, args);
        }

        /// <summary>
        /// 触发肉度条变化事件
        /// </summary>
        private void TriggerBarCountChanged(MeatBarCountChangedEventArgs args)
        {
            Event.Trigger(MeatEvents.MeatBarCountChanged, args);

            if (_currentMeatBars >= _maxMeatBars)
                Event.Trigger(MeatEvents.MeatMaxBarsReached);
        }

        
        #endregion
    }
}


