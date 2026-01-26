using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 返回/结算按钮组件
/// </summary>
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
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 结算管理器
    /// </summary>
    private SettlementRewardManager _settlementRewardManager = GameServiceLocator.GetRoundManager<SettlementRewardManager>();

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
    }

    private void OnDestroy()
    {
        _buttonReturnOrSettlement.onClick.RemoveListener(OnReturnOrSettlementButtonClicked);
    }

    public void Init()
    {
        _textReturnOrSettlement.text = "返回";
        _isSettlementMode = false;
        _isMeatScaleFull = false;
        _hasHiddenMap = false;

        _eventManager.AddListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
        _eventManager.AddListener(MeatEvents.MeatScaleFull, OnMeatScaleFull);
        _eventManager.AddListener(RoundEvents.RoundStarted, OnRoundStarted);

        Debug.Log("[UIComponentReturnOrSettlement] 初始化完成");
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
        _eventManager.RemoveListener(MeatEvents.MeatScaleFull, OnMeatScaleFull);

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
    /// 单局开始事件回调
    /// </summary>
    private void OnRoundStarted(RoundStartedEventArgs args)
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
            if (_isMeatScaleFull && _hasHiddenMap)
                await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
            else
                await _uiManager.OpenUIAsync<UIPopupSettlementNormal>("UIPopupSettlementNormal", UIManager.UILayer.PopUp);

            _settlementRewardManager.CalculateReward();
        }
        else
        {   
            // TODO: 将来可能正式化
            _uiManager.CloseUI("UIGameplay");
            await HuntingAppFlow.Instance.EnterPrepareAsync();
            SceneManager.LoadSceneAsync("PrepareScene").completed += async (ao) =>
            {
                await _uiManager.OpenUIAsync<UIPrepare>("UIPrepare");
            };
        }
    }
    #endregion
}
