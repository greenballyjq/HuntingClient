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
    /// 道具商店界面
    /// </summary>
    public class UIGameMall : UIBase
    {
        /// <summary>
        /// 关闭按钮
        /// </summary>
        [SerializeField] private Button closeButton;

        /// <summary>
        /// 单个道具商城物品UI预制体
        /// </summary>
        [SerializeField] private GameObject uiSingleGameMallItemPrefab;
        
        /// <summary>
        /// 单个道具商城物品UI父物体
        /// </summary>
        [SerializeField] private Transform uiSingleGameMallItemParent;
        
        private List<MallItemVo> _gameMallItemVoList;
        
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
            _gameMallItemVoList = mallManager.GetVirtualMallItemVoList();
            
            foreach (Transform child in uiSingleGameMallItemParent)
            {
                Destroy(child.gameObject);
            }
            
            if (_gameMallItemVoList == null || _gameMallItemVoList.Count == 0) return;
            
            foreach (var mallItemData in _gameMallItemVoList)
            {
                // Instantiate and set up the propsPurchaseItemUIPrefab with the mallData
                GameObject propsPurchaseItemUI = Instantiate(uiSingleGameMallItemPrefab, uiSingleGameMallItemParent);
                propsPurchaseItemUI.GetComponent<UISingleGameMallItem>().SetMallItem(mallItemData);
            }
        }

        private void OnCloseButtonClick()
        {
            Hide();
        }
    }
}