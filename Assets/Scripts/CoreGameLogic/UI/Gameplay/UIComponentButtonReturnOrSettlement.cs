using System;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 返回/结算按钮组件
/// </summary>
[Obsolete("现已使用自动结算模式，无需手动结算按钮")]
public class UIComponentButtonReturnOrSettlement : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 返回/结算按钮
    /// </summary>
    [SerializeField] private Button _buttonReturnOrSettlement;

    /// <summary>
    /// 返回/结算文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textReturnOrSettlement;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager;

    /// <summary>
    /// 结算管理器
    /// </summary>
    private SettlementManager _settlementRewardManager;

    /// <summary>
    /// 单局流程
    /// </summary>
    private RoundFlow _roundFlow;

    /// <summary>
    /// 返回/结算按钮矩形变换组件
    /// </summary>
    private RectTransform _rectTransformReturnOrSettlementButton;
    public RectTransform RectTransformReturnOrSettlementButton => _rectTransformReturnOrSettlementButton;

    /// <summary>
    /// 是否为结算模式
    /// </summary>
    private bool _isSettlementMode;

    /// <summary>
    /// 肉条刻度是否集满
    /// </summary>
    private bool _isMeatScaleFull;

    /// <summary>
    /// 是否有隐藏地图
    /// </summary>
    private bool _hasHiddenMap;

    private void Awake()
    {
        _buttonReturnOrSettlement.onClick.AddListener(OnReturnOrSettlementButtonClicked);
        _rectTransformReturnOrSettlementButton = _buttonReturnOrSettlement.GetComponent<RectTransform>();

        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _settlementRewardManager = GameServiceLocator.GetRoundManager<SettlementManager>();
        _roundFlow = RoundFlow.Instance;
    }

    private void OnDestroy()
    {
        _buttonReturnOrSettlement.onClick.RemoveListener(OnReturnOrSettlementButtonClicked);
    }

    public void Init()
    {
        _isSettlementMode = false;
        _isMeatScaleFull = false;
        _hasHiddenMap = false;

        _eventManager.AddListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
        _eventManager.AddListener(MeatEvents.MeatScaleFull, OnMeatScaleFull);
        _eventManager.AddListener(RoundEvents.RoundEntered, OnRoundEntered);

        Debug.Log("[UIComponentReturnOrSettlement] 初始化完成");
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
        _eventManager.RemoveListener(MeatEvents.MeatScaleFull, OnMeatScaleFull);
        _eventManager.RemoveListener(RoundEvents.RoundEntered, OnRoundEntered);

        Debug.Log("[UIComponentReturnOrSettlement] 已清理");
    }

    #region 事件相关
    /// <summary>
    /// 肉条刻度完成事件回调
    /// </summary>
    private void OnMeatScaleCompleted(MeatScaleCompletedEventArgs args)
    {
        _isSettlementMode = true;
        _textReturnOrSettlement.text = "结算";
    }

    /// <summary>
    /// 肉条满事件回调
    /// </summary>
    private void OnMeatScaleFull()
    {
        _isMeatScaleFull = true;
    }

    /// <summary>
    /// 进入单局事件回调
    /// </summary>
    private void OnRoundEntered(RoundEnteredEventArgs args)
    {
        _hasHiddenMap = args.RoundContext.HasHiddenMap;
    }

    /// <summary>
    /// 返回/结算按钮点击事件回调
    /// </summary>
    private async void OnReturnOrSettlementButtonClicked()
    {
        if (_isSettlementMode)
        {
            _roundFlow.StartSettlement();
            if (_isMeatScaleFull && _hasHiddenMap)
                await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
            else
                await _uiManager.OpenUIAsync<UIPopupSettlementNormal>("UIPopupSettlementNormal", UIManager.UILayer.PopUp);

            _settlementRewardManager.CalculateReward();
        }
        else
        {
            _roundFlow.StartSettlement();
            await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
            _settlementRewardManager.CalculateReward();
        }
    }
    #endregion
}
