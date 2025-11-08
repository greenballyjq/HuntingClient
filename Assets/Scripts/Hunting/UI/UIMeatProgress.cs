using System;
using GameFramework.Core;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 肉度条展示组件
    /// </summary>
    public class UIMeatProgress : MonoBehaviour
    {
        /// <summary>
        /// 肉度进度条
        /// </summary>
        [SerializeField] private Slider sliderMeatProgress;

        /// <summary>
        /// 进度填充图像
        /// </summary>
        [SerializeField] private Image imageFill;

        /// <summary>
        /// 肉度值文本
        /// </summary>
        [SerializeField] private Text textMeatValue;

        /// <summary>
        /// 肉度条文本
        /// </summary>
        [SerializeField] private Text textMeatBars;

        /// <summary>
        /// 低档颜色
        /// </summary>
        [SerializeField] private Color colorLow = Color.green;

        /// <summary>
        /// 中档颜色
        /// </summary>
        [SerializeField] private Color colorMid = Color.yellow;

        /// <summary>
        /// 高档颜色
        /// </summary>
        [SerializeField] private Color colorHigh = new Color(1f, 0.85f, 0f);

        /// <summary>
        /// 肉度条管理器
        /// </summary>
        private MeatProgressManager MeatProgressManager => GameServiceLocator.GetGameManager<MeatProgressManager>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 初始化组件
        /// </summary>
        public void Init()
        {
            Event.AddListener(MeatEvents.MeatProgressChanged, OnMeatProgressChanged);
            Event.AddListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
            Event.AddListener(MeatEvents.MeatMaxBarsReached, OnMeatMaxBarsReached);

            UpdateMeatValue(MeatProgressManager.GetCurrentMeatValue(), MeatProgressManager.GetRequiredPerBar());
            UpdateMeatBars(MeatProgressManager.GetCurrentMeatBars());
        }

        /// <summary>
        /// 清理组件
        /// </summary>
        public void CleanUp()
        {
            Event.RemoveListener(MeatEvents.MeatProgressChanged, OnMeatProgressChanged);
            Event.RemoveListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
            Event.RemoveListener(MeatEvents.MeatMaxBarsReached, OnMeatMaxBarsReached);
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        #region 私有方法
        /// <summary>
        /// 更新肉度值显示
        /// </summary>
        private void UpdateMeatValue(int value, int required)
        {
            int currentBars = MeatProgressManager.GetCurrentMeatBars();
            int maxBars = MeatProgressManager.GetMaxMeatBars();

            if (currentBars >= maxBars)
            {
                sliderMeatProgress.value = 1f;
                textMeatValue.text = "∞";
                return;
            }

            float ratio = required > 0 ? Mathf.Clamp01((float)value / required) : 0f;
            sliderMeatProgress.value = ratio;
            textMeatValue.text = $"{value}/{required}";
        }

        /// <summary>
        /// 更新肉度条显示
        /// </summary>
        private void UpdateMeatBars(int bars)
        {
            textMeatBars.text = $"{bars}条";
            UpdateFillColor(bars);
        }

        /// <summary>
        /// 更新填充颜色
        /// </summary>
        private void UpdateFillColor(int bars)
        {
            if (imageFill == null) return;

            if (bars < 3)
                imageFill.color = colorLow;
            else if (bars < 5)
                imageFill.color = colorMid;
            else
                imageFill.color = colorHigh;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 肉度值变化回调
        /// </summary>
        private void OnMeatProgressChanged(MeatProgressChangedEventArgs args)
        {
            UpdateMeatValue(args.CurrentProgress, MeatProgressManager.GetRequiredPerBar());
        }

        /// <summary>
        /// 肉度条变化回调
        /// </summary>
        private void OnMeatBarCountChanged(MeatBarCountChangedEventArgs args)
        {
            UpdateMeatBars(args.CurrentBars);
        }

        /// <summary>
        /// 肉度条满回调
        /// </summary>
        private void OnMeatMaxBarsReached()
        {
            UpdateMeatValue(0, 0);
            UpdateMeatBars(MeatProgressManager.GetMaxMeatBars());
        }
        #endregion

        
    }
}


