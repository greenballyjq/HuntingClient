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
    [SerializeField] private TextMeshProUGUI _textEnergyBars;

    /// <summary>
    /// 技能按钮
    /// </summary>
    [SerializeField] private Button _buttonSkill;

    /// <summary>
    /// 技能图像
    /// </summary>
    [SerializeField] private Image _imageSkill;

    /// <summary>
    /// 颜色数组
    /// </summary>
    [SerializeField] private Color[] _colors;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 技能管理器
    /// </summary>
    private SkillManager _skillManager => GameServiceLocator.GetRoundManager<SkillManager>();

    /// <summary>
    /// 丰收能量条管理器
    /// </summary>
    private EnergyProgressManager _energyProgressManager => GameServiceLocator.GetRoundManager<EnergyProgressManager>();

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    private void Awake()
    {
        _buttonSkill.onClick.AddListener(OnSkillButtonClicked);
    }

    public void Init()
    {
        float currentEnergy = _energyProgressManager.GetCurrentEnergyValue();
        int currentBars = _energyProgressManager.GetCompletedBars();
        int totalBar = _energyProgressManager.GetTotalBar();
        UpdateFill(currentEnergy, currentBars);
        UpdateBarText(currentBars, totalBar);
        UpdateBackgroundColor(currentBars);

        _eventManager.AddListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
        _eventManager.AddListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
        _eventManager.AddListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);
        _eventManager.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(EnergyEvents.EnergyProgressChanged, OnEnergyProgressChanged);
        _eventManager.RemoveListener(EnergyEvents.EnergyBarCountChanged, OnEnergyBarCountChanged);
        _eventManager.RemoveListener(EnergyEvents.EnergyMaxBarsReached, OnEnergyMaxBarsReached);
        _eventManager.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
    }

    private void OnDestroy()
    {
        _buttonSkill.onClick.RemoveListener(OnSkillButtonClicked);
        CleanUp();
    }

    #region 更新逻辑
    /// <summary>
    /// 更新填充图像
    /// </summary>
    private void UpdateFill(float currentEnergy, int currentBars)
    {
        _imageEnergyFill.fillAmount = currentEnergy / _energyProgressManager.GetValuePerBar();
        int colorIndex = currentBars >= _colors.Length ? _colors.Length - 1 : currentBars;
        _imageEnergyFill.color = _colors[colorIndex];
    }

    /// <summary>
    /// 更新文本
    /// </summary>
    private void UpdateBarText(int currentBars, int maxBars)
    {
        _textEnergyBars.text = $"{currentBars}/{maxBars}";
    }

    /// <summary>
    /// 更新底图颜色
    /// </summary>
    private void UpdateBackgroundColor(int currentBars)
    {
        if (currentBars <= 0)
        {
            _imageEnergyBackground.color = Color.clear;
            return;
        }
        int colorIndex = currentBars - 1 >= _colors.Length ? _colors.Length - 1 : currentBars - 1;
        _imageEnergyBackground.color = _colors[colorIndex];
    }


    /// <summary>
    /// 异步加载技能图标
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    private async UniTask LoadSkillIconAsync(string assetPath)
    {
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(assetPath);
        _imageSkill.sprite = sprite;
        Debug.Log($"加载技能图标: {assetPath}");
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
        UpdateBarText(args.CurrentBars, _energyProgressManager.GetTotalBar());
        UpdateBackgroundColor(args.CurrentBars);
        UpdateFill(_energyProgressManager.GetCurrentEnergyValue(), args.CurrentBars);
    }

    /// <summary>
    /// 能量条上限回调
    /// </summary>
    private void OnEnergyMaxBarsReached()
    {
        int maxBars = _energyProgressManager.GetTotalBar();
        UpdateBarText(maxBars, maxBars);
        int colorIndex = maxBars - 1 >= _colors.Length ? _colors.Length - 1 : maxBars - 1;
        _imageEnergyBackground.color = _colors[colorIndex];
        _imageEnergyFill.color = _colors[colorIndex];
        _imageEnergyFill.fillAmount = 1f;
    }

    /// <summary>
    /// 点击技能按钮回调
    /// </summary>
    private void OnSkillButtonClicked()
    {
        _skillManager.TryStartSkill();
    }

    /// <summary>
    /// 单局开始事件回调
    /// </summary>
    private void OnRoundStarted(RoundStartedEventArgs args)
    {
        LoadSkillIconAsync(args.RoundContext.SkillData.IconResourcePath).Forget();
    }
    #endregion
}