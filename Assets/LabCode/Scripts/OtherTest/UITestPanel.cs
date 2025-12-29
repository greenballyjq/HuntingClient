using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 测试UI面板
    /// </summary>
    public class UITestPanel : UIBase
    {
        /// <summary>
        /// 测试按钮
        /// </summary>
        [SerializeField] private Button _testButton;
        
        /// <summary>
        /// 动画状态标志位
        /// </summary>
        private bool _isAnimating = false;

        private void Awake()
        {
            _testButton.onClick.AddListener(OnClickTestButton);
        }

        private void OnDestroy()
        {
            _testButton.onClick.RemoveListener(OnClickTestButton);
        }

        /// <summary>
        /// 测试按钮点击回调
        /// </summary>
        private async void OnClickTestButton()
        {
            if (_isAnimating) return;
            
            _isAnimating = true;
            Sequence sequence = DOTween.Sequence();

            // 获取按钮原始颜色
            Color originalColor = _testButton.image.color;
            // 目标透明度50%的颜色
            Color halfAlphaColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

            // 重复3次：透明100% -> 透明50% -> 透明100%
            for (int i = 0; i < 3; i++)
            {
                sequence.Append(_testButton.image.DOColor(halfAlphaColor, 0.5f));
                sequence.Append(_testButton.image.DOColor(originalColor, 0.5f));
            }

            // 等待动画完成
            await sequence.AsyncWaitForCompletion();

            // 重置动画状态
            _isAnimating = false;

            // 调用HuntingAppFlow的EnterPrepareAsync
            // await HuntingAppFlow.Instance.EnterPrepareAsync();

            // this.Close();
        }
    }
}