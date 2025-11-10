using GameFramework.Core;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 丰收能量条展示组件
    /// </summary>
    public class UIEnergyProgress : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 能量填充图像
        /// </summary>
        [SerializeField] private Image imageEnergyFill;

        /// <summary>
        /// 能量底图
        /// </summary>
        [SerializeField] private Image imageEnergyBackground;

        /// <summary>
        /// 能量条文本
        /// </summary>
        [SerializeField] private Text textEnergyBars;

        /// <summary>
        /// 前段颜色
        /// </summary>
        [SerializeField] private Color colorTierA = new Color(0.17f, 0.87f, 0.52f);

        /// <summary>
        /// 中段颜色
        /// </summary>
        [SerializeField] private Color colorTierB = new Color(0.98f, 0.78f, 0.29f);

        /// <summary>
        /// 后段颜色
        /// </summary>
        [SerializeField] private Color colorTierC = new Color(0.98f, 0.36f, 0.29f);

        /// <summary>
        /// 同段内亮度提升因子
        /// </summary>
        [SerializeField, Range(1.0f, 2.0f)] private float brightnessMultiplier = 1.1f;

        /// <summary>
        /// 丰收能量条管理器
        /// </summary>
        private EnergyProgressManager EnergyManager => GameServiceLocator.GetGameManager<EnergyProgressManager>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        public void Init()
        {
            Event.AddListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
            Event.AddListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
            Event.AddListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);

            UpdateAll(EnergyManager.GetCurrentEnergyValue(), EnergyManager.GetCurrentBars(), EnergyManager.GetRequiredPerBar());
        }

        public void CleanUp()
        {
            Event.RemoveListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
            Event.RemoveListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
            Event.RemoveListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        #region 更新逻辑
        /// <summary>
        /// 更新整体显示
        /// </summary>
        private void UpdateAll(float currentEnergy, int currentBars, float requiredPerBar)
        {
            // 刷新进度、文本与颜色
            UpdateFill(currentEnergy, requiredPerBar, currentBars);
            UpdateBarText(currentBars, EnergyManager.GetMaxBars());
            UpdateBackgroundColor(currentBars);
        }

        /// <summary>
        /// 更新填充图像
        /// </summary>
        private void UpdateFill(float currentEnergy, float requiredPerBar, int currentBars)
        {
            float normalized = requiredPerBar > 0f ? Mathf.Clamp01(currentEnergy / requiredPerBar) : 0f;
            imageEnergyFill.fillAmount = normalized;

            int maxBars = EnergyManager.GetMaxBars();
            int nextBarIndex = currentBars >= maxBars ? maxBars : Mathf.Clamp(currentBars + 1, 1, maxBars);
            imageEnergyFill.color = EvaluateBarColor(nextBarIndex);
        }

        /// <summary>
        /// 更新文本
        /// </summary>
        private void UpdateBarText(int currentBars, int maxBars)
        {
            if (maxBars > 0 && currentBars >= maxBars)
            {
                textEnergyBars.text = "∞";
                return;
            }

            textEnergyBars.text = $"{currentBars}条";
        }

        /// <summary>
        /// 更新底图颜色
        /// </summary>
        private void UpdateBackgroundColor(int currentBars)
        {
            if (currentBars <= 0)
            {
                imageEnergyBackground.color = Color.clear;
                return;
            }

            imageEnergyBackground.color = EvaluateBarColor(currentBars);
        }

        /// <summary>
        /// 计算指定条数对应的颜色
        /// </summary>
        private Color EvaluateBarColor(int barIndex)
        {
            int maxBars = Mathf.Max(1, EnergyManager.GetMaxBars());
            int clampedIndex = Mathf.Clamp(barIndex, 1, maxBars);

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
                // 中段条目
                groupIndex = 1;
                groupStart = group1End + 1;
            }
            else
            {
                // 末段条目
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
            if (step <= 0 || brightnessMultiplier <= 1f)
                return baseColor;

            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            float boostedV = Mathf.Clamp01(v * Mathf.Pow(brightnessMultiplier, step));
            Color result = Color.HSVToRGB(h, s, boostedV);
            result.a = baseColor.a;
            return result;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 能量值变化回调
        /// </summary>
        private void OnEnergyProgressChanged(EnergyProgressChangedEventArgs args)
        {
            // 能量值变化时刷新填充即可
            UpdateFill(args.CurrentEnergy, args.RequiredPerBar, args.CurrentBars);
        }

        /// <summary>
        /// 能量条变化回调
        /// </summary>
        private void OnEnergyBarCountChanged(EnergyBarCountChangedEventArgs args)
        {
            // 顺序刷新文本、颜色与当前进度
            UpdateBarText(args.CurrentBars, EnergyManager.GetMaxBars());
            UpdateBackgroundColor(args.CurrentBars);
            UpdateFill(EnergyManager.GetCurrentEnergyValue(), EnergyManager.GetRequiredPerBar(), args.CurrentBars);
        }

        /// <summary>
        /// 能量条上限回调
        /// </summary>
        private void OnEnergyMaxBarsReached()
        {
            int maxBars = EnergyManager.GetMaxBars();
            // 满条后直接显示无穷并锁定最终色
            UpdateBarText(maxBars, maxBars);
            imageEnergyBackground.color = EvaluateBarColor(maxBars);
            imageEnergyFill.fillAmount = 1f;
            imageEnergyFill.color = EvaluateBarColor(maxBars);
        }
        #endregion
    }
}

