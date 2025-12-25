using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 礼包组件
    /// </summary>
    public class UIComponentGift : MonoBehaviour, IUIComponent
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
        /// 价格文本
        /// </summary>
        [SerializeField] private Text _textPrice;

        /// <summary>
        /// 购买按钮
        /// </summary>
        [SerializeField] private Button _buttonGift;

        /// <summary>
        /// 礼包价值
        /// </summary>
        private int _giftPrice;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 玩家数据管理器
        /// </summary>
        private PlayerDataManager PlayerData => GameServiceLocator.GetAppManager<PlayerDataManager>();

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
            Event.AddListener(PlayerDataEvents.ThreeKPCoinChanged, OnThreeKPCoinChanged);

            UpdateGiftButtonInteractable(PlayerData.GetThreeKPCoin());
        }

        public void CleanUp()
        {
            Event.RemoveListener(LuckyBuffEvents.GiftOpenAnimationStarted, OnGiftOpenAnimationStarted);
            Event.RemoveListener(LuckyBuffEvents.GiftOpenAnimationEnded, OnGiftOpenAnimationEnded);
            Event.RemoveListener(PlayerDataEvents.ThreeKPCoinChanged, OnThreeKPCoinChanged);
        }

        #region 私有方法
        /// <summary>
        /// 初始化礼包
        /// </summary>
        private void InitializeGift()
        {
            var gift = Config.GetLuckyGift(_giftType);

            _giftPrice = gift.ThreeKPCoin;
            _textGiftName.text = gift.Name;
            _textPrice.text = _giftPrice.ToString();
        }

        /// <summary>
        /// 开启礼包
        /// </summary>
        private void OpenGift()
        {
            // 抽到的幸运仪式增益配置
            LuckyBuff buff = Config.GetLuckyBuffFromGift(_giftType);

            // 扣除3币
            PlayerData.UpdateThreeKPCoin(-_giftPrice);

            // 触发礼包开启事件
            TriggerGiftOpened(new GiftOpenedEventArgs
            {
                GiftType = _giftType,
                LuckyBuffData = buff
            });
        }

        /// <summary>
        /// 更新礼包按钮交互状态
        /// </summary>
        /// <param name="currentCoin">当前3币数量</param>
        private void UpdateGiftButtonInteractable(int currentCoin)
        {
            _buttonGift.interactable = currentCoin >= _giftPrice;
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
            int currentCoin = PlayerData.GetThreeKPCoin();
            UpdateGiftButtonInteractable(currentCoin);
        }

        /// <summary>
        /// 3币数量改变事件回调
        /// </summary>
        /// 
        private void OnThreeKPCoinChanged(ThreeKPCoinChangedEventArgs args)
        {
            UpdateGiftButtonInteractable(args.CurrentAmount);
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
