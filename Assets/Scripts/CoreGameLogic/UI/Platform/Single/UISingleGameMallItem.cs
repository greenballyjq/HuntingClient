using System;
using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Network.Models.Vo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGameLogic.UI.Platform.Single
{
    /// <summary>
    /// 单个实物商城物品UI
    /// </summary>
    public class UISingleGameMallItem : UIBase
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
        
        /// <summary>
        /// 金币购买Transform
        /// </summary>
        [SerializeField] private GameObject coinPriceObj;
        
        /// <summary>
        /// 看广告获取Transform
        /// </summary>
        [SerializeField] private GameObject adPriceObj;

        /// <summary>
        /// 冷却时间obj
        /// </summary>
        [SerializeField] private GameObject cooldownObj;

        /// <summary>
        /// 冷却时间text
        /// </summary>
        [SerializeField] private TextMeshProUGUI cooldownText;
        
        private MallItemVo _mallItemVo;
        private MallManager _mallManager;
        private IPlatform _platform;
        
        private DateTime _cooldownCompleteDateTime;
        
        private void Start()
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
            _platform = GameServiceLocator.PlatformManager.CurrentPlatform;
            _mallManager = GameServiceLocator.GetAppManager<MallManager>();
        }

        public void SetMallItem(MallItemVo mallItemVo)
        {
            _mallItemVo = mallItemVo;
            mallItemNameText.text = mallItemVo.Name;
            priceText.text = $"x{mallItemVo.PriceCoin}";
            mallItemImage.sprite = mallItemVo.Sprite;

            if (mallItemVo.IsVideoReward)
            {
                RefreshCooldownAsync().Forget();
            }
        }

        private async UniTask RefreshCooldownAsync()
        {
            MallManager mallManager = GameServiceLocator.GetAppManager<MallManager>();
            await mallManager.RefreshMallItemCooldown();
            Debug.Log($"[SingleMallItemUI] 重新刷新商品冷却");
            ulong mallItemCooldown = mallManager.GetItemCooldown(_mallItemVo.ItemID);
            
            _cooldownCompleteDateTime = DateTimeOffset.FromUnixTimeSeconds((long)mallItemCooldown).LocalDateTime;
            DateTime now = DateTime.Now;
            
            Debug.Log($"[SingleMallItemUI] {_mallItemVo.ItemID} 刷新商品冷却：{_cooldownCompleteDateTime}");
            
            if (DateTime.Compare(_cooldownCompleteDateTime, now) < 0)
            {
                // 冷却结束
                cooldownObj.SetActive(false);
            }
            else
            {
                // 冷却未结束
                cooldownObj.SetActive(true);
                TimeSpan timeSpan = _cooldownCompleteDateTime - now;
                int leftMinutes = timeSpan.Minutes;
                cooldownText.text = $"冷却中:{leftMinutes}分钟";

                adPriceObj.gameObject.SetActive(false);
                coinPriceObj.gameObject.SetActive(false);
            }
            
            if (DateTime.Compare(_cooldownCompleteDateTime, now) < 0)
            {
                if (_mallItemVo.IsVideoReward)
                {
                    adPriceObj.gameObject.SetActive(true);
                    coinPriceObj.gameObject.SetActive(false);
                    mallItemNameText.text = $"免费{_mallItemVo.PriceCoin}金";
                }
                else
                {
                    adPriceObj.gameObject.SetActive(false);
                    coinPriceObj.gameObject.SetActive(true);
                }
            }
        }
        
        private async void OnPurchaseButtonClick()
        {
            if (_mallItemVo.IsVideoReward)
            {
                if (DateTime.Compare(_cooldownCompleteDateTime, DateTime.Now) > 0)
                {
                    // 在冷却中，弹出提示
                    _platform.ShowToast("冷却中");
                    return;
                }
                // todo IAA 逻辑待实现
                
                // GooseAdManager gooseAdManager = GooseCatcherGame.Instance.GetGameManager<GooseAdManager>();
                // gooseAdManager.ShowAdFromType(AdType.Reward, GooseAdData.GooseAdType.Ad_Mall, OnMallAdClose);
            }
            else
            {
                int priceCoin = _mallItemVo.PriceCoin;
                bool enoughCoin = GameServiceLocator.GetAppManager<PlayerDataManager>().EnoughThreeKp(priceCoin);
                if (enoughCoin)
                {
                    UIManager uiManager = GameServiceLocator.UIManager;
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
        }

        private async void EnoughCoinCallback(UIDialog uiDialog)
        {
            uiDialog.Hide();
            bool success = await _mallManager.BuyItem(_mallItemVo.ItemID, 1);
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