using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 雪山胜利结算弹窗
/// </summary>

public class UIPopupSettlementSnowVictory : UIBase
{
    /// <summary>
    /// 三千盘金币文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textThreeKPCoin;

    /// <summary>
    /// 积分文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textPoint;

    /// <summary>
    /// 结算按钮
    /// </summary>
    [SerializeField] private Button _buttonSettlement;
    
    /// <summary>
    /// 分享按钮
    /// </summary>
    [SerializeField] private Button _buttonShare;
    
    /// <summary>
    /// 骰子按钮
    /// </summary>
    [SerializeField] private Button _buttonDice;

    /// <summary>
    /// 骰子动画预制体
    /// </summary>
    [SerializeField] private Transform _dicePrefab;

    /// <summary>
    /// 骰子动画父物体
    /// </summary>
    [SerializeField] private Transform _diceParent;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager;

    private int _totalCoin;
    private int _totalMastery;
    private Transform _diceTransform;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;

        _buttonSettlement.onClick.AddListener(OnSettlementButtonClicked);
        _buttonShare.onClick.AddListener(OnShareButtonClicked);
        _buttonDice.onClick.AddListener(OnDiceButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonSettlement.onClick.RemoveListener(OnSettlementButtonClicked);
        _buttonShare.onClick.RemoveListener(OnShareButtonClicked);
        _buttonDice.onClick.RemoveListener(OnDiceButtonClicked);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _eventManager.AddListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
    }

    public override void OnClose()
    {
        base.OnClose();
        _eventManager.RemoveListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
    }

    #region 事件相关
    /// <summary>
    /// 结算按钮点击事件回调
    /// </summary>
    private async void OnSettlementButtonClicked()
    {
        Close();
        await HuntingAppFlow.Instance.EnterPrepareAsync();
        _uiManager.CloseUI("UISnowMountainVictory");
    }

    /// <summary>
    /// 分享按钮点击事件回调
    /// </summary>
    private void OnShareButtonClicked()
    {
        // TODO: 分享
    }

    /// <summary>
    /// 骰子按钮点击事件回调
    /// </summary>
    private async void OnDiceButtonClicked()
    {
        _buttonDice.enabled = false;
        // 掷骰子动画
        int point = UnityEngine.Random.Range(1, 7);
        if (!_diceTransform)
        {
            _diceTransform = Instantiate(_dicePrefab, _diceParent);
        }
        await _diceTransform.GetComponent<DiceRollAnimator>().PlayRoll(point);
        
        _textThreeKPCoin.text = (point * _totalCoin).ToString();
        _textPoint.text = (point * _totalMastery).ToString();
    }

    /// <summary>
    /// 结算计算完成事件回调
    /// </summary>
    private void OnSettlementCalculated(SettlementCalculatedEventArgs obj)
    {
        _totalCoin = obj.TotalCoin;
        _totalMastery = obj.TotalMastery;
        _textThreeKPCoin.text = obj.TotalCoin.ToString();
        _textPoint.text = obj.TotalMastery.ToString();
    }

    #endregion
}