using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 礼包开箱视觉组件
    /// </summary>
    public class GiftOpenVFX : MonoBehaviour
    {
        /// <summary>
        /// 礼包类型
        /// </summary>
        [SerializeField] private ELuckyGiftType _giftType;

        /// <summary>
        /// 礼包图像
        /// </summary>
        [SerializeField] private Image _imageGift;

        /// <summary>
        /// 礼包打开序列帧图片数组
        /// </summary>
        [SerializeField] private Sprite[] _giftOpenSprites;

        /// <summary>
        /// 动画帧率
        /// </summary>
        [SerializeField, Range(1, 60)] private int _animationFPS = 12;

        /// <summary>
        /// 初始礼包图像
        /// </summary>
        private Sprite _originalGiftSprite;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        private void Awake()
        {
            _originalGiftSprite = _imageGift.sprite;

            Event.AddListener(LuckyBuffEvents.GiftOpened, OnGiftOpened);
            Event.AddListener(LuckyBuffEvents.LuckyBuffConfirmClicked, OnLuckyBuffConfirmClicked);
        }

        private void OnDestroy()
        {
            Event.RemoveListener(LuckyBuffEvents.GiftOpened, OnGiftOpened);
            Event.RemoveListener(LuckyBuffEvents.LuckyBuffConfirmClicked, OnLuckyBuffConfirmClicked);
        }

        #region 私有方法
        /// <summary>
        /// 播放礼包动画
        /// </summary>
        /// <param name="buffData">幸运仪式增益配置</param>
        private async UniTask PlayGiftAnimationAsync(LuckyBuff buffData)
        {
            // 触发礼包开启动画开始事件
            Event.Trigger(LuckyBuffEvents.GiftOpenAnimationStarted);

            // 播放礼包开启动画
            await PlayOpenAnimationAsync();

            // 触发礼包开启动画结束事件
            Event.Trigger(LuckyBuffEvents.GiftOpenAnimationEnded, new GiftOpenAnimationEndedEventArgs
            {
                LuckyBuffData = buffData
            });
        }

        /// <summary>
        /// 播放礼包动画
        /// </summary>
        private async UniTask PlayOpenAnimationAsync()
        {
            
            // float frameInterval = totalDuration / _giftOpenSprites.Length;
            // 测试阶段：无论有多少帧，总时长固定约为 2 秒，方便观察
            const float totalDuration = 2f;
            float frameInterval = totalDuration / _giftOpenSprites.Length;

            for (int i = 0; i < _giftOpenSprites.Length; i++)
            {
                if (_imageGift != null)
                    _imageGift.sprite = _giftOpenSprites[i];

                await UniTask.Delay((int)(frameInterval * 1000));
            }
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 礼包开启事件回调
        /// </summary>
        /// <param name="args">事件参数</param>
        private void OnGiftOpened(GiftOpenedEventArgs args)
        {
            // 判断是否匹配自己的礼包类型
            if (args.GiftType == _giftType)
                PlayGiftAnimationAsync(args.LuckyBuffData).Forget();
        }

        /// <summary>
        /// 幸运增益确认点击事件回调
        /// </summary>
        private void OnLuckyBuffConfirmClicked()
        {
            // 重置礼包图像到初始状态
            _imageGift.sprite = _originalGiftSprite;
        }
        #endregion
    }
}

