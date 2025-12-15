using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Game;
using GameFramework.Manager;
using Hunting.Manager;
using Hunting.UI;
using UnityEngine;

public class HuntingLogic : GameLogic
{
    private UIManager UI => GetFrameworkManager<UIManager>();

    protected override void RegisterGameManagers()
    {
        Debug.Log("[HuntingGame] 开始注册游戏业务管理器");
        RegisterGameManager<RoundManager>();
        RegisterGameManager<InputManager>();
        RegisterGameManager<PlayerControlManager>();
        RegisterGameManager<PlayerDataManager>();
        RegisterGameManager<LuckyManager>();
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
        return HuntingGameConfigManager.Instance;
    }

    protected override async UniTask OnGameInitAsync()
    {
        UIPrepare uiHuntingPrepare = await UI.OpenUIAsync<UIPrepare>("UIPrepare");
    }
}
