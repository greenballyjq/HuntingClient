using cfg;
using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using Hunting.Manager;
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
        private EventManager _eventManager;
        #endregion

        #region 配置数据
        /// <summary>
        /// 默认使用的肉度条配置ID
        /// </summary>
        [SerializeField]
        private int meatProgressConfigId = 1;

        /// <summary>
        /// 肉度条配置数据
        /// </summary>
        private cfg.HuntingConfig.MeatProgress meatProgressData;
        /// <summary>
        /// 单条所需值
        /// </summary>
        public float RequiredPerBar => meatProgressData?.RequiredPerBar ?? 200f;
        /// <summary>
        /// 最大条数
        /// </summary>
        public int MaxBars => meatProgressData?.MaxBar ?? 5;


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
            // 等待框架与服务初始化
            await GameServiceLocator.WaitForInitialization();

            _eventManager = GameServiceLocator.Event;

            // 等待配置管理器初始化完成
            while (HuntingGameConfigManager.Instance == null || !HuntingGameConfigManager.Instance.Initialized)
            {
                await UniTask.Delay(10);
            }

            // 从配置管理器获取肉度条配置数据
            meatProgressData = HuntingGameConfigManager.Instance.GetMeatProgress(meatProgressConfigId);
            if (meatProgressData == null)
            {
                Debug.LogError("[MeatProgressManager] 无法获取肉度条配置数据");
                return;
            }

            // 订阅动物掉落奖励事件
            _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);

            // 标记初始化完成
            initialized = true;
            Debug.Log($"[MeatProgressManager] 初始化完成，单条所需: {RequiredPerBar}，最大条数: {MaxBars}");

            // 触发初始状态事件，通知UI更新
            TriggerMeatProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentProgress = currentBarProgress,
                CompletedBars = currentCompletedBars
            });
        }

        /// <summary>
        /// OnDestroy生命周期 - 清理资源，取消事件订阅
        /// </summary>
        private void OnDestroy()
        {
            // 取消订阅动物掉落奖励事件
            if (initialized && _eventManager != null)
            {
                _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
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
            while (currentBarProgress >= RequiredPerBar && !IsMaxBarsReached)
            {
                // 完成一条：扣除所需进度，增加完成条数
                currentBarProgress -= RequiredPerBar;
                currentCompletedBars++;

                Debug.Log($"[MeatProgressManager] 完成一条肉度条！当前条数: {currentCompletedBars}/{MaxBars}");
            }

            // 触发进度变化事件，通知所有监听者
            TriggerMeatProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentProgress = currentBarProgress,
                CompletedBars = currentCompletedBars
            });

            // 如果完成了新条，触发条数变化事件
            if (currentCompletedBars != oldBars)
            {
                TriggerMeatBarCountChanged(new MeatBarCountChangedEventArgs
                {
                    Sender = this,
                    CurrentBars = currentCompletedBars
                });
            }

            // 检查是否首次达到最大条数
            if (IsMaxBarsReached && oldBars < MaxBars)
            {
                TriggerMeatMaxBarsReached();
                Debug.LogWarning("[MeatProgressManager] 已达到最大肉度条数！");
            }
        }

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
        /// 测试方法：重置进度
        /// </summary>
        [ContextMenu("测试重置进度")]
        public void TestResetProgress()
        {
            currentBarProgress = 0;
            currentCompletedBars = 0;
            // 触发事件通知UI重置显示
            TriggerMeatProgressChanged(new MeatProgressChangedEventArgs
            {
                Sender = this,
                CurrentProgress = currentBarProgress,
                CompletedBars = currentCompletedBars
            });

            TriggerMeatBarCountChanged(new MeatBarCountChangedEventArgs
            {
                Sender = this,
                CurrentBars = currentCompletedBars
            });
            Debug.Log("[MeatProgressManager] 进度已重置");
        }
        #endregion

        #region 事件触发
        /// <summary>
        /// 触发肉度条进度变化事件
        /// </summary>
        private void TriggerMeatProgressChanged(MeatProgressChangedEventArgs args)
        {
            if (_eventManager == null) return;

            _eventManager.Trigger(MeatEvents.MeatProgressChanged, args);
        }

        /// <summary>
        /// 触发肉度条数量变化事件
        /// </summary>
        private void TriggerMeatBarCountChanged(MeatBarCountChangedEventArgs args)
        {
            if (_eventManager == null) return;

            _eventManager.Trigger(MeatEvents.MeatBarCountChanged, args);
        }

        /// <summary>
        /// 触发肉度条达到上限事件
        /// </summary>
        private void TriggerMeatMaxBarsReached()
        {
            if (_eventManager == null) return;

            _eventManager.Trigger(MeatEvents.MeatMaxBarsReached);
        }
        #endregion
    }
}