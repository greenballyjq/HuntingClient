using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 肉度条展示组件
    /// </summary>
    public class UIComponentMeatProgress : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 肉度填充图像
        /// </summary>
        [SerializeField] private Image imageMeatFill;

        /// <summary>
        /// 肉度底图
        /// </summary>
        [SerializeField] private Image imageMeatBackground;

        /// <summary>
        /// 肉度值文本
        /// </summary>
        [SerializeField] private Text textMeatValue;

        /// <summary>
        /// 肉度条文本
        /// </summary>
        [SerializeField] private Text textMeatBars;

        /// <summary>
        /// 前段颜色
        /// </summary>
        [SerializeField] private Color colorTierA;

        /// <summary>
        /// 中段颜色
        /// </summary>
        [SerializeField] private Color colorTierB;

        /// <summary>
        /// 后段颜色
        /// </summary>
        [SerializeField] private Color colorTierC;

        /// <summary>
        /// 同段内亮度提升因子
        /// </summary>
        [SerializeField, Range(1.0f, 2.0f)] private float brightnessMultiplier = 1.1f;

        /// <summary>
        /// 肉度条管理器
        /// </summary>
        private MeatProgressManager MeatProgressManager => GameServiceLocator.GetGameManager<MeatProgressManager>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        public void Init()
        {
            Event.AddListener(MeatEvents.MeatProgressChanged, OnMeatProgressChanged);
            Event.AddListener(MeatEvents.MeatBarCountChanged, OnMeatBarCountChanged);
            Event.AddListener(MeatEvents.MeatMaxBarsReached, OnMeatMaxBarsReached);

            UpdateAll(MeatProgressManager.GetCurrentMeatValue(), MeatProgressManager.GetCurrentMeatBars());
        }

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
        /// 更新整体显示
        /// </summary>
        private void UpdateAll(float currentMeat, int currentBars)
        {
            UpdateFill(currentMeat, currentBars);
            UpdateBarText(currentBars, MeatProgressManager.GetMaxMeatBars());
            UpdateValueText(currentMeat, currentBars);
            UpdateBackgroundColor(currentBars);
        }

        /// <summary>
        /// 更新填充图像
        /// </summary>
        private void UpdateFill(float currentMeat, int currentBars)
        {
            // 计算当前条的填充值
            float requiredPerBar = MeatProgressManager.GetRequiredPerBar();
            int maxBars = MeatProgressManager.GetMaxMeatBars();
            bool reachedMax = currentBars >= maxBars;

            float normalized = Mathf.Clamp01(currentMeat / requiredPerBar);
            imageMeatFill.fillAmount = reachedMax ? 1f : normalized;

            // 根据当前条决定填充颜色
            int nextBarIndex = reachedMax ? maxBars : Mathf.Clamp(currentBars + 1, 1, maxBars);
            imageMeatFill.color = EvaluateBarColor(nextBarIndex);
        }

        /// <summary>
        /// 更新条数文本
        /// </summary>
        private void UpdateBarText(int currentBars, int maxBars)
        {
            if (currentBars >= maxBars)
            {
                // 达到上限时显示无穷
                textMeatBars.text = "∞";
                textMeatBars.color = EvaluateBarColor(maxBars);
                return;
            }

            // 非满条时显示 当前/总条
            textMeatBars.text = $"{currentBars}/{maxBars}";
            int displayIndex = Mathf.Clamp(Mathf.Max(1, currentBars), 1, maxBars);
            textMeatBars.color = EvaluateBarColor(displayIndex);
        }

        /// <summary>
        /// 更新肉度值文本
        /// </summary>
        private void UpdateValueText(float currentMeat, int currentBars)
        {
            // 显示肉度值的当前/单条需求
            float requiredPerBar = MeatProgressManager.GetRequiredPerBar();
            int maxBars = MeatProgressManager.GetMaxMeatBars();
            if (currentBars >= maxBars)
            {
                textMeatValue.text = $"{Mathf.RoundToInt(requiredPerBar)}/{Mathf.RoundToInt(requiredPerBar)}";
                return;
            }

            textMeatValue.text = $"{Mathf.RoundToInt(currentMeat)}/{Mathf.RoundToInt(requiredPerBar)}";
        }

        /// <summary>
        /// 更新底图颜色
        /// </summary>
        private void UpdateBackgroundColor(int currentBars)
        {
            if (currentBars <= 0)
            {
                // 没有任何条时隐藏底图
                imageMeatBackground.color = Color.clear;
                return;
            }

            // 使用当前条颜色作为底图
            imageMeatBackground.color = EvaluateBarColor(currentBars);
        }

        /// <summary>
        /// 计算指定条数对应的颜色
        /// </summary>
        private Color EvaluateBarColor(int barIndex)
        {
            // 依据总条数将条索引限制在有效范围内
            int maxBars = Mathf.Max(1, MeatProgressManager.GetMaxMeatBars());
            int clampedIndex = Mathf.Clamp(barIndex, 1, maxBars);

            // 划分三个颜色区间并定位当前条所在区间
            int group1End = Mathf.Max(1, Mathf.CeilToInt(maxBars / 3f));
            int group2End = Mathf.Max(group1End, Mathf.CeilToInt(maxBars * 2f / 3f));

            int groupIndex;
            int groupStart;
            if (clampedIndex <= group1End)
            {
                groupIndex = 0;
                groupStart = 1;
            }
            else if (clampedIndex <= group2End)
            {
                groupIndex = 1;
                groupStart = group1End + 1;
            }
            else
            {
                groupIndex = 2;
                groupStart = group2End + 1;
            }

            int stepInGroup = Mathf.Max(0, clampedIndex - groupStart);
            Color baseColor = GetGroupBaseColor(groupIndex);
            return AdjustBrightness(baseColor, stepInGroup);
        }

        /// <summary>
        /// 获取组基础颜色
        /// </summary>
        private Color GetGroupBaseColor(int groupIndex)
        {
            return groupIndex switch
            {
                0 => colorTierA,
                1 => colorTierB,
                _ => colorTierC
            };
        }

        /// <summary>
        /// 调整亮度
        /// </summary>
        private Color AdjustBrightness(Color baseColor, int step)
        {
            // 提升亮度使同组后续条更亮
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            float boostedV = Mathf.Clamp01(v * Mathf.Pow(brightnessMultiplier, step));
            Color result = Color.HSVToRGB(h, s, boostedV);
            result.a = baseColor.a;
            return result;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 肉度值变化回调
        /// </summary>
        private void OnMeatProgressChanged(MeatProgressChangedEventArgs args)
        {
            int currentBars = MeatProgressManager.GetCurrentMeatBars();
            UpdateValueText(args.CurrentMeat, currentBars);
            UpdateFill(args.CurrentMeat, currentBars);
        }

        /// <summary>
        /// 肉度条变化回调
        /// </summary>
        private void OnMeatBarCountChanged(MeatBarCountChangedEventArgs args)
        {
            float currentMeat = MeatProgressManager.GetCurrentMeatValue();
            UpdateBarText(args.CurrentBars, MeatProgressManager.GetMaxMeatBars());
            UpdateBackgroundColor(args.CurrentBars);
            UpdateFill(currentMeat, args.CurrentBars);
            UpdateValueText(currentMeat, args.CurrentBars);
        }

        /// <summary>
        /// 肉度条满回调
        /// </summary>
        private void OnMeatMaxBarsReached()
        {
            int maxBars = MeatProgressManager.GetMaxMeatBars();
            UpdateBarText(maxBars, maxBars);
            imageMeatBackground.color = EvaluateBarColor(maxBars);
            imageMeatFill.fillAmount = 1f;
            imageMeatFill.color = EvaluateBarColor(maxBars);
            UpdateValueText(MeatProgressManager.GetRequiredPerBar(), maxBars);
        }
        #endregion
    }
}


