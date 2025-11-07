using GameFramework.Core;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 结算弹窗
    /// </summary>
    public class UISettlement : UIBase
    {
        [SerializeField] private Text _textCoin;
        [SerializeField] private Text _textMastery;
        [SerializeField] private Button _buttonSettlement;
        [SerializeField] private Button _buttonDoubleSettlement;

        private SettlementCalculatedEventArgs _currentData;
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 结算管理器
        /// </summary>
        private SettlementRewardManager SettlementManager => GameServiceLocator.GetGameManager<SettlementRewardManager>();

        private void Awake()
        {
            _buttonSettlement.onClick.AddListener(OnSettlementButtonClicked);
            _buttonDoubleSettlement.onClick.AddListener(OnDoubleSettlementButtonClicked);
        }

        private void OnEnable()
        {
            Event.AddListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
            Event.AddListener(SettlementEvents.SettlementCompleted, OnSettlementCompleted);

            // 开始结算
            SettlementManager.StartSettlement();
        }

        private void OnDisable()
        {
            Event.RemoveListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
            Event.RemoveListener(SettlementEvents.SettlementCompleted, OnSettlementCompleted);
        }

        private void OnDestroy()
        {
            _buttonSettlement.onClick.RemoveListener(OnSettlementButtonClicked);
            _buttonDoubleSettlement.onClick.RemoveListener(OnDoubleSettlementButtonClicked);
        }

        #region UI回调
        /// <summary>
        /// 普通结算按钮
        /// </summary>
        private void OnSettlementButtonClicked()
        {
            SettlementManager.CompleteSettlement();
        }

        /// <summary>
        /// 看广告翻倍按钮
        /// </summary>
        private void OnDoubleSettlementButtonClicked()
        {
            SettlementManager.ApplyDouble();
        }
        #endregion

        #region 事件回调
        /// <summary>
        /// 结算数据更新
        /// </summary>
        private void OnSettlementCalculated(SettlementCalculatedEventArgs args)
        {
            _currentData = args;
            UpdateDisplay(args);
            UpdateDoubleButtonState(args);
        }

        /// <summary>
        /// 结算完成
        /// </summary>
        private void OnSettlementCompleted(SettlementCompletedEventArgs args)
        {
            Close();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 更新界面显示
        /// </summary>
        private void UpdateDisplay(SettlementCalculatedEventArgs args)
        {
            _textCoin.text = args.TotalCoin.ToString();
            _textMastery.text = args.BaseMastery.ToString();
        }

        /// <summary>
        /// 根据结算状态更新翻倍按钮显示
        /// </summary>
        private void UpdateDoubleButtonState(SettlementCalculatedEventArgs args)
        {
            _buttonDoubleSettlement.gameObject.SetActive(!args.IsDoubleApplied);
        }
        #endregion
    }
}
