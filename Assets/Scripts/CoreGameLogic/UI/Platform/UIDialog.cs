using System;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGameLogic.UI.Platform
{
    public class UIDialog : UIBase
    {
        [SerializeField] protected TextMeshProUGUI dialogTitleText;
        [SerializeField] protected TextMeshProUGUI contentTitleText;
        [SerializeField] protected TextMeshProUGUI messageText;

        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        
        protected Action<UIDialog> _leftButtonCallback;
        protected Action<UIDialog> _rightButtonCallback;

        public void SetDialog(string confirm, string title, string message,
            Action<UIDialog> leftButtonCallback = null, Action<UIDialog> rightButtonCallback = null)
        {
            dialogTitleText.text = confirm;
            contentTitleText.text = title;
            messageText.text = message;
            _leftButtonCallback = leftButtonCallback;
            _rightButtonCallback = rightButtonCallback;
        }
        
        public override void OnInit(object userData)
        {
            leftButton.onClick.AddListener(OnLeftButtonClick);
            rightButton.onClick.AddListener(OnRightButtonClick);
            base.OnInit(userData);
        }

        public override void OnClose()
        {
            base.OnClose();
            leftButton.onClick.RemoveListener(OnLeftButtonClick);
            rightButton.onClick.RemoveListener(OnRightButtonClick);
        }

        private void OnLeftButtonClick()
        {
            _leftButtonCallback?.Invoke(this);
        }

        private void OnRightButtonClick()
        {
            _rightButtonCallback?.Invoke(this);
        }
    }
}