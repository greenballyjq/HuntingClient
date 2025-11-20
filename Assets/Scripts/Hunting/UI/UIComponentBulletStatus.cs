using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 子弹状态展示组件
    /// </summary>
    public class UIComponentBulletStatus : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 子弹图标图像
        /// </summary>
        [SerializeField] private Image _imageBullet;

        /// <summary>
        /// 倒计时文本
        /// </summary>
        [SerializeField] private Text _textCountdown;

        /// <summary>
        /// 正常颜色（倒计时初始颜色）
        /// </summary>
        [SerializeField] private Color _colorNormal = Color.white;

        /// <summary>
        /// 警告颜色（倒计时临近结束颜色）
        /// </summary>
        [SerializeField] private Color _colorWarning = Color.red;

        /// <summary>
        /// 闪烁速度倍率
        /// </summary>
        [SerializeField, Range(0.5f, 3.0f)] private float _blinkSpeedMultiplier = 2.0f;

        /// <summary>
        /// 当前子弹ID
        /// </summary>
        private int _currentBulletId;

        /// <summary>
        /// 是否为特殊子弹
        /// </summary>
        private bool _isSpecialBullet;

        /// <summary>
        /// 特殊子弹剩余时间
        /// </summary>
        private float _remainingTime;

        /// <summary>
        /// 特殊子弹总时间
        /// </summary>
        private float _totalTime;

        /// <summary>
        /// 闪烁动画时间累积
        /// </summary>
        private float _blinkTime;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager Resource => GameServiceLocator.Resource;

        public void Init()
        {
            Event.AddListener(BulletEvents.BulletChanged, OnBulletChanged);
            Event.AddListener(BulletEvents.SpecialBulletCountdown, OnSpecialBulletCountdown);
            Event.AddListener(BulletEvents.SpecialBulletEffectEnded, OnSpecialBulletEffectEnded);
        }

        public void CleanUp()
        {
            Event.RemoveListener(BulletEvents.BulletChanged, OnBulletChanged);
            Event.RemoveListener(BulletEvents.SpecialBulletCountdown, OnSpecialBulletCountdown);
            Event.RemoveListener(BulletEvents.SpecialBulletEffectEnded, OnSpecialBulletEffectEnded);
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void Update()
        {
            // 特殊子弹时更新闪烁效果
            if (_isSpecialBullet)
                UpdateBlinkEffect();
        }

        #region 私有方法
        /// <summary>
        /// 更新子弹图标
        /// </summary>
        /// <param name="bulletData">子弹配置</param>
        private async UniTask UpdateBulletIconAsync(Bullet bulletData)
        {
            var sprite = await Resource.LoadAssetAsync<Sprite>(bulletData.IconResourcePath);
            _imageBullet.sprite = sprite;
        }

        /// <summary>
        /// 更新倒计时显示
        /// </summary>
        /// <param name="isSpecial">是否为特殊子弹</param>
        /// <param name="remainingTime">剩余时间</param>
        /// <param name="totalTime">总时间</param>
        private void UpdateCountdown(bool isSpecial, float remainingTime = 0f, float totalTime = 0f)
        {
            if (!isSpecial)
            {
                // 普通子弹显示无穷符号
                _textCountdown.text = "∞";
                _textCountdown.color = _colorNormal;
                _blinkTime = 0f;
            }
            else
            {
                // 特殊子弹显示倒计时数字
                _textCountdown.text = Mathf.CeilToInt(remainingTime).ToString();
                
                // 根据剩余时间比例计算颜色（从正常颜色渐变到警告颜色）
                float timeRatio = Mathf.Clamp01(remainingTime / totalTime);
                _textCountdown.color = Color.Lerp(_colorWarning, _colorNormal, timeRatio);
            }
        }

        /// <summary>
        /// 更新闪烁效果
        /// </summary>
        private void UpdateBlinkEffect()
        {
            // 计算剩余时间比例
            float timeRatio = Mathf.Clamp01(_remainingTime / _totalTime);
            
            // 时间越少，闪烁速度越快
            float blinkSpeed = _blinkSpeedMultiplier * (1f - timeRatio) + 0.5f;
            _blinkTime += Time.deltaTime * blinkSpeed;

            // 使用正弦波实现闪烁效果（时间越少闪烁越明显）
            float alpha = Mathf.Lerp(0.3f, 1f, (Mathf.Sin(_blinkTime * Mathf.PI * 2f) + 1f) * 0.5f);
            Color currentColor = _textCountdown.color;
            currentColor.a = alpha;
            _textCountdown.color = currentColor;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 子弹切换事件回调
        /// </summary>
        private void OnBulletChanged(BulletChangedEventArgs args)
        {
            _currentBulletId = args.NewBulletData.ID;
            _isSpecialBullet = args.IsSpecialBullet;
            _remainingTime = args.RemainingTime;
            _totalTime = args.NewBulletData.Duration;

            // 更新子弹图标
            UpdateBulletIconAsync(args.NewBulletData).Forget();

            // 更新倒计时显示
            UpdateCountdown(args.IsSpecialBullet, args.RemainingTime, _totalTime);
        }

        /// <summary>
        /// 特殊子弹倒计时事件回调
        /// </summary>
        private void OnSpecialBulletCountdown(SpecialBulletCountdownEventArgs args)
        {
            if (args.BulletData.ID != _currentBulletId)
                return;

            _remainingTime = args.RemainingTime;
            _totalTime = args.TotalTime;

            // 更新倒计时显示
            UpdateCountdown(true, args.RemainingTime, args.TotalTime);
        }

        /// <summary>
        /// 特殊子弹效果结束事件回调
        /// </summary>
        private void OnSpecialBulletEffectEnded(SpecialBulletEffectEndedEventArgs args)
        {
            _isSpecialBullet = false;
            _remainingTime = 0f;
            _totalTime = 0f;
            _blinkTime = 0f;
        }
        #endregion
    }
}

