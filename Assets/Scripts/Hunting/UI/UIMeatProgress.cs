using Cysharp.Threading.Tasks;
using GameFramework.Core;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;


namespace Hunting.UI
{
    /// <summary>
    /// 肉度条UI控制器 - 负责显示和管理肉度条的UI表现
    /// </summary>
    public class UIMeatProgress : UIBase
    {
        #region UI组件引用
        [Header("UI组件引用")]
        [SerializeField] private Slider sliderProgress;    // 进度条Slider组件
        [SerializeField] private Image fillImage;          // 进度条Fill图像（用于变色）
        [SerializeField] private Text txtProgress;         // 进度文字显示 "当前进度/总需求"
        [SerializeField] private Text txtProgressAmount;   // 条数文字显示 "X条"
        #endregion

        #region 进度条颜色配置
        [Header("进度条颜色配置")]
        [SerializeField] private Color colorGreen = Color.green;   // 0-2条时的颜色
        [SerializeField] private Color colorYellow = Color.yellow; // 第3条时的颜色  
        [SerializeField] private Color colorRed = Color.red;       // 第4-5条时的颜色
        #endregion

        #region 初始化
        /// <summary>
        /// 事件中心
        /// </summary>
        private EventManager _eventManager;

        /// <summary>
        /// Start生命周期 - 初始化UI并订阅事件
        /// </summary>
        private async void Start()
        {
            InitializeUI();
            await SubscribeToEventsAsync();
        }

        /// <summary>
        /// 初始化UI组件状态
        /// </summary>
        private void InitializeUI()
        {
            // 确保必要的UI组件都存在
            if (sliderProgress == null)
            {
                Debug.LogError("[UIMeatProgress] 缺少sliderProgress引用！");
                return;
            }

            // 如果没有单独指定fillImage，尝试从sliderProgress中获取
            if (fillImage == null)
            {
                fillImage = sliderProgress.fillRect?.GetComponent<Image>();
                if (fillImage == null)
                {
                    Debug.LogError("[UIMeatProgress] 无法找到进度条Fill图像组件！");
                }
            }

            // 设置进度条初始值
            sliderProgress.minValue = 0f;
            sliderProgress.maxValue = 1f;
            sliderProgress.value = 0f;

            // 初始化文字显示
            UpdateProgressText(0f, 200f); // 默认值，实际会从配置读取
            UpdateAmountText(0);
        }

        /// <summary>
        /// 订阅肉度条事件
        /// </summary>
        private async UniTask SubscribeToEventsAsync()
        {
            await GameServiceLocator.WaitForInitialization();

            _eventManager = GameServiceLocator.Event;

            if (_eventManager == null)
            {
                Debug.LogError("[UIMeatProgress] 事件中心未准备好，无法订阅肉度条事件");
                return;
            }

            _eventManager.AddListener(MeatEvents.MeatProgressChanged, OnMeatProgressChanged);
            _eventManager.AddListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
            _eventManager.AddListener(MeatEvents.MeatMaxBarsReached, OnMeatMaxBarsReached);

            if (MeatProgressManager.Instance != null)
            {
                var (currentProgress, completedBars, _) = MeatProgressManager.Instance.GetProgressInfo();
                UpdateDisplay(currentProgress, completedBars);
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 处理进度变化事件
        /// </summary>
        private void OnMeatProgressChanged(MeatProgressChangedEventArgs args)
        {
            UpdateDisplay(args.CurrentProgress, args.CompletedBars);
        }

        /// <summary>
        /// 处理条数变化事件
        /// </summary>
        private void OnMeatBarCountChanged(MeatBarCountChangedEventArgs args)
        {
            UpdateAmountText(args.CurrentBars);
            UpdateProgressBarColor(args.CurrentBars);
        }

        /// <summary>
        /// 处理达到最大条数事件
        /// </summary>
        private void OnMeatMaxBarsReached()
        {
            Debug.Log("[UIMeatProgress] 肉度条已满！");
        }
        #endregion

        #region UI更新方法
        /// <summary>
        /// 更新整体显示
        /// </summary>
        /// <param name="currentProgress">当前条进度值</param>
        /// <param name="completedBars">已完成条数</param>
        private void UpdateDisplay(float currentProgress, int completedBars)
        {
            // 获取配置的单条所需值
            float requiredPerBar = MeatProgressManager.Instance?.RequiredPerBar ?? 200f;

            // 更新进度条
            UpdateProgressBar(currentProgress, requiredPerBar);

            // 更新进度文字
            UpdateProgressText(currentProgress, requiredPerBar);

            // 更新条数文字
            UpdateAmountText(completedBars);

            // 更新进度条颜色（基于已完成条数）
            UpdateProgressBarColor(completedBars);
        }

        /// <summary>
        /// 更新进度条Slider的值
        /// </summary>
        /// <param name="currentProgress">当前进度值</param>
        /// <param name="requiredPerBar">单条所需值</param>
        private void UpdateProgressBar(float currentProgress, float requiredPerBar)
        {
            if (sliderProgress != null)
            {
                float progressPercent = currentProgress / requiredPerBar;
                sliderProgress.value = progressPercent;
            }
        }

        /// <summary>
        /// 更新进度文字显示
        /// </summary>
        /// <param name="currentProgress">当前进度值</param>
        /// <param name="requiredPerBar">单条所需值</param>
        private void UpdateProgressText(float currentProgress, float requiredPerBar)
        {
            if (txtProgress != null)
            {
                // 显示格式："当前进度/总需求"，例如："150/200"
                txtProgress.text = $"{currentProgress:F0}/{requiredPerBar:F0}";
            }
        }

        /// <summary>
        /// 更新条数文字显示
        /// </summary>
        /// <param name="completedBars">已完成条数</param>
        private void UpdateAmountText(int completedBars)
        {
            if (txtProgressAmount != null)
            {
                // 显示格式："X条"，例如："3条"
                txtProgressAmount.text = $"{completedBars}条";
            }
        }

        /// <summary>
        /// 根据已完成条数更新进度条颜色
        /// </summary>
        /// <param name="completedBars">已完成条数</param>
        private void UpdateProgressBarColor(int completedBars)
        {
            if (fillImage == null) return;

            // 根据条数决定颜色：
            // 0-2条：绿色
            // 第3条：黄色  
            // 第4-5条：红色
            Color targetColor = completedBars switch
            {
                < 2 => colorGreen,   // 0,1,条 - 绿色
                < 4 => colorYellow,    // 第3条 - 黄色
                _ => colorRed        // 4,5条 - 红色
            };

            fillImage.color = targetColor;
        }
        #endregion

        #region 清理
        /// <summary>
        /// OnDestroy生命周期 - 取消事件订阅
        /// </summary>
        private void OnDestroy()
        {
            if (_eventManager != null)
            {
                _eventManager.RemoveListener(MeatEvents.MeatProgressChanged, OnMeatProgressChanged);
                _eventManager.RemoveListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
                _eventManager.RemoveListener(MeatEvents.MeatMaxBarsReached, OnMeatMaxBarsReached);
            }
        }
        #endregion
    }
}