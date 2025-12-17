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
        /// 礼包付费方式
        /// </summary>
        private ELuckyGiftCostType _giftCostType;

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
        private PlayerDataManager PlayerData => GameServiceLocator.GetGameManager<PlayerDataManager>();

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

            UpdateGiftButtonInteractableByCost(PlayerData.GetThreeKPCoin());
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
            LuckyGift gift = Config.GetLuckyGift(_giftType);

            _giftPrice = gift.ThreeKPCoin;
            _giftCostType = gift.CostType;

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

            // 3币购买的礼包：检查并扣除3币
            if (_giftCostType == ELuckyGiftCostType.ThreeKPCoin)
            {
                int currentCoin = PlayerData.GetThreeKPCoin();
                if (currentCoin < _giftPrice)
                {
                    // 正常情况下按钮已被置灰，这里只是防御性返回
                    return;
                }

                PlayerData.UpdateThreeKPCoin(-_giftPrice);
            }
            else if (_giftCostType == ELuckyGiftCostType.Ad)
            {
                // TODO: 将来接入看广告逻辑
                // 当前版本：视为广告已观看，直接获得增益
            }

            // 触发礼包开启事件
            TriggerGiftOpened(new GiftOpenedEventArgs
            {
                GiftType = _giftType,
                LuckyBuffData = buff
            });
        }

        /// <summary>
        /// 根据付费方式和当前3币数量更新礼包按钮可点击状态
        /// </summary>
        /// <param name="currentCoin">当前3币数量</param>
        private void UpdateGiftButtonInteractableByCost(int currentCoin)
        {
            if (_giftCostType == ELuckyGiftCostType.ThreeKPCoin)
            {
                _buttonGift.interactable = currentCoin >= _giftPrice;
            }
            else
            {
                _buttonGift.interactable = true;
            }
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
            UpdateGiftButtonInteractableByCost(PlayerData.GetThreeKPCoin());
        }

        /// <summary>
        /// 3币数量改变事件回调
        /// </summary>
        /// 
        private void OnThreeKPCoinChanged(ThreeKPCoinChangedEventArgs args)
        {
            UpdateGiftButtonInteractableByCost(args.CurrentAmount);
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
