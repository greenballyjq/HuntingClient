using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Game;
using GameFramework.Manager;
using Hunting.Manager;
using Hunting.Round;
using Hunting.UI;
using UnityEngine;

public class HuntingLogic : GameLogic
{
    private UIManager UI => GetFrameworkManager<UIManager>();
    
    /// <summary>
    /// 单局流程控制器
    /// </summary>
    private RoundController _roundController;
   
    protected override void RegisterGameManagers()
    {
        Debug.Log("[HuntingGame] 开始注册游戏业务管理器");
        RegisterGameManager<RoundManager>();
        RegisterGameManager<InputManager>();
        RegisterGameManager<PlayerControlManager>();
        RegisterGameManager<PlayerDataManager>();
        RegisterGameManager<LuckyBuffManager>();
        RegisterGameManager<MeatProgressManager>();
        RegisterGameManager<EnergyProgressManager>();
        RegisterGameManager<SettlementRewardManager>();
        RegisterGameManager<WeaponManager>();
        RegisterGameManager<BulletManager>();
        RegisterGameManager<AnimalManager>();
        RegisterGameManager<SpawnerManager>();
        RegisterGameManager<TrapManager>();
        RegisterGameManager<PropManager>();
        RegisterGameManager<SkillManager>();
        RegisterGameManager<QuestManager>();
        Debug.Log("[HuntingGame] 注册游戏业务管理器完成");
    }

    protected override BaseConfigManager GetConfigManager()
    {
        return HuntingConfigManager.Instance;
    }

    protected override async UniTask OnGameInitAsync()
    {
        UIPrepare uiHuntingPrepare = await UI.OpenUIAsync<UIPrepare>("UIPrepare");
    }

    /// <summary>
    /// 进入单局
    /// </summary>
    /// <param name="context">单局上下文</param>
    public void EnterRound(RoundContext context)
    {
        _roundController = new RoundController();
        _roundController.StartRound(context);
        // TODO: 后续接入状态机/场景切换
    }

    protected override void Update()
    {
        base.Update();
        _roundController?.Update(Time.deltaTime);
        // TODO: 状态机落地后按状态驱动 RoundController
    }
}
