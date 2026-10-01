using GameFramework.Utility;
using Cysharp.Threading.Tasks;

/// <summary>
/// 结算管理器
/// </summary>
public class SettlementManager : IRoundManager
{
    private HuntingConfigManager _configManager;
    private MeatProgressManager _meatProgressManager;
    private PlayerDataManager _playerDataManager;
    private RoundNumericLayer _numeric;

    public UniTask InitAsync(RoundContext context)
    {
        BindServices();
        Log.Info("[SettlementManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        Log.Info("[SettlementManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 计算结算奖励
    /// </summary>
    public SettlementReward EvaluateReward()
    {
        int totalMeat = _meatProgressManager.TotalMeatAcrossMaps + _meatProgressManager.CurrentMeatValue;
        var draft = new SettlementDraft
        {
            DisplayMeat = totalMeat,
            CoinFromMeat = _meatProgressManager.GetThreeKPCoinCountFromMeat(totalMeat),
            Point = _configManager.GetSettlementPointRewardAmount()
        };

        _numeric.EvaluateSettlement(draft);

        return new SettlementReward
        {
            DisplayMeat = draft.DisplayMeat,
            CoinFromMeat = draft.CoinFromMeat,
            Point = draft.Point
        };
    }

    /// <summary>
    /// 领取肉换金币
    /// </summary>
    public int ClaimMeatCoin()
    {
        int coin = EvaluateReward().CoinFromMeat;
        _playerDataManager.UpdateThreeKPCoinAmount(coin);
        return coin;
    }
    #endregion

    #region 私有方法
    private void BindServices()
    {
        _configManager = GameServiceLocator.ConfigManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
    }
    #endregion
}
