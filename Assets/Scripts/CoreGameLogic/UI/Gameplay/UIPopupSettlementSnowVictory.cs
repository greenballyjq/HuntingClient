using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    /// 确认按钮
    /// </summary>
    [SerializeField] private Button _buttonConfirm;
    
    /// <summary>
    /// 分享按钮
    /// </summary>
    [SerializeField] private Button _buttonShare;
    
    /// <summary>
    /// 掷骰子按钮
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
    
    private int _totalCoin;
    private int _totalMastery;

    private Transform _diceTransform;

    private void Awake()
    {
        _buttonConfirm.onClick.AddListener(OnSettleAndReturnButtonClick);
        _buttonShare.onClick.AddListener(OnShareButtonClick);
        _buttonDice.onClick.AddListener(OnDiceButtonClick);
    }

    private void OnDestroy()
    {
        _buttonConfirm.onClick.RemoveListener(OnSettleAndReturnButtonClick);
        _buttonShare.onClick.RemoveListener(OnShareButtonClick);
        _buttonDice.onClick.RemoveListener(OnDiceButtonClick);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        var settlementRewardManager = GameServiceLocator.GetRoundManager<SettlementRewardManager>();
        settlementRewardManager.CalculateReward();
        var eventManager = GameServiceLocator.EventManager;
        eventManager.AddListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
    }

    public override void OnClose()
    {
        base.OnClose();
        var eventManager = GameServiceLocator.EventManager;
        eventManager.RemoveListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
    }

    private void OnSettlementCalculated(SettlementCalculatedEventArgs obj)
    {
        _totalCoin = obj.TotalCoin;
        _totalMastery = obj.TotalMastery;
        _textThreeKPCoin.text = obj.TotalCoin.ToString();
        _textPoint.text = obj.TotalMastery.ToString();
    }

    private void OnSettleAndReturnButtonClick()
    {
        HuntingAppFlow.Instance.EnterPrepareAsync().Forget();
    }

    private void OnShareButtonClick()
    {
        // TODO: 分享
    }
    
    private async void OnDiceButtonClick()
    {
        // 掷骰子动画
        int point = UnityEngine.Random.Range(0, 7);
        if (!_diceTransform)
        {
            _diceTransform = Instantiate(_dicePrefab, _diceParent);
        }
        await _diceTransform.GetComponent<DiceAnimation>().PlayRoll(point);
        
        _textThreeKPCoin.text = (point * _totalCoin).ToString();
        _textPoint.text = (point * _totalMastery).ToString();
    }
}