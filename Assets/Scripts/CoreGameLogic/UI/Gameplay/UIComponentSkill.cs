using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;

/// <summary>
/// 技能组件
/// </summary>
public class UIComponentSkill : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 技能条填充背景
    /// </summary>
    [SerializeField] private Image _imageSkillBarBackground;

    /// <summary>
    /// 技能条填充
    /// </summary>
    [SerializeField] private Image _imageSkillBarFill;

    /// <summary>
    /// 技能按钮
    /// </summary>
    [SerializeField] private Button _buttonSkill;

    /// <summary>
    /// 技能图像
    /// </summary>
    [SerializeField] private Image _imageSkill;

    /// <summary>
    /// 技能条文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textSkillBarAmount;

    /// <summary>
    /// 颜色数组
    /// </summary>
    [SerializeField] private Color[] _colors;

    /// <summary>
    /// 技能图像矩形变换组件
    /// </summary>
    private RectTransform _rectTransformSkillImage;

    public RectTransform RectTransformSkillImage => _rectTransformSkillImage;

    /// <summary>
    /// 单条所需值
    /// </summary>
    private float _valuePerBar;

    /// <summary>
    /// 总条数
    /// </summary>
    private int _totalBar;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 丰收能量条管理器
    /// </summary>
    private EnergyProgressManager _energyProgressManager => GameServiceLocator.GetRoundManager<EnergyProgressManager>();

    /// <summary>
    /// 技能管理器
    /// </summary>
    private SkillManager _skillManager => GameServiceLocator.GetRoundManager<SkillManager>();

    private void Awake()
    {
        _buttonSkill.onClick.AddListener(OnSkillButtonClicked);

        _rectTransformSkillImage =  _imageSkill.GetComponent<RectTransform>();
    }

    public void Init()
    {
        _totalBar = _energyProgressManager.TotalBar;

        _valuePerBar = _energyProgressManager.ValuePerBar;

        UpdateFill(0,0);
        UpdateBarText(0);
        UpdateBackgroundColor(0);

        _eventManager.AddListener(RoundEvents.RoundEntered, OnRoundEntered);
        _eventManager.AddListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
        _eventManager.AddListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
        _eventManager.AddListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(RoundEvents.RoundEntered, OnRoundEntered);
        _eventManager.RemoveListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
        _eventManager.RemoveListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
        _eventManager.RemoveListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);
    }

    private void OnDestroy()
    {
        _buttonSkill.onClick.RemoveListener(OnSkillButtonClicked);
    }

    #region 私有方法
    /// <summary>
    /// 更新填充图像
    /// </summary>
    private void UpdateFill(float currentEnergy, int currentBars)
    {
        _imageSkillBarFill.fillAmount = currentEnergy / _valuePerBar;
        int colorIndex = currentBars >= _colors.Length ? _colors.Length - 1 : currentBars;
        _imageSkillBarFill.color = _colors[colorIndex];
    }

    /// <summary>
    /// 更新文本
    /// </summary>
    private void UpdateBarText(int currentBars)
    {
        _textSkillBarAmount.text = $"{currentBars}/{_totalBar}";
    }

    /// <summary>
    /// 更新底图颜色
    /// </summary>
    private void UpdateBackgroundColor(int currentBars)
    {
        if (currentBars <= 0)
        {
            _imageSkillBarBackground.color = Color.clear;
            return;
        }
        int colorIndex = currentBars - 1 >= _colors.Length ? _colors.Length - 1 : currentBars - 1;
        _imageSkillBarBackground.color = _colors[colorIndex];
    }

    /// <summary>
    /// 更新技能图标
    /// </summary>
    /// <param name="skillId">技能ID</param>
    private void UpdateSkillIcon(int skillId)
    {
        _imageSkill.sprite = _configManager.SkillRefSo.GetSkillIcon(skillId);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 单局开始事件回调
    /// </summary>
    private void OnRoundEntered(RoundEnteredEventArgs args)
    {
        UpdateSkillIcon(args.RoundContext.SkillData.ID);
    }

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
        UpdateFill(_energyProgressManager.CurrentEnergyValue, args.CurrentBars);
        UpdateBarText(args.CurrentBars);
        UpdateBackgroundColor(args.CurrentBars);
    }

    /// <summary>
    /// 能量条上限回调
    /// </summary>
    private void OnEnergyMaxBarsReached()
    {
        UpdateBarText(_totalBar);
        int colorIndex = _totalBar - 1 >= _colors.Length ? _colors.Length - 1 : _totalBar - 1;
        _imageSkillBarBackground.color = _colors[colorIndex];
        _imageSkillBarFill.color = _colors[colorIndex];
        _imageSkillBarFill.fillAmount = 1f;
    }

    /// <summary>
    /// 技能按钮点击回调
    /// </summary>
    private void OnSkillButtonClicked()
    {
        _skillManager.TryStartSkill();
    }
    #endregion
}