using GameFramework.Core;
using GameFramework.Core.UI;
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
        /// <summary>
        /// 金币文本
        /// </summary>
        [SerializeField] private Text textCoin;

        /// <summary>
        /// 熟练度文本
        /// </summary>
        [SerializeField] private Text textMastery;

        /// <summary>
        /// 普通结算按钮
        /// </summary>
        [SerializeField] private Button buttonSettlement;

        /// <summary>
        /// 翻倍结算按钮
        /// </summary>
        [SerializeField] private Button buttonDoubleSettlement;

        /// <summary>
        /// 结算管理器
        /// </summary>
        private SettlementRewardManager SettlementManager => GameServiceLocator.GetRoundManager<SettlementRewardManager>();

        /// <summary>
        /// 事件中心
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        private void Awake()
        {
            buttonSettlement.onClick.AddListener(OnClickSettlement);
            buttonDoubleSettlement.onClick.AddListener(OnClickDoubleSettlement);
        }

        private void OnDestroy()
        {
            buttonSettlement.onClick.RemoveListener(OnClickSettlement);
            buttonDoubleSettlement.onClick.RemoveListener(OnClickDoubleSettlement);
        }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            Event.AddListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
            Event.AddListener(SettlementEvents.SettlementCompleted, OnSettlementCompleted);
        }

        public override void OnClose()
        {
            Event.RemoveListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
            Event.RemoveListener(SettlementEvents.SettlementCompleted, OnSettlementCompleted);
            base.OnClose();
        }

        #region 事件相关
        /// <summary>
        /// 普通结算按钮回调
        /// </summary>
        private void OnClickSettlement()
        {
            Debug.Log("[UISettlement] 玩家选择普通结算");
            SettlementManager.CompleteSettlement();
            Close();
        }

        /// <summary>
        /// 翻倍结算按钮回调
        /// </summary>
        private void OnClickDoubleSettlement()
        {
            SettlementManager.ApplyDouble();
            buttonDoubleSettlement.gameObject.SetActive(false);
        }

        /// <summary>
        /// 结算数据更新
        /// </summary>
        private void OnSettlementCalculated(SettlementCalculatedEventArgs args)
        {
            // 更新界面上的金币与熟练度
            textCoin.text = args.TotalCoin.ToString();
            textMastery.text = args.TotalMastery.ToString();
        }

        /// <summary>
        /// 结算完成
        /// </summary>
        private void OnSettlementCompleted(SettlementCompletedEventArgs args)
        {
            Debug.Log($"[UISettlement] 结算完成 金币:{args.TotalCoin} 熟练度:{args.TotalMastery}");
        }
        #endregion
    }
}
