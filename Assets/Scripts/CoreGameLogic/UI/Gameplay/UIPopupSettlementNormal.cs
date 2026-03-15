using System;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 结算弹窗
/// </summary>
public class UIPopupSettlementNormal : UIBase
{
    /// <summary>
    /// 三千盘金币文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI textThreeKPCoin;

    /// <summary>
    /// 积分文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI textPoint;

    /// <summary>
    /// 结算按钮
    /// </summary>
    [SerializeField] private Button buttonSettlement;

    /// <summary>
    /// 翻倍结算按钮
    /// </summary>
    [SerializeField] private Button buttonDoubleSettlement;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// 结算管理器
    /// </summary>
    private SettlementManager _settlementRewardManager;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _settlementRewardManager = GameServiceLocator.GetRoundManager<SettlementManager>();

        buttonSettlement.onClick.AddListener(OnSettlementButtonClicked);
        buttonDoubleSettlement.onClick.AddListener(OnDoubleSettlementClicked);
    }

    private void OnDestroy()
    {
        buttonSettlement.onClick.RemoveListener(OnSettlementButtonClicked);
        buttonDoubleSettlement.onClick.RemoveListener(OnDoubleSettlementClicked);
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
    /// 结算按钮点击事件回调
    /// </summary>
    private void OnSettlementButtonClicked()
    {
        Close();
        HuntingAppFlow.Instance.EnterPrepareAsync().Forget();
    }

    /// <summary>
    /// 翻倍结算按钮点击事件回调
    /// </summary>
    private void OnDoubleSettlementClicked()
    {
        _settlementRewardManager.SetRewardDouble();
        buttonDoubleSettlement.gameObject.SetActive(false);
    }

    /// <summary>
    /// 结算数据更新
    /// </summary>
    private void OnSettlementCalculated(SettlementCalculatedEventArgs args)
    {
        textThreeKPCoin.text = args.TotalCoin.ToString();
        textPoint.text = args.TotalMastery.ToString();
    }
    #endregion
}
