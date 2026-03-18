using System.Collections.Generic;
using CoreGameLogic.Managers.AppManagers;
using CoreGameLogic.UI.Platform.Single;
using GameFramework.Core.UI;
using GameFramework.Network.Models.Vo;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGameLogic.UI.Platform
{
    /// <summary>
    /// 实物兑换商店界面
    /// </summary>
    public class UIPhysicalMall : UIBase
    {
        /// <summary>
        /// 关闭按钮
        /// </summary>
        [SerializeField] private Button closeButton;

        /// <summary>
        /// 单个实物商城物品UI预制体
        /// </summary>
        [SerializeField] private GameObject uiSinglePhysicalMallItemPrefab;
        
        /// <summary>
        /// 单个实物商城物品UI父物体
        /// </summary>
        [SerializeField] private Transform uiSinglePhysicalMallItemParent;
        
        private List<MallItemVo> _physicalMallItemVoList;
        
        public override void OnInit(object userData)
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
            base.OnInit(userData);
        }
        
        protected override void OnShow()
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
            SpawnMallData();
            base.OnShow();
        }

        protected override void OnHide()
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClick);
            base.OnHide();
        }

        private void SpawnMallData()
        {
            MallManager mallManager = GameServiceLocator.GetAppManager<MallManager>();
            _physicalMallItemVoList = mallManager.GetPhysicalMallItemVoList();
            
            foreach (Transform child in uiSinglePhysicalMallItemParent)
            {
                Destroy(child.gameObject);
            }
            
            if (_physicalMallItemVoList == null || _physicalMallItemVoList.Count == 0) return;
            
            foreach (var mallItemData in _physicalMallItemVoList)
            {
                // Instantiate and set up the propsPurchaseItemUIPrefab with the mallData
                GameObject propsPurchaseItemUI = Instantiate(uiSinglePhysicalMallItemPrefab, uiSinglePhysicalMallItemParent);
                propsPurchaseItemUI.GetComponent<UISinglePhysicalMallItem>().SetMallItem(mallItemData);
            }
        }

        private void OnCloseButtonClick()
        {
            Hide();
        }
    }
}