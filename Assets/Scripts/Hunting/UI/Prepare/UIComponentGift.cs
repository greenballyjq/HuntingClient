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
        }

        public void CleanUp()
        {
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
            int currentCoin = PlayerData.GetThreeKPCoin();
            if (currentCoin < _giftPrice)
            {
                // TODO: 看广告逻辑
                return;
            }

            PlayerData.UpdateThreeKPCoin(-_giftPrice);

            LuckyBuff buff = Config.GetLuckyBuffFromGift(_giftType);
                
            TriggerGiftOpened(new GiftOpenedEventArgs
            {
                GiftType = _giftType,
                LuckyBuffData = buff
            });

            Debug.Log($"[UIComponentGift] 抽中 Buff: 类型={buff.LuckyBuffType}, 强度={buff.LuckyBuffStrengthType}, ID={buff.ID}");
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
        /// 触发礼包开启成功事件
        /// </summary>
        private void TriggerGiftOpened(GiftOpenedEventArgs args)
        {
            Event.Trigger(LuckyBuffEvents.GiftOpened, args);
        }
        #endregion
    }
}
