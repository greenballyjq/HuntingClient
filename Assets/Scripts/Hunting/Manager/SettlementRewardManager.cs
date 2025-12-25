using cfg.HuntingConfig.Enum;
using Hunting.App;
using Hunting.Round;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 结算奖励管理器
    /// </summary>
    public class SettlementRewardManager : IRoundManager
    {
        /// <summary>
        /// 本局完成的肉度条数量
        /// </summary>
        private int _completedMeatBars;

        /// <summary>
        /// 肉度条奖励金币
        /// </summary>
        private int _baseCoin;

        /// <summary>
        /// 肉度条奖励熟练度
        /// </summary>
        private int _baseMastery;

        /// <summary>
        /// 金币物种掉落累计金币
        /// </summary>
        private int _coinFromSpecie;

        /// <summary>
        /// 动态任务累计金币
        /// </summary>
        private int _coinFromQuest;

        /// <summary>
        /// 额外结算倍率
        /// </summary>
        private float _extraMultiplier = 1f;

        /// <summary>
        /// 是否已经完成结算计算
        /// </summary>
        private bool _isSettlementReady;

        /// <summary>
        /// 是否已经应用广告翻倍
        /// </summary>
        private bool _isDoubleApplied;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        public void Init(RoundContext context)
        {
            ClearRoundData();
            RegisterEvents();
            Debug.Log("[SettlementRewardManager] 初始化完成");
        }

        public void Dispose()
        {
            UnregisterEvents();
            ClearRoundData();
            Debug.Log("[SettlementRewardManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 设置额外结算倍率
        /// </summary>
        public void SetExtraMultiplier(float multiplier)
        {
            _extraMultiplier = Mathf.Max(1f, multiplier);
        }

        /// <summary>
        /// 开始计算结算数据
        /// </summary>
        public void StartSettlement()
        {
            CalculateBaseReward();
            _isSettlementReady = true;
            TriggerSettlementCalculated(new SettlementCalculatedEventArgs
            {
                Sender = this,
                CompletedMeatBars = _completedMeatBars,
                BaseCoin = _baseCoin,
                BaseMastery = _baseMastery,
                CoinFromSpecie = _coinFromSpecie,
                CoinFromQuest = _coinFromQuest,
                ExtraMultiplier = _extraMultiplier,
                IsDoubleApplied = _isDoubleApplied,
                TotalCoin = GetTotalCoin(),
                TotalMastery = GetTotalMastery()
            });
        }

        /// <summary>
        /// 应用广告翻倍
        /// </summary>
        public void ApplyDouble()
        {
            if (!_isSettlementReady || _isDoubleApplied)
                return;

            _isDoubleApplied = true;
            TriggerSettlementCalculated(new SettlementCalculatedEventArgs
            {
                Sender = this,
                CompletedMeatBars = _completedMeatBars,
                BaseCoin = _baseCoin,
                BaseMastery = _baseMastery,
                CoinFromSpecie = _coinFromSpecie,
                CoinFromQuest = _coinFromQuest,
                ExtraMultiplier = _extraMultiplier,
                IsDoubleApplied = _isDoubleApplied,
                TotalCoin = GetTotalCoin(),
                TotalMastery = GetTotalMastery()
            });
        }

        /// <summary>
        /// 完成结算
        /// </summary>
        public void CompleteSettlement()
        {
            if (!_isSettlementReady)
                return;

            TriggerSettlementCompleted(new SettlementCompletedEventArgs
            {
                Sender = this,
                TotalCoin = GetTotalCoin(),
                TotalMastery = GetTotalMastery()
            });
            ClearRoundData();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 清理本局结算数据
        /// </summary>
        private void ClearRoundData()
        {
            _completedMeatBars = 0;
            _baseCoin = 0;
            _baseMastery = 0;
            _coinFromSpecie = 0;
            _coinFromQuest = 0;
            _extraMultiplier = 1f;
            _isSettlementReady = false;
            _isDoubleApplied = false;
        }

        /// <summary>
        /// 计算肉度条基础奖励
        /// </summary>
        private void CalculateBaseReward()
        {
            var reward = Config.GetMeatProgressReward(_completedMeatBars);
            if (reward == null)
            {
                _baseCoin = 0;
                _baseMastery = 0;
                return;
            }

            _baseCoin = reward.Coin;
            _baseMastery = reward.Mastery;
        }

        /// <summary>
        /// 计算包含倍率与翻倍后的总金币
        /// </summary>
        private int GetTotalCoin()
        {
            int total = _baseCoin + _coinFromSpecie + _coinFromQuest;
            total = Mathf.RoundToInt(total * _extraMultiplier);
            if (_isDoubleApplied)
            {
                total *= 2;
            }
            return total;
        }

        /// <summary>
        /// 计算包含翻倍后的熟练度
        /// </summary>
        private int GetTotalMastery()
        {
            int mastery = _baseMastery;
            if (_isDoubleApplied)
            {
                mastery *= 2;
            }
            return mastery;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
            Event.AddListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
            Event.AddListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
            Event.RemoveListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
            Event.RemoveListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        }

        /// <summary>
        /// 掉落奖励回调
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            if (!args.DropRewards.TryGetValue(EDropType.Coin, out var coinAmount) || coinAmount < 0)
                return;

            _coinFromSpecie += coinAmount;
        }

        /// <summary>
        /// 肉度条数量变化回调
        /// </summary>
        private void OnMeatBarCountChanged(MeatBarCountChangedEventArgs args)
        {
            _completedMeatBars = Mathf.Max(0, args.CurrentBars);
        }

        /// <summary>
        /// 任务完成回调
        /// </summary>
        private void OnQuestCompleted(QuestCompletedEventArgs args)
        {
            if (args.RewardCoin <= 0)
                return;

            _coinFromQuest += args.RewardCoin;
        }

        /// <summary>
        /// 触发结算计算事件
        /// </summary>
        private void TriggerSettlementCalculated(SettlementCalculatedEventArgs args)
        {
            Event.Trigger(SettlementEvents.SettlementCalculated, args);
        }

        /// <summary>
        /// 触发结算完成事件
        /// </summary>
        private void TriggerSettlementCompleted(SettlementCompletedEventArgs args)
        {
            Event.Trigger(SettlementEvents.SettlementCompleted, args);
        }

        
        #endregion
    }
}

