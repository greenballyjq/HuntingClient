using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 丰收能量条展示组件
    /// </summary>
    public class UIComponentEnergyProgress : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 能量填充图像
        /// </summary>
        [SerializeField] private Image _imageEnergyFill;

        /// <summary>
        /// 能量底图图像
        /// </summary>
        [SerializeField] private Image _imageEnergyBackground;

        /// <summary>
        /// 能量条文本
        /// </summary>
        [SerializeField] private Text _textEnergyBars;

        /// <summary>
        /// 技能按钮
        /// </summary>
        [SerializeField] private Button _buttonSkill;

        /// <summary>
        /// 前段颜色
        /// </summary>
        [SerializeField] private Color _colorTierA;

        /// <summary>
        /// 中段颜色
        /// </summary>
        [SerializeField] private Color _colorTierB;

        /// <summary>
        /// 后段颜色
        /// </summary>
        [SerializeField] private Color _colorTierC;

        /// <summary>
        /// 同段内亮度提升因子
        /// </summary>
        [SerializeField, Range(1.0f, 2.0f)] private float _brightnessMultiplier = 1.1f;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 技能管理器
        /// </summary>
        private SkillManager Skill => GameServiceLocator.GetGameManager<SkillManager>();

        /// <summary>
        /// 丰收能量条管理器
        /// </summary>
        private EnergyProgressManager EnergyManager => GameServiceLocator.GetGameManager<EnergyProgressManager>();

        private void Awake()
        {
            _buttonSkill.onClick.AddListener(OnSkillButtonClicked);
        }

        public void Init()
        {
            Event.AddListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
            Event.AddListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
            Event.AddListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);

            UpdateAll(EnergyManager.GetCurrentEnergyValue(), EnergyManager.GetCurrentBars());
        }

        public void CleanUp()
        {
            Event.RemoveListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
            Event.RemoveListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
            Event.RemoveListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);
        }

        private void OnDestroy()
        {
            _buttonSkill.onClick.RemoveListener(OnSkillButtonClicked);
            CleanUp();
        }

        #region 更新逻辑
        /// <summary>
        /// 更新整体显示
        /// </summary>
        private void UpdateAll(float currentEnergy, int currentBars)
        {
            UpdateFill(currentEnergy, currentBars);
            UpdateBarText(currentBars, EnergyManager.GetMaxBars());
            UpdateBackgroundColor(currentBars);
        }

        /// <summary>
        /// 更新填充图像
        /// </summary>
        private void UpdateFill(float currentEnergy, int currentBars)
        {
            // 计算当前条的填充比例
            float requiredPerBar = EnergyManager.GetRequiredPerBar();
            float normalized = Mathf.Clamp01(currentEnergy / requiredPerBar);
            _imageEnergyFill.fillAmount = normalized;

            // 根据当前条决定填充颜色
            int maxBars = EnergyManager.GetMaxBars();
            int nextBarIndex = currentBars >= maxBars ? maxBars : Mathf.Clamp(currentBars + 1, 1, maxBars);
            _imageEnergyFill.color = EvaluateBarColor(nextBarIndex);
        }

        /// <summary>
        /// 更新文本
        /// </summary>
        private void UpdateBarText(int currentBars, int maxBars)
        {
            if (maxBars > 0 && currentBars >= maxBars)
            {
                // 满条时显示无穷并与最终颜色同步
                _textEnergyBars.text = "∞";
                _textEnergyBars.color = EvaluateBarColor(maxBars);
                return;
            }

            // 非满条时显示 当前/总条
            _textEnergyBars.text = $"{currentBars}/{maxBars}";
            int displayIndex = Mathf.Clamp(Mathf.Max(1, currentBars), 1, maxBars);
            _textEnergyBars.color = EvaluateBarColor(displayIndex);
        }

        /// <summary>
        /// 更新底图颜色
        /// </summary>
        private void UpdateBackgroundColor(int currentBars)
        {
            if (currentBars <= 0)
            {
                // 没有任何条时保持透明
                _imageEnergyBackground.color = Color.clear;
                return;
            }

            // 使用当前条的颜色渲染底图
            _imageEnergyBackground.color = EvaluateBarColor(currentBars);
        }

        /// <summary>
        /// 计算指定条数对应的颜色
        /// </summary>
        private Color EvaluateBarColor(int barIndex)
        {
            // 依据总条数将条索引限制在有效范围内
            int maxBars = Mathf.Max(1, EnergyManager.GetMaxBars());
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
                0 => _colorTierA,
                1 => _colorTierB,
                _ => _colorTierC
            };
        }

        /// <summary>
        /// 调整亮度
        /// </summary>
        private Color AdjustBrightness(Color baseColor, int step)
        {
            // 提升亮度使同组后续条更亮
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            float boostedV = Mathf.Clamp01(v * Mathf.Pow(_brightnessMultiplier, step));
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
            UpdateFill(args.CurrentEnergy, args.CurrentBars);
        }

        /// <summary>
        /// 能量条变化回调
        /// </summary>
        private void OnEnergyBarCountChanged(EnergyBarCountChangedEventArgs args)
        {
            UpdateBarText(args.CurrentBars, EnergyManager.GetMaxBars());
            UpdateBackgroundColor(args.CurrentBars);
            UpdateFill(EnergyManager.GetCurrentEnergyValue(), args.CurrentBars);
        }

        /// <summary>
        /// 能量条上限回调
        /// </summary>
        private void OnEnergyMaxBarsReached()
        {
            int maxBars = EnergyManager.GetMaxBars();
            UpdateBarText(maxBars, maxBars);
            _imageEnergyBackground.color = EvaluateBarColor(maxBars);
            _imageEnergyFill.fillAmount = 1f;
            _imageEnergyFill.color = EvaluateBarColor(maxBars);
        }

        /// <summary>
        /// 点击技能按钮回调
        /// </summary>
        private void OnSkillButtonClicked()
        {
            // 请求技能管理器尝试启动技能
            Skill.TryStartSkill();
        }
        #endregion
    }
}

