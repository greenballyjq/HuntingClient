using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 游玩界面主面板
    /// </summary>
    public class UIHuntingGameplay : UIBase
    {
        /// <summary>
        /// 结算按钮
        /// </summary>
        [SerializeField] private Button buttonSettlement;

        /// <summary>
        /// 肉度条组件
        /// </summary>
        [SerializeField] private UIMeatProgress uiComponentMeatProgress;

        /// <summary>
        /// 能量条组件
        /// </summary>
        [SerializeField] private UIEnergyProgress uiComponentEnergyProgress;

        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager UI => GameServiceLocator.UI;

        /// <summary>
        /// 结算管理器
        /// </summary>
        private SettlementRewardManager SettlementManager => GameServiceLocator.GetGameManager<SettlementRewardManager>();

        private void Awake()
        {
            buttonSettlement.onClick.AddListener(OnSettlementButtonClicked);
        }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            uiComponentMeatProgress.Init();
            uiComponentEnergyProgress.Init();
        }

        public override void OnClose()
        {
            uiComponentEnergyProgress.CleanUp();
            uiComponentMeatProgress.CleanUp();
            base.OnClose();
        }

        private void OnDestroy()
        {
            buttonSettlement.onClick.RemoveListener(OnSettlementButtonClicked);
        }

        #region 私有方法
        /// <summary>
        /// 打开结算面板
        /// </summary>
        private async UniTask OpenSettlementWindowAsync()
        {
            await UI.OpenUIAsync<UISettlement>("UISettlement");
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 结算按钮点击回调
        /// </summary>
        private async void OnSettlementButtonClicked()
        {
            // 打开结算弹窗
            await OpenSettlementWindowAsync();

            // 通知结算管理器计算本局奖励
            SettlementManager.StartSettlement();
        }
        #endregion
    }
}
