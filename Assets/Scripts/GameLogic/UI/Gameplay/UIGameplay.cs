using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游玩界面主面板
/// </summary>
public class UIGameplay : UIBase
{
    /// <summary>
    /// 结算按钮
    /// </summary>
    [SerializeField] private Button buttonSettlement;

    /// <summary>
    /// 肉度条组件
    /// </summary>
    [SerializeField] private UIComponentMeatProgress uiComponentMeatProgress;

    /// <summary>
    /// 能量条组件
    /// </summary>
    [SerializeField] private UIComponentEnergyProgress uiComponentEnergyProgress;

    /// <summary>
    /// 子弹状态组件
    /// </summary>
    [SerializeField] private UIComponentBulletStatus uiComponentBulletStatus;

    /// <summary>
    /// 道具组组件
    /// </summary>
    [SerializeField] private UIComponentPropGroup uiComponentPropGroup;
    
    /// <summary>
    /// 动态任务组件
    /// </summary>
    //[SerializeField] private UIComponentQuest uiComponentQuest;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 结算管理器
    /// </summary>
    private SettlementRewardManager _settlementRewardManager => GameServiceLocator.GetRoundManager<SettlementRewardManager>();

    private void Awake()
    {
        buttonSettlement.onClick.AddListener(OnSettlementButtonClicked);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        uiComponentMeatProgress.Init();
        uiComponentEnergyProgress.Init();
        uiComponentBulletStatus.Init();
        uiComponentPropGroup.Init();
        //uiComponentQuest.Init();
    }

    public override void OnClose()
    {
        uiComponentPropGroup.CleanUp();
        uiComponentBulletStatus.CleanUp();
        uiComponentEnergyProgress.CleanUp();
        uiComponentMeatProgress.CleanUp();
        //uiComponentQuest.CleanUp();
        base.OnClose();
    }

    private void OnDestroy()
    {
        buttonSettlement.onClick.RemoveListener(OnSettlementButtonClicked);
    }

    #region 私有方法
    /// <summary>
    /// 打开结算面板
    /// </summary>
    private async UniTask OpenSettlementWindowAsync()
    {
        await _uiManager.OpenUIAsync<UISettlement>("UISettlement",UIManager.UILayer.PopUp);

        // TODO: 测试暂停  待换正式
        Time.timeScale = 0;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 结算按钮点击回调
    /// </summary>
    private async void OnSettlementButtonClicked()
    {
        // 打开结算弹窗
        await OpenSettlementWindowAsync();

        // 计算本局奖励
        _settlementRewardManager.CalculateReward();
    }
    #endregion
}
