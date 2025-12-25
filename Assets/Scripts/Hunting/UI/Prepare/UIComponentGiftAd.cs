using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 广告礼包组件
    /// </summary>
    public class UIComponentGiftAd : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 礼包类型
        /// </summary>
        [SerializeField] private ELuckyGiftType _giftType;

        /// <summary>
        /// 礼包名称文本
        /// </summary>
        [SerializeField] private Text _textGiftName;

        /// <summary>
        /// 购买按钮
        /// </summary>
        [SerializeField] private Button _buttonGift;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        private void Awake()
        {
            _buttonGift.onClick.AddListener(OnGiftButtonClicked);
        }

        private void OnDestroy()
        {
            _buttonGift.onClick.RemoveListener(OnGiftButtonClicked);
        }

        public void Init()
        {
            InitializeGift();

            Event.AddListener(LuckyBuffEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
            Event.AddListener(LuckyBuffEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);
        }

        public void CleanUp()
        {
            Event.RemoveListener(LuckyBuffEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
            Event.RemoveListener(LuckyBuffEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);
        }

        #region 私有方法
        /// <summary>
        /// 初始化礼包
        /// </summary>
        private void InitializeGift()
        {
            LuckyGift gift = Config.GetLuckyGift(_giftType);
            _textGiftName.text = gift.Name;
        }

        /// <summary>
        /// 开启礼包
        /// </summary>
        private void OpenGift()
        {
            LuckyBuff buff = Config.GetLuckyBuffFromGift(_giftType);

            TriggerGiftOpened(new GiftOpenedEventArgs
            {
                GiftType = _giftType,
                LuckyBuffData = buff
            });
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 礼包按钮点击回调
        /// </summary>
        private void OnGiftButtonClicked()
        {
            OpenGift();
        }

        /// <summary>
        /// 礼包开启动画开始事件回调
        /// </summary>
        private void OnGiftOpenAnimationStarted()
        {
            _buttonGift.interactable = false;
        }

        /// <summary>
        /// 礼包开启动画结束事件回调
        /// </summary>
        private void OnGiftOpenAnimationEnded(GiftOpenAnimationEndedEventArgs args)
        {
            _buttonGift.interactable = true;
        }

        /// <summary>
        /// 触发礼包开启成功事件
        /// </summary>
        private void TriggerGiftOpened(GiftOpenedEventArgs args)
        {
            Event.Trigger(LuckyBuffEvents.GiftOpened, args);
        }
        #endregion
    }
}


