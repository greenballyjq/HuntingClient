using System.Collections.Generic;
using cfg;
using cfg.HuntingConfig;
using GameFramework.Core;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 肉度条管理器
    /// </summary>
    public class MeatProgressManager : BaseGameManager
    {
        /// <summary>
        /// 当前肉度值
        /// </summary>
        private int _currentMeatValue;

        /// <summary>
        /// 当前肉度条
        /// </summary>
        private int _currentMeatBars;

        /// <summary>
        /// 单条所需肉度值
        /// </summary>
        private int _requiredPerBar;

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
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        public override void Init()
        {
            LoadConfig();
            RegisterEvents();
            ResetProgress();
            Debug.Log("[MeatProgressManager] 初始化完成");
        }

        public override void Update()
        {
        }

        public override void Release()
        {
            UnregisterEvents();
            ResetProgress();
            Debug.Log("[MeatProgressManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 获取当前肉度值
        /// </summary>
        public int GetCurrentMeatValue() => _currentMeatValue;

        /// <summary>
        /// 获取当前肉度条
        /// </summary>
        public int GetCurrentMeatBars() => _currentMeatBars;

        /// <summary>
        /// 获取单条所需肉度值
        /// </summary>
        public int GetRequiredPerBar() => _requiredPerBar;

        /// <summary>
        /// 获取肉度条上限
        /// </summary>
        public int GetMaxMeatBars() => _maxMeatBars;

        /// <summary>
        /// 获取当前肉度条奖励
        /// </summary>
        public MeatProgressReward GetCurrentReward() => Config.GetMeatProgressReward(_currentMeatBars);
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
            _currentMeatValue = 0;
            _currentMeatBars = 0;
            TriggerProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentProgress = _currentMeatValue,
                CompletedBars = _currentMeatBars
            });
            TriggerBarCountChanged(new MeatBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = _currentMeatBars
            });
        }

        /// <summary>
        /// 增加肉度
        /// </summary>
        private void AddMeat(int amount)
        {
            if (amount <= 0 || _currentMeatBars >= _maxMeatBars)
                return;

            // 累计肉度值
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
                _currentMeatValue = 0;
            }

            TriggerProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentProgress = _currentMeatValue,
                CompletedBars = _currentMeatBars
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

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            // TODO: 将来替换为 RoundEvents.RoundStarted / RoundEvents.RoundEnded
            Event.AddListener("GameStarted", OnGameStarted);
            Event.AddListener("GameEnded", OnGameEnded);
            // ↓
            Event.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);

            Event.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            // TODO: 将来替换为 RoundEvents.RoundStarted / RoundEvents.RoundEnded
            Event.RemoveListener("GameStarted", OnGameStarted);
            Event.RemoveListener("GameEnded", OnGameEnded);
            // ↓
            Event.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
            Event.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }
        
        /// <summary>
        /// 游戏开始回调 TODO: 将来将来替换为 RoundEvents.RoundStarted / RoundEvents.RoundEnded时删掉
        /// </summary>
        private void OnGameStarted()
        {
            ResetProgress();
        }

        /// <summary>
        /// 游戏结束回调 TODO: 将来将来替换为 RoundEvents.RoundStarted / RoundEvents.RoundEnded时删掉
        /// </summary>
        private void OnGameEnded()
        {
            ResetProgress();
        }

        /// <summary>
        /// 本局开始回调
        /// </summary>
        private void OnRoundStarted()
        {
            ResetProgress();
        }

        /// <summary>
        /// 本局结束回调
        /// </summary>
        private void OnRoundEnded()
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


