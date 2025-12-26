using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core;
using Hunting.Events;
using UnityEngine;

namespace Hunting.UI
{
    /// <summary>
    /// 走格子视觉组件
    /// </summary>
    public class SlotVFX : MonoBehaviour
    {
        /// <summary>
        /// 单个格子动画时长
        /// </summary>
        [SerializeField] private float _slotDuration = 0.3f;

        /// <summary>
        /// 缩放倍率
        /// </summary>
        [SerializeField] private float _scaleMultiplier = 1.2f;

        /// <summary>
        /// 起始格子索引
        /// </summary>
        private int _fromSlotIndex;

        /// <summary>
        /// 骰子点数
        /// </summary>
        private int _diceValue;

        /// <summary>
        /// 角色格子列表
        /// </summary>
        private UIComponentRoleSlot[] _roleSlots;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager => GameServiceLocator.EventManager;

        private void Awake()
        {
            _eventManager.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
            _eventManager.AddListener(PrepareEvents.DiceAnimationEnded, OnDiceAnimationEnded);
        }

        private void OnDestroy()
        {
            _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
            _eventManager.RemoveListener(PrepareEvents.DiceAnimationEnded, OnDiceAnimationEnded);
        }

        #region 私有方法
        /// <summary>
        /// 播放走格子动画
        /// </summary>
        private async UniTask PlaySlotAnimationAsync()
        {
            // 触发走格子动画开始事件
            _eventManager.Trigger(PrepareEvents.SlotAnimationStarted);

            // 恢复起始格子到原始大小
            if (_fromSlotIndex >= 0 && _fromSlotIndex < _roleSlots.Length)
            {
                Transform fromSlotTransform = _roleSlots[_fromSlotIndex].transform;
                Tween resetTween = fromSlotTransform.DOScale(Vector3.one, _slotDuration);
                while (resetTween.IsActive())
                    await UniTask.Yield();
            }

            // 计算起始格子循环走到目标格子的路径
            int currentIndex = _fromSlotIndex;
            int stepCount = _diceValue;
            int totalSlots = _roleSlots.Length;

            for (int i = 0; i < stepCount; i++)
            {
                // 计算下一个格子索引
                int nextIndex = (currentIndex + 1) % totalSlots;
                bool isLastStep = (i == stepCount - 1);

                await PlaySingleSlotAnimationAsync(nextIndex, isLastStep);

                currentIndex = nextIndex;
            }

            // 触发走格子动画结束事件
            _eventManager.Trigger(PrepareEvents.SlotAnimationEnded);

            // 触发完整选角动画结束事件
            _eventManager.Trigger(PrepareEvents.RoleSelectionAnimationEnded);
        }

        /// <summary>
        /// 播放单个格子动画
        /// </summary>
        /// <param name="slotIndex">格子索引</param>
        /// <param name="isLastStep">是否为最后一步</param>
        private async UniTask PlaySingleSlotAnimationAsync(int slotIndex, bool isLastStep)
        {
            Transform slotTransform = _roleSlots[slotIndex].transform;
            Vector3 originalScale = Vector3.one;
            Vector3 targetScale = originalScale * _scaleMultiplier;

            Tween tween;
            if (isLastStep)
            {
                // 最后一步放大并保持
                tween = slotTransform.DOScale(targetScale, _slotDuration);
            }
            else
            {
                // 中间步骤放大后恢复原样
                Sequence sequence = DOTween.Sequence();
                sequence.Append(slotTransform.DOScale(targetScale, _slotDuration / 2f));
                sequence.Append(slotTransform.DOScale(originalScale, _slotDuration / 2f));
                tween = sequence;
            }

            // 等待动画完成
            while (tween.IsActive())
                await UniTask.Yield();
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 角色选中事件回调
        /// </summary>
        private void OnRoleSelected(RoleSelectedEventArgs args)
        {
            _fromSlotIndex = args.FromSlotIndex;
            _diceValue = args.DiceValue;
            _roleSlots = args.RoleSlots;
        }

        /// <summary>
        /// 骰子动画结束事件回调
        /// </summary>
        /// <param name="args">事件参数</param>
        private void OnDiceAnimationEnded()
        {
            PlaySlotAnimationAsync().Forget();
        }
        #endregion
    }
}

