using System;
using System.Collections.Generic;
using CoreGameLogic.Managers.AppManagers;
using GameFramework.Network.Models.Vo;
using GameFramework.Network.Proxy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGameLogic.UI.Platform.Single
{
    /// <summary>
    /// 单个实物商城物品UI
    /// </summary>
    public class UISinglePhysicalMallItem : MonoBehaviour
    {
        /// <summary>
        /// 商城物品名text
        /// </summary>
        [SerializeField] private TextMeshProUGUI mallItemNameText;

        /// <summary>
        /// 商城物品图Image
        /// </summary>
        [SerializeField] private Image mallItemImage;

        /// <summary>
        /// 价格text
        /// </summary>
        [SerializeField] private TextMeshProUGUI priceText;

        /// <summary>
        /// 购买按钮
        /// </summary>
        [SerializeField] private Button purchaseButton;
        
        private MallItemVo _mallItemVo;
        private IPlatform _platform;
        
        private void Start()
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
            _platform = GameServiceLocator.PlatformManager.CurrentPlatform;
        }
        
        public void SetMallItem(MallItemVo mallItemVo)
        {
            _mallItemVo = mallItemVo;
            mallItemNameText.text = mallItemVo.Name;
            priceText.text = $"x{mallItemVo.PriceCoin}";
            // _propsType = GetPropsTypeFromName(mallItemVo.Name);
            mallItemImage.sprite = mallItemVo.Sprite;
        }

        private async void OnPurchaseButtonClick()
        {
            UIManager uiManager = GameServiceLocator.UIManager;
            
            int priceCoin = _mallItemVo.PriceCoin;
            bool enoughCoin = GameServiceLocator.GetAppManager<PlayerDataManager>().EnoughThreeKp(priceCoin);
            if (enoughCoin)
            {
                Action<UIDialog> callback = EnoughCoinCallback;
                UIDialog uiDialog = await uiManager.OpenUIAsync<UIDialog>(nameof(UIDialog));
                uiDialog.SetDialog("是否确认购买?", _mallItemVo.Name, _mallItemVo.Description, dialog =>
                {
                    dialog.Hide();
                }, callback);
                uiDialog.Show();
            }
            else
            {
                _platform.ShowToast("三币余额不足", "error");
            }
        }

        private async void EnoughCoinCallback(UIDialog uiDialog)
        {
            MallManager mallManager = GameServiceLocator.GetAppManager<MallManager>();
            List<AddressInfo> addressInfos = await UserProxy.Instance.GetAddresses();
            if (addressInfos.Count == 0)
            {
                // todo 没有物流信息，弹出物流信息页面
                    
                _platform.ShowToast("购买失败", "error");
                return;
            }
            uiDialog.Hide();
            bool success = await mallManager.BuyPhysicalItem(_mallItemVo.ItemID, 1);
            if (success)
            {
                _platform.ShowToast("购买成功", "success");
            }
            else
            {
                _platform.ShowToast("购买失败", "error");
            }
        }
    }
}