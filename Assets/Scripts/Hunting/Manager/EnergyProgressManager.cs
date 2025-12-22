using cfg;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using GameFramework.Game;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 丰收能量条管理器
    /// </summary>
    public class EnergyProgressManager : BaseGameManager
    {
        /// <summary>
        /// 当前能量值
        /// </summary>
        private float _currentEnergyValue;

        /// <summary>
        /// 当前能量条条数
        /// </summary>
        private int _currentBars;

        /// <summary>
        /// 单条所需能量值
        /// </summary>
        private float _requiredPerBar;

        /// <summary>
        /// 能量条上限
        /// </summary>
        private int _maxBars;

        /// <summary>
        /// 自动积攒速率（每秒）
        /// </summary>
        private float _increasePerSecond;

        /// <summary>
        /// 是否正在自动积攒
        /// </summary>
        private bool _isAutoAccumulating;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        public override void Init()
        {
            LoadConfig();
            RegisterEvents();
            ResetProgress();
            Debug.Log("[EnergyProgressManager] 初始化完成");
        }

        public void Update()
        {
            if (!_isAutoAccumulating)
                return;

            // 累积本帧自动增加的能量
            float deltaEnergy = Time.deltaTime * _increasePerSecond;
            if (deltaEnergy > 0f)
                AddEnergy(deltaEnergy);
        }

        public override void DoUpdate()
        {

        }

        public override void Release()
        {
            UnregisterEvents();
            ResetProgress();
            Debug.Log("[EnergyProgressManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 获取当前能量值
        /// </summary>
        public float GetCurrentEnergyValue() => _currentEnergyValue;

        /// <summary>
        /// 获取当前能量条条数
        /// </summary>
        public int GetCurrentBars() => _currentBars;

        /// <summary>
        /// 获取单条所需能量值
        /// </summary>
        public float GetRequiredPerBar() => _requiredPerBar;

        /// <summary>
        /// 获取能量条上限
        /// </summary>
        public int GetMaxBars() => _maxBars;

        /// <summary>
        /// 尝试消耗一条能量
        /// </summary>
        public bool TryConsumeOneBar()
        {
            if (_currentBars <= 0)
                return false;

            // 直接扣除一条能量
            _currentBars = _currentBars - 1;
            TriggerBarCountChanged(new EnergyBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = _currentBars
            });
            TriggerEnergyConsumed(new EnergyConsumedEventArgs
            {
                Sender = this,
                RemainingBars = _currentBars
            });
            TriggerProgressChanged(new EnergyProgressChangedEventArgs
            {
                Sender = this,
                CurrentEnergy = _currentEnergyValue,
                CurrentBars = _currentBars
            });
            return true;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 读取配置
        /// </summary>
        private void LoadConfig()
        {
            EnergyProgress energyProgress = Config.GetEnergyProgress(1);
            _requiredPerBar = energyProgress.RequiredPerBar;
            _maxBars = energyProgress.MaxBar;
            _increasePerSecond = energyProgress.IncreasePerSecond;
        }

        /// <summary>
        /// 重置能量进度
        /// </summary>
        private void ResetProgress()
        {
            _currentEnergyValue = 0f;
            _currentBars = 0;
            _isAutoAccumulating = false;
            TriggerProgressChanged(new EnergyProgressChangedEventArgs
            {
                Sender = this,
                CurrentEnergy = _currentEnergyValue,
                CurrentBars = _currentBars
            });
            TriggerBarCountChanged(new EnergyBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = _currentBars
            });
        }

        /// <summary>
        /// 增加能量
        /// </summary>
        public void AddEnergy(float amount)
        {
            //Debug.LogWarning("[AddEnergy] " + amount);

            if (amount <= 0f || _currentBars >= _maxBars)
                return;

            // 累加能量值，并在超过单条阈值时转换成完整能量条
            _currentEnergyValue += amount;

            bool barIncreased = false;
            while (_currentEnergyValue >= _requiredPerBar && _currentBars < _maxBars)
            {
                _currentEnergyValue -= _requiredPerBar;
                _currentBars++;
                barIncreased = true;
            }

            if (_currentBars >= _maxBars)
            {
                _currentBars = _maxBars;
                _currentEnergyValue = 0f;
            }

            //Debug.LogWarning("[AddEnergy] 触发 TriggerProgressChanged" + "_currentEnergyValue" + _currentEnergyValue + "_currentBars" + _currentBars);
            TriggerProgressChanged(new EnergyProgressChangedEventArgs
            {
                Sender = this,
                CurrentEnergy = _currentEnergyValue,
                CurrentBars = _currentBars,
            });
            if (barIncreased)
            {
                Debug.LogWarning("[AddEnergy] 触发 TriggerBarCountChanged" + "_currentBars" + _currentBars);
                TriggerBarCountChanged(new EnergyBarCountChangedEventArgs
                {
                    Sender = this,
                    CurrentBars = _currentBars
                });
            }

            if (_currentBars >= _maxBars)
                TriggerMaxBarsReached();
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);
            Event.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
            Event.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        }

        /// <summary>
        /// 单局开始回调
        /// </summary>
        private void OnRoundStarted(RoundStartedEventArgs args)
        {
            _isAutoAccumulating = true;
        }

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            _isAutoAccumulating = false;
            ResetProgress();
        }

        /// <summary>
        /// 动物掉落奖励回调
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            if (!args.DropRewards.TryGetValue(EDropType.Energy, out int energyAmount) || energyAmount <= 0)
                return;

            AddEnergy(energyAmount);
        }

        /// <summary>
        /// 触发能量值变化事件
        /// </summary>
        private void TriggerProgressChanged(EnergyProgressChangedEventArgs args)
        {
            Event.Trigger(EnergyEvents.EnergyProgressChanged, args);
        }

        /// <summary>
        /// 触发能量条数变化事件
        /// </summary>
        private void TriggerBarCountChanged(EnergyBarCountChangedEventArgs args)
        {
            Event.Trigger(EnergyEvents.EnergyBarCountChanged, args);
        }

        /// <summary>
        /// 触发能量条达成上限事件
        /// </summary>
        private void TriggerMaxBarsReached()
        {
            Event.Trigger(EnergyEvents.EnergyMaxBarsReached);
        }

        /// <summary>
        /// 触发能量条被消耗事件
        /// </summary>
        private void TriggerEnergyConsumed(EnergyConsumedEventArgs args)
        {
            Event.Trigger(EnergyEvents.EnergyConsumed, args);
        }

       

        #endregion
    }
}

