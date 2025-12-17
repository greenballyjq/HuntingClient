using Cysharp.Threading.Tasks;
using GameFramework.Core;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 骰子视觉组件
    /// </summary>
    public class DiceVFX : MonoBehaviour
    {
        /// <summary>
        /// 骰子图像
        /// </summary>
        [SerializeField] private Image _imageDice;

        /// <summary>
        /// 骰子旋转序列帧图片数组
        /// </summary>
        [SerializeField] private Sprite[] _spinSprites;

        /// <summary>
        /// 骰子结果图片数组
        /// </summary>
        [SerializeField] private Sprite[] _resultSprites;

        /// <summary>
        /// 旋转动画时长
        /// </summary>
        [SerializeField] private float _spinDuration = 2f;

        /// <summary>
        /// 动画帧率
        /// </summary>
        [SerializeField, Range(1, 60)] private int _animationFPS = 12;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        private void Awake()
        {
            Event.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
        }

        private void OnDestroy()
        {
            Event.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        }

        #region 私有方法
        /// <summary>
        /// 播放骰子动画
        /// </summary>
        /// <param name="diceValue">骰子点数</param>
        private async UniTask PlayDiceAnimationAsync(int diceValue)
        {
            // 触发投骰子动画开始事件
            Event.Trigger(PrepareEvents.DiceAnimationStarted);

            // 播放旋转动画
            await PlaySpinAnimationAsync();

            // 显示骰子结果
            ShowDiceResult(diceValue);

            // 触发投骰子动画结束事件
            Event.Trigger(PrepareEvents.DiceAnimationEnded);
        }

        /// <summary>
        /// 播放旋转动画
        /// </summary>
        private async UniTask PlaySpinAnimationAsync()
        {
            // 根据FPS计算帧间隔
            float frameInterval = 1f / _animationFPS;
            float elapsed = 0f;
            int currentFrameIndex = 0;

            while (elapsed < _spinDuration)
            {
                // 循环播放旋转序列
                _imageDice.sprite = _spinSprites[currentFrameIndex];
                currentFrameIndex = (currentFrameIndex + 1) % _spinSprites.Length;

                await UniTask.Delay((int)(frameInterval * 1000));
                elapsed += frameInterval;
            }
        }

        /// <summary>
        /// 显示骰子结果
        /// </summary>
        /// <param name="diceValue">骰子点数</param>
        private void ShowDiceResult(int diceValue)
        {
            int spriteIndex = diceValue - 1;
            if (spriteIndex >= 0 && spriteIndex < _resultSprites.Length)
                _imageDice.sprite = _resultSprites[spriteIndex];
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 角色选中事件回调
        /// </summary>
        /// <param name="args">事件参数</param>
        private void OnRoleSelected(RoleSelectedEventArgs args)
        {
            PlayDiceAnimationAsync(args.DiceValue).Forget();
        }
        #endregion
    }
}

