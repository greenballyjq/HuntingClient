using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Manager;
using Hunting.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hunting
{
    public class HuntingGame : GameLogic
    {
        protected override void RegisterGameManagers()
        {
            Debug.Log("[HuntingGame] 开始注册游戏业务管理器");
            RegisterManager<SpawnerManager>();
            RegisterManager<AnimalManager>();
            RegisterManager<SettlementRewardManager>();
            Debug.Log("[HuntingGame] 注册游戏业务管理器完成");
        }

        protected override BaseConfigManager GetConfigManager()
        {
            return HuntingGameConfigManager.Instance;
        }

        protected override async UniTask OnGameInit()
        {
            UIHuntingPrepare uiHuntingPrepare = await GetFrameworkManager<UIManager>().OpenUIAsync<UIHuntingPrepare>("UIHuntingPrepare");
            GameLauncher.Instance.OnCompleteLauncher();
            uiHuntingPrepare.SetGameLogic(this);
        }

        protected override void OnGameStart()
        {
            Debug.Log("[HuntingGame] 游戏开始");
        }

        protected override void OnGamePause()
        {
            Debug.Log("[HuntingGame] 游戏暂停");
        }

        protected override void OnGameResume()
        {
            Debug.Log("[HuntingGame] 游戏恢复");
        }

        protected override void OnGameEnd()
        {
            Debug.Log("[HuntingGame] 游戏结束");
        }

        protected override void OnGamePlaying()
        {
            // 游戏进行中的逻辑
        }

        protected override void OnGamePaused()
        {
            // 游戏暂停中的逻辑
        }

        protected override void OnGameOver()
        {
            // 游戏结束后的逻辑
        }
    }
}
