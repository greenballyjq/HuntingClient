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
    /// 幸运仪式界面
    /// </summary>
    public class UILucky : UIBase
    {
        /// <summary>
        /// 3币数量文本
        /// </summary>
        [SerializeField] private Text _textThreeCoinAmount;

        /// <summary>
        /// 关闭按钮
        /// </summary>
        [SerializeField] private Button _buttonClose;

        /// <summary>
        /// 礼包组件列表
        /// </summary>
        [SerializeField] private UIComponentGift[] _uiComponentGifts;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 玩家数据管理器
        /// </summary>
        private PlayerDataManager PlayerData => GameServiceLocator.GetGameManager<PlayerDataManager>();

        private void Awake()
        {
            _buttonClose.onClick.AddListener(OnClickClose);
        }

        private void OnDestroy()
        {
            _buttonClose.onClick.RemoveListener(OnClickClose);
        }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            foreach (var gift in _uiComponentGifts)
                gift.Init();

            UpdateThreeCoinDisplay(PlayerData.GetThreeKPCoin());

            Event.AddListener(PlayerDataEvents.ThreeKPCoinChanged, OnThreeKPCoinChanged);
        }

        public override void OnClose()
        {
            foreach (var gift in _uiComponentGifts)
                gift.CleanUp();

            Event.RemoveListener(PlayerDataEvents.ThreeKPCoinChanged, OnThreeKPCoinChanged);

            base.OnClose();
        }

        #region 私有方法
        /// <summary>
        /// 更新3币显示
        /// </summary>
        /// <param name="coinAmount">3币数量</param>
        private void UpdateThreeCoinDisplay(int coinAmount)
        {
            _textThreeCoinAmount.text = coinAmount.ToString();
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 关闭按钮点击回调
        /// </summary>
        private void OnClickClose()
        {
            Close();
        }

        /// <summary>
        /// 3币数量改变事件回调
        /// </summary>
        private void OnThreeKPCoinChanged(ThreeKPCoinChangedEventArgs args)
        {
            UpdateThreeCoinDisplay(args.CurrentAmount);
        }
        #endregion
    }
}
