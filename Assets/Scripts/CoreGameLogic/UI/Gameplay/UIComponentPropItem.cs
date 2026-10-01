using Cysharp.Threading.Tasks;
using cfg.HuntingConfig.Enum;
using UnityEngine;
using UnityEngine.UI;
using GameFramework.Manager;
using GameFramework.UI;
using TMPro;

/// <summary>
/// 道具UI组件
/// </summary>
public class UIComponentPropItem : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 道具按钮
    /// </summary>
    [SerializeField] private Button _buttonProp;

    /// <summary>
    /// 加号按钮
    /// </summary>
    [SerializeField] private Button _buttonAdd;

    /// <summary>
    /// 道具图像
    /// </summary>
    [SerializeField] private Image _imageProp;

    /// <summary>
    /// 加号图像
    /// </summary>
    [SerializeField] private Image _imageAdd;

    /// <summary>
    /// 冷却遮罩图像
    /// </summary>
    [SerializeField] private Image _imageCooldownMask;

    /// <summary>
    /// 道具数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCount;

    /// <summary>
    /// 道具类型
    /// </summary>
    [SerializeField] private EPropType _propType;

    /// <summary>
    /// 冷却剩余时间
    /// </summary>
    private float _cooldownRemainingTime;

    /// <summary>
    /// 冷却总时间
    /// </summary>
    private float _cooldownTotalTime;

    private RoundFlow _roundFlow => HuntingAppFlow.Instance.RoundFlow;
    private EventManager _eventManager;
    private UIManager _uiManager;
    private PropManager _propManager;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        BindServices();
    }

    public void Init()
    {
        _buttonProp.onClick.AddListener(OnPropIconClicked);
        _buttonAdd.onClick.AddListener(OnAddButtonClicked);

        _eventManager.AddListener(PropEvents.PropStarted, OnPropStarted);
        _eventManager.AddListener(PropEvents.PropUpdated, OnPropUpdated);
        _eventManager.AddListener(PropEvents.PropEnded, OnPropEnded);
        _eventManager.AddListener(PlayerDataEvents.PropCountChanged, OnPropCountChanged);

        InitializeDisplay();
    }

    public void CleanUp()
    {
        _buttonProp.onClick.RemoveListener(OnPropIconClicked);
        _buttonAdd.onClick.RemoveListener(OnAddButtonClicked);

        _eventManager.RemoveListener(PropEvents.PropStarted, OnPropStarted);
        _eventManager.RemoveListener(PropEvents.PropUpdated, OnPropUpdated);
        _eventManager.RemoveListener(PropEvents.PropEnded, OnPropEnded);
        _eventManager.RemoveListener(PlayerDataEvents.PropCountChanged, OnPropCountChanged);
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
        _propManager = GameServiceLocator.GetRoundManager<PropManager>();
    }

    /// <summary>
    /// 初始化显示
    /// </summary>
    private void InitializeDisplay()
    {
        RefreshCount();

        _imageCooldownMask.fillAmount = 0f;
    }

    /// <summary>
    /// 开始冷却
    /// </summary>
    private void StartCooldown(float duration)
    {
        _cooldownTotalTime = duration;
        _cooldownRemainingTime = duration;

        _buttonProp.interactable = false;

        _imageCooldownMask.fillAmount = 1f;
    }

    /// <summary>
    /// 刷新冷却显示
    /// </summary>
    private void RefreshCooldown(float remainingTime, float totalTime)
    {
        float progress = remainingTime / totalTime;
        _imageCooldownMask.fillAmount = progress;
    }

    /// <summary>
    /// 刷新道具数量显示
    /// </summary>
    private void RefreshCount()
    {
        _textCount.text = _playerDataManager.GetPropCount(_propType).ToString();
    }

    /// <summary>
    /// 结束冷却
    /// </summary>
    private void EndCooldown()
    {
        _cooldownRemainingTime = 0f;
        _cooldownTotalTime = 0f;

        _buttonProp.interactable = true;

        _imageCooldownMask.fillAmount = 0f;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 道具图标按钮点击回调
    /// </summary>
    private void OnPropIconClicked()
    {
        _propManager.TryStartProp(_propType);
    }

    /// <summary>
    /// 加号按钮点击回调
    /// </summary>
    private async void OnAddButtonClicked()
    {
        _roundFlow.PauseRound();
        await _uiManager.OpenAsync<UIPopupProp, EPropType>(_propType);
    }

    /// <summary>
    /// 道具开始事件回调
    /// </summary>
    private void OnPropStarted(PropStartedEventArgs args)
    {
            if (args.PropData.PropType != _propType)
            return;

        StartCooldown(args.PropData.Duration);
    }

    /// <summary>
    /// 道具更新事件回调
    /// </summary>
    private void OnPropUpdated(PropUpdatedEventArgs args)
    {
        if (args.PropData.PropType != _propType)
            return;

        _cooldownRemainingTime = args.RemainingTime;
        RefreshCooldown(args.RemainingTime, args.PropData.Duration);
    }

    /// <summary>
    /// 道具结束事件回调
    /// </summary>
    private void OnPropEnded(PropEndedEventArgs args)
    {
        if (args.PropData.PropType != _propType)
            return;

        EndCooldown();

        RefreshCount();
    }

    /// <summary>
    /// 道具数量改变事件回调
    /// </summary>
    private void OnPropCountChanged(PropCountChangedEventArgs args)
    {
        if (args.PropType != _propType)
            return;
        RefreshCount();
    }
    #endregion
}