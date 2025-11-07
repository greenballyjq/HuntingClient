using Cysharp.Threading.Tasks;
using GameFramework.Core;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 游玩界面主面板
    /// </summary>
    public class UIHuntingGamePlay : UIBase
    {
        [SerializeField] private Button buttonSettlement;

        /// <summary>
        /// 结算管理器
        /// </summary>
        private SettlementRewardManager SettlementManager => GameServiceLocator.GetGameManager<SettlementRewardManager>();

        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager UI => GameServiceLocator.UI;

        #region 生命周期
        private void Awake()
        {
            buttonSettlement.onClick.AddListener(OnSettlementButtonClicked);
        }

        private void OnDestroy()
        {
            buttonSettlement.onClick.RemoveListener(OnSettlementButtonClicked);
        }
        #endregion

        #region UI回调
        /// <summary>
        /// 结算按钮点击回调
        /// </summary>
        private async void OnSettlementButtonClicked()
        {
            SettlementManager.StartSettlement();
            await OpenSettlementWindowAsync();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 打开结算面板
        /// </summary>
        private async UniTask OpenSettlementWindowAsync()
        {
            await UI.OpenUIAsync<UISettlement>("UISettlement");
        }
        #endregion
    }
}
