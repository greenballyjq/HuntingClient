using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPopupSettlementSnowVictory : UIBase
{
    /// <summary>
    /// 金币文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCoin;

    /// <summary>
    /// 熟练度文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textMastery;

    /// <summary>
    /// 回到主菜单按钮
    /// </summary>
    [SerializeField] private Button _settleAndReturnButton;
    
    /// <summary>
    /// 分享按钮
    /// </summary>
    [SerializeField] private Button _shareButton;
    
    /// <summary>
    /// 掷骰子按钮
    /// </summary>
    [SerializeField] private Button _diceButton;

    /// <summary>
    /// 骰子动画预制体
    /// </summary>
    [SerializeField] private Transform _dicePrefab;

    /// <summary>
    /// 骰子动画父物体
    /// </summary>
    [SerializeField] private Transform _diceParent;

    private UIManager _uiManager => GameServiceLocator.UIManager;
    
    private int _totalCoin;
    private int _totalMastery;

    private Transform _diceTransform;

    private void Awake()
    {
        _settleAndReturnButton.onClick.AddListener(OnSettleAndReturnButtonClick);
        _shareButton.onClick.AddListener(OnShareButtonClick);
        _diceButton.onClick.AddListener(OnDiceButtonClick);
    }

    private void OnDestroy()
    {
        _settleAndReturnButton.onClick.RemoveListener(OnSettleAndReturnButtonClick);
        _shareButton.onClick.RemoveListener(OnShareButtonClick);
        _diceButton.onClick.RemoveListener(OnDiceButtonClick);
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
        _textCoin.text = obj.TotalCoin.ToString();
        _textMastery.text = obj.TotalMastery.ToString();
    }

    private async void OnSettleAndReturnButtonClick()
    {
        // 返回主菜单
        await SceneManager.LoadSceneAsync("PrepareScene").ToUniTask();
        _uiManager.CloseUI("UIPopupSettlementSnowVictory");
        _uiManager.CloseUI("UIGameplay");
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
        
        _textCoin.text = (point * _totalCoin).ToString();
        _textMastery.text = (point * _totalMastery).ToString();
    }
}