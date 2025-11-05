using cfg;
using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using Hunting.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 回血肉进度条管理器
    /// </summary>
    public class MeatProgressManager : MonoBehaviour
    {
        #region 单例模式
        private static MeatProgressManager _instance;
        public static MeatProgressManager Instance => _instance;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        #endregion

        #region 服务引用
        /// <summary>
        /// 事件中心
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;
        #endregion

        #region 配置数据
        /// <summary>
        /// 肉度条配置数据
        /// </summary>
        private cfg.HuntingConfig.Progress meatProgressData;
        /// <summary>
        /// 单条所需值
        /// </summary>
        public float RequiredPerBar => meatProgressData?.RequiredPerBar ?? 200f;
        /// <summary>
        /// 最大条数
        /// </summary>
        public int MaxBars => meatProgressData?.MaxBars ?? 5;
        #endregion

        #region 运行时状态
        /// <summary>
        /// 当前条积攒量
        /// </summary>
        private float currentBarProgress;
        /// <summary>
        /// 当前已攒满的条数
        /// </summary>
        private int currentCompletedBars;
        /// <summary>
        /// 是否达到条数上限
        /// </summary>
        public bool IsMaxBarsReached => currentCompletedBars >= MaxBars;
        /// <summary>
        /// 当前进度百分比
        /// </summary>
        public float CurrentProgressPercent => currentBarProgress / RequiredPerBar;
        #endregion

        #region 事件系统
        /// <summary>
        /// 进度变化事件 - 当肉度条进度发生变化时触发
        ///   float - 当前条进度值
        ///   int - 当前已完成条数
        ///   bool - 是否新完成了一条
        /// </summary>
        public event Action<float, int, bool> OnProgressChanged;
        /// <summary>
        /// 条数变化事件 - 当肉度条数量发生变化时触发
        ///   int - 新的条数
        ///   int - 变化的数量（正数表示增加，负数表示减少）
        /// </summary>
        public event Action<int, int> OnBarsChanged;
        /// <summary>
        /// 达到最大条数事件 - 当肉度条达到最大数量时触发
        /// </summary>
        public event Action OnMaxBarsReached;
        #endregion

        /// <summary>
        /// 初始化状态标志
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Start生命周期 - 开始异步初始化
        /// </summary>
        private void Start()
        {
            InitializeAsync().Forget();
        }
    
        /// <summary>
        /// 异步初始化方法 - 等待配置管理器就绪后完成初始化
        /// </summary>
        private async UniTask InitializeAsync()
        {
            // 等待配置管理器初始化完成
            while (HuntingGameConfigManager.Instance == null || !HuntingGameConfigManager.Instance.Initialized)
            {
                await UniTask.Delay(10);
            }

            // 从配置管理器获取肉度条配置数据
            meatProgressData = HuntingGameConfigManager.Instance.GetProgress(EProgressType.MeatProgress);
            if (meatProgressData == null)
            {
                Debug.LogError("[MeatProgressManager] 无法获取肉度条配置数据");
                return;
            }

            // 订阅动物掉落奖励事件
            Event.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);

            // 标记初始化完成
            initialized = true;
            Debug.Log($"[MeatProgressManager] 初始化完成，单条所需: {RequiredPerBar}，最大条数: {MaxBars}");

            // 触发初始状态事件，通知UI更新
            OnProgressChanged?.Invoke(currentBarProgress, currentCompletedBars, false);
        }

        /// <summary>
        /// OnDestroy生命周期 - 清理资源，取消事件订阅
        /// </summary>
        private void OnDestroy()
        {
            // 取消订阅动物掉落奖励事件
            if (initialized)
            {
                Event.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
            }
        }

        /// <summary>
        /// 处理动物掉落奖励
        /// </summary>
        private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
        {
            // 检查初始化状态和掉落类型
            if (!initialized || args.DropType != EDropType.Meat) 
                return;

            // 处理肉类掉落，增加肉度条进度
            AddMeatProgress(args.Amount);
        }

        /// <summary>
        /// 添加肉类进度
        /// </summary>
        /// <param name="amount">要添加的进度值</param>
        public void AddMeatProgress(float amount)
        {
            // 前置检查：未初始化或已达到最大条数时不处理
            if (!initialized || IsMaxBarsReached) return;

            // 记录添加前的条数，用于比较
            int oldBars = currentCompletedBars;

            // 添加进度到当前条
            currentBarProgress += amount;

            // 检查是否完成了一条或多条
            bool newBarCompleted = false;
            while (currentBarProgress >= RequiredPerBar && !IsMaxBarsReached)
            {
                // 完成一条：扣除所需进度，增加完成条数
                currentBarProgress -= RequiredPerBar;
                currentCompletedBars++;
                newBarCompleted = true;

                Debug.Log($"[MeatProgressManager] 完成一条肉度条！当前条数: {currentCompletedBars}/{MaxBars}");
            }

            // 触发进度变化事件，通知所有监听者
            OnProgressChanged?.Invoke(currentBarProgress, currentCompletedBars, newBarCompleted);

            // 如果完成了新条，触发条数变化事件
            if (newBarCompleted)
            {
                int barsChange = currentCompletedBars - oldBars;
                OnBarsChanged?.Invoke(currentCompletedBars, barsChange);
            }

            // 检查是否首次达到最大条数
            if (IsMaxBarsReached && oldBars < MaxBars)
            {
                OnMaxBarsReached?.Invoke();
                Debug.LogWarning("[MeatProgressManager] 已达到最大肉度条数！");
            }
        }

        /// <summary>
        /// 结算一条肉度条
        /// </summary>
        /// <returns>结算是否成功</returns>
        public bool SettleOneBar()
        {
            // 检查是否有可结算的条
            if (!initialized || currentCompletedBars <= 0) return false;

            // 减少一条完成条数
            currentCompletedBars--;

            Debug.Log($"[MeatProgressManager] 结算一条肉度条，剩余条数: {currentCompletedBars}");

            // 触发事件通知UI更新
            OnBarsChanged?.Invoke(currentCompletedBars, -1);
            OnProgressChanged?.Invoke(currentBarProgress, currentCompletedBars, false);

            return true;
        }

        /// <summary>
        /// 结算所有肉度条
        /// </summary>
        /// <returns>实际结算的条数</returns>
        public int SettleAllBars()
        {
            // 检查是否有可结算的条
            if (!initialized || currentCompletedBars <= 0) return 0;

            // 记录结算前的条数
            int settledBars = currentCompletedBars;

            // 清空所有完成条数
            currentCompletedBars = 0;

            Debug.Log($"[MeatProgressManager] 结算所有肉度条，共 {settledBars} 条");

            // 触发事件通知UI更新
            OnBarsChanged?.Invoke(0, -settledBars);
            OnProgressChanged?.Invoke(currentBarProgress, 0, false);

            return settledBars;
        }

        #region 状态查询方法
        /// <summary>
        /// 获取当前进度信息 - 获取完整的进度状态
        /// </summary>
        /// <returns>
        /// 元组包含：
        ///   currentProgress - 当前条进度
        ///   completedBars - 已完成条数  
        ///   isMax - 是否达到最大条数
        /// </returns>
        public (float currentProgress, int completedBars, bool isMax) GetProgressInfo()
        {
            return (currentBarProgress, currentCompletedBars, IsMaxBarsReached);
        }

        /// <summary>
        /// 获取当前条数
        /// </summary>
        /// <returns>当前已完成条数</returns>
        public int GetCurrentBarCount() => currentCompletedBars;

        /// <summary>
        /// 获取当前单条进度
        /// </summary>
        /// <returns>当前条进度值</returns>
        public float GetCurrentProgress() => currentBarProgress;

        /// <summary>
        /// 检查是否可以结算
        /// </summary>
        /// <returns>有可结算条时返回true</returns>
        public bool CanSettle() => currentCompletedBars > 0;
        #endregion

        #region 测试和调试方法
        /// <summary>
        /// 测试方法：添加指定进度
        /// </summary>
        [ContextMenu("测试添加50进度")]
        public void TestAddProgress()
        {
            AddMeatProgress(50);
        }

        /// <summary>
        /// 测试方法：结算一条
        /// </summary>
        [ContextMenu("测试结算一条")]
        public void TestSettleOne()
        {
            SettleOneBar();
        }

        /// <summary>
        /// 测试方法：重置进度
        /// </summary>
        [ContextMenu("测试重置进度")]
        public void TestResetProgress()
        {
            currentBarProgress = 0;
            currentCompletedBars = 0;
            // 触发事件通知UI重置显示
            OnProgressChanged?.Invoke(0, 0, false);
            OnBarsChanged?.Invoke(0, 0);
            Debug.Log("[MeatProgressManager] 进度已重置");
        }
        #endregion
    }
}