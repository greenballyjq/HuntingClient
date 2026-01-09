using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 结算弹窗
/// </summary>
public class UISettlement : UIBase
{
    /// <summary>
    /// 金币文本
    /// </summary>
    [SerializeField] private Text textCoin;

    /// <summary>
    /// 熟练度文本
    /// </summary>
    [SerializeField] private Text textMastery;

    /// <summary>
    /// 普通结算按钮
    /// </summary>
    [SerializeField] private Button buttonSettlement;

    /// <summary>
    /// 翻倍结算按钮
    /// </summary>
    [SerializeField] private Button buttonDoubleSettlement;

    /// <summary>
    /// 结算管理器
    /// </summary>
    private SettlementRewardManager _settlementRewardManager => GameServiceLocator.GetRoundManager<SettlementRewardManager>();

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    private GameObjectPoolManager _gameObjectPoolManager => GameServiceLocator.GameObjectPoolManager;

    private void Awake()
    {
        buttonSettlement.onClick.AddListener(OnClickSettlement);
        buttonDoubleSettlement.onClick.AddListener(OnClickDoubleSettlement);
    }

    private void OnDestroy()
    {
        buttonSettlement.onClick.RemoveListener(OnClickSettlement);
        buttonDoubleSettlement.onClick.RemoveListener(OnClickDoubleSettlement);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _eventManager.AddListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
    }

    public override void OnClose()
    {
        _eventManager.RemoveListener(SettlementEvents.SettlementCalculated, OnSettlementCalculated);
        base.OnClose();
    }

    #region 事件相关
    /// <summary>
    /// 普通结算按钮回调
    /// </summary>
    private async void OnClickSettlement()
    {
        Close();

        //TODO : 测试代码，正式版本需要移除
        _uiManager.CloseUI("UIHuntingGameplay");
        await HuntingAppFlow.Instance.EnterPrepareAsync();
        _gameObjectPoolManager.ClearAllPools();
        SceneManager.LoadSceneAsync("PrepareScene").completed += async (ao) =>
        {
            await _uiManager.OpenUIAsync<UIPrepare>("UIPrepare");
            Time.timeScale = 1;
        };
    }

    /// <summary>
    /// 翻倍结算按钮回调
    /// </summary>
    private void OnClickDoubleSettlement()
    {
        _settlementRewardManager.SetRewardDouble();
        buttonDoubleSettlement.gameObject.SetActive(false);
    }

    /// <summary>
    /// 结算数据更新
    /// </summary>
    private void OnSettlementCalculated(SettlementCalculatedEventArgs args)
    {
        // 更新界面上的金币与熟练度
        textCoin.text = args.TotalCoin.ToString();
        textMastery.text = args.TotalMastery.ToString();
    }
    #endregion
}
