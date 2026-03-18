using CoreGameLogic.UI.Platform;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGameLogic.UI.GM
{
    /// <summary>
    /// GM界面
    /// </summary>
    public class UIGM : UIBase
    {
        [SerializeField] private Button openGameMallButton;
        
        [SerializeField] private Button openPhysicalMallButton;
        
        // [SerializeField] private Button openSignInMallButton;
        //
        // [SerializeField] private Button openPlayerProfileMallButton;
        
        
        [SerializeField] private UIGameMall gameMallPanel;
        [SerializeField] private UIPhysicalMall physicalMallPanel;
        // [SerializeField] private GameObject signInMallPanel;
        // [SerializeField] private GameObject playerProfileMallPanel;

        [SerializeField] private Button closeButton;
        
        private UIManager _uiManager;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            // openGameMallButton.onClick.AddListener(OpenGameMallButtonClick);
            // openPhysicalMallButton.onClick.AddListener(OpenPhysicalMallButtonClick);
            // openSignInMallButton.onClick.AddListener(OpenSignInMallButtonClick);
            // openPlayerProfileMallButton.onClick.AddListener(OpenPlayerProfileMallButtonClick);
            
            closeButton.onClick.AddListener(() =>
            {
                Hide();
                gameMallPanel.Hide();
                physicalMallPanel.Hide();
            });

            _uiManager = GameServiceLocator.UIManager;
            
            gameMallPanel.Hide();
            physicalMallPanel.Hide();
        }

        protected override void OnShow()
        {
            base.OnShow();
            openGameMallButton.onClick.AddListener(OpenGameMallButtonClick);
            openPhysicalMallButton.onClick.AddListener(OpenPhysicalMallButtonClick);
            
            closeButton.onClick.AddListener(() =>
            {
                Hide();
                gameMallPanel.Hide();
                physicalMallPanel.Hide();
            });

            _uiManager = GameServiceLocator.UIManager;
            
            gameMallPanel.Hide();
            physicalMallPanel.Hide();
        }

        protected override void OnHide()
        {
            base.OnHide();
            openGameMallButton.onClick.RemoveListener(OpenGameMallButtonClick);
            openPhysicalMallButton.onClick.RemoveListener(OpenPhysicalMallButtonClick);
        }

        private async void OpenGameMallButtonClick()
        {
            
            gameMallPanel.Show();
            // UIGameMall uiGameMall = await _uiManager.OpenUIAsync<UIGameMall>(nameof(UIGameMall));
            // uiGameMall.Show();
        }

        private async void OpenPhysicalMallButtonClick()
        {
            physicalMallPanel.Show();
            // UIPhysicalMall uiPhysicalMall = await _uiManager.OpenUIAsync<UIPhysicalMall>(nameof(UIPhysicalMall));
            // uiPhysicalMall.Show();
        }

        private void OpenSignInMallButtonClick()
        {
            
        }

        private void OpenPlayerProfileMallButtonClick()
        {
            
        }
    }
}