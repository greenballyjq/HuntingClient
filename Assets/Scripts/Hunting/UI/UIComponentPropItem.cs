using Cysharp.Threading.Tasks;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 道具UI组件
    /// </summary>
    public class UIComponentPropItem : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 道具按钮
        /// </summary>
        [SerializeField] private Button _buttonProp;

        /// <summary>
        /// 加号按钮
        /// </summary>
        [SerializeField] private Button _buttonAdd;

        /// <summary>
        /// 道具图像
        /// </summary>
        [SerializeField] private Image _imageProp;

        /// <summary>
        /// 加号图像
        /// </summary>
        [SerializeField] private Image _imageAdd;

        /// <summary>
        /// 冷却遮罩图像
        /// </summary>
        [SerializeField] private Image _imageCooldownMask;

        /// <summary>
        /// 道具数量文本
        /// </summary>
        [SerializeField] private Text _textCount;

        /// <summary>
        /// 道具类型
        /// </summary>
        [SerializeField] private EPropType _propType;

        /// <summary>
        /// 是否冷却
        /// </summary>
        private bool _isCooldown;

        /// <summary>
        /// 冷却剩余时间
        /// </summary>
        private float _cooldownRemainingTime;

        /// <summary>
        /// 冷却总时间
        /// </summary>
        private float _cooldownTotalTime;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager Resource => GameServiceLocator.Resource;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 道具管理器
        /// </summary>
        private PropManager Prop => GameServiceLocator.GetGameManager<PropManager>();

        public void Init()
        {
            _buttonProp.onClick.AddListener(OnPropIconClicked);
            _buttonAdd.onClick.AddListener(OnAddButtonClicked);

            Event.AddListener(PropEvents.PropStarted, OnPropStarted);
            Event.AddListener(PropEvents.PropUpdated, OnPropUpdated);
            Event.AddListener(PropEvents.PropEnded, OnPropEnded);
            Event.AddListener(PropEvents.PropUseRejected, OnPropUseRejected);

            // 初始化显示
            InitializeDisplay();
        }

        public void CleanUp()
        {
            _buttonProp.onClick.RemoveListener(OnPropIconClicked);
            _buttonAdd.onClick.RemoveListener(OnAddButtonClicked);

            Event.RemoveListener(PropEvents.PropStarted, OnPropStarted);
            Event.RemoveListener(PropEvents.PropUpdated, OnPropUpdated);
            Event.RemoveListener(PropEvents.PropEnded, OnPropEnded);
            Event.RemoveListener(PropEvents.PropUseRejected, OnPropUseRejected);
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        #region 私有方法
        /// <summary>
        /// 初始化显示
        /// </summary>
        private void InitializeDisplay()
        {
            // 加载道具图标
            LoadPropIconAsync().Forget();

            // 更新数量显示
            UpdateCount();

            // 初始化冷却遮罩
            _imageCooldownMask.fillAmount = 0f;
        }

        /// <summary>
        /// 异步加载道具图标
        /// </summary>
        private async UniTask LoadPropIconAsync()
        {
            var propData = Config.GetProp(_propType);
            var sprite = await Resource.LoadAssetAsync<Sprite>(propData.IconResourcePath);
            _imageProp.sprite = sprite;
        }

        /// <summary>
        /// 开始冷却
        /// </summary>
        private void StartCooldown(float duration)
        {
            _isCooldown = true;
            _cooldownTotalTime = duration;
            _cooldownRemainingTime = duration;

            // 禁用按钮
            _buttonProp.interactable = false;

            // 显示遮罩
            _imageCooldownMask.fillAmount = 1f;
        }

        /// <summary>
        /// 更新冷却显示
        /// </summary>
        private void UpdateCooldown(float remainingTime, float totalTime)
        {
            // 计算进度比例
            float progress = remainingTime / totalTime;
            _imageCooldownMask.fillAmount = progress;
        }

        /// <summary>
        /// 更新道具数量显示
        /// </summary>
        private void UpdateCount()
        {
            // TODO: 从PlayerData获取道具数量，目前占位显示
            _textCount.text = "0";
        }

        /// <summary>
        /// 结束冷却
        /// </summary>
        private void EndCooldown()
        {
            _isCooldown = false;
            _cooldownRemainingTime = 0f;
            _cooldownTotalTime = 0f;

            // 启用按钮
            _buttonProp.interactable = true;

            // 隐藏遮罩
            _imageCooldownMask.fillAmount = 0f;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 道具图标按钮点击回调
        /// </summary>
        private void OnPropIconClicked()
        {
            Prop.TryUseProp(_propType);
        }

        /// <summary>
        /// 加号按钮点击回调
        /// </summary>
        private void OnAddButtonClicked()
        {
            // TODO: 实现看广告增加道具逻辑
        }

        /// <summary>
        /// 道具开始事件回调
        /// </summary>
        private void OnPropStarted(PropStartedEventArgs args)
        {
             if (args.PropData.PropType != _propType)
                return;

            // 开始冷却
            StartCooldown(args.PropData.Duration);
        }

        /// <summary>
        /// 道具更新事件回调
        /// </summary>
        private void OnPropUpdated(PropUpdatedEventArgs args)
        {
            if (args.PropData.PropType != _propType)
                return;

            // 更新冷却进度
            _cooldownRemainingTime = args.RemainingTime;
            UpdateCooldown(args.RemainingTime, args.PropData.Duration);
        }

        /// <summary>
        /// 道具结束事件回调
        /// </summary>
        private void OnPropEnded(PropEndedEventArgs args)
        {
            if (args.PropData.PropType != _propType)
                return;

            // 结束冷却
            EndCooldown();

            // 更新数量
            UpdateCount();
        }

        /// <summary>
        /// 道具使用被拒绝事件回调
        /// </summary>
        private void OnPropUseRejected(PropUseRejectedEventArgs args)
        {
            if (args.PropData.PropType != _propType)
                return;

            // TODO：显示提示
        }
        #endregion
    }
}

