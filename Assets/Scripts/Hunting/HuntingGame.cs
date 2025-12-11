using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Manager;
using Hunting.UI;
using UnityEngine;

namespace Hunting
{
    public class HuntingGame : GameLogic
    {

        private UIManager UI => GetFrameworkManager<UIManager>();

        private ResourceManager Resource => GetFrameworkManager<ResourceManager>();

        private HuntingGameConfigManager Config => HuntingGameConfigManager.Instance;

        // TODO: 待转到专门负责资源预加载的类中
        private readonly HashSet<string> _preloadedAssetPaths = new HashSet<string>();

        protected override void RegisterGameManagers()
        {
            Debug.Log("[HuntingGame] 开始注册游戏业务管理器");
            RegisterManager<InputManager>();
            RegisterManager<PlayerControlManager>();
            RegisterManager<RoundManager>();
            RegisterManager<SpawnerManager>();
            RegisterManager<AnimalManager>();
            RegisterManager<BulletManager>();
            RegisterManager<WeaponManager>();
            RegisterManager<MeatProgressManager>();
            RegisterManager<EnergyProgressManager>();
            RegisterManager<SettlementRewardManager>();
            RegisterManager<SkillManager>();
            RegisterManager<LuckyManager>();
            RegisterManager<QuestManager>();
            RegisterManager<PropManager>();
            RegisterManager<TrapManager>();
            RegisterManager<PlayerDataManager>();
            Debug.Log("[HuntingGame] 注册游戏业务管理器完成");
        }

        protected override BaseConfigManager GetConfigManager()
        {
            return HuntingGameConfigManager.Instance;
        }

        protected override async UniTask OnGameInit()
        {
            await PreloadPrepareAssetsAsync();
            UIPrepare uiHuntingPrepare = await UI.OpenUIAsync<UIPrepare>("UIPrepare");
            GameLauncher.Instance.OnCompleteLauncher();
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

        #region TODO: 待转到专门负责资源预加载的类中
        /// <summary>
        /// 预加载准备界面所需的静态资源
        /// </summary>
        private async UniTask PreloadPrepareAssetsAsync()
        {
            Debug.Log("[HuntingGame] 预加载准备界面资源");

            var loadTasks = new List<UniTask>();

            foreach (var role in Config.RoleTable.DataList)
            {
                var path = role.RoleImageResourcePath;
                if (string.IsNullOrEmpty(path) || path == "/" || !_preloadedAssetPaths.Add(path))
                {
                    continue;
                }

                loadTasks.Add(Resource.LoadAssetAsync<Sprite>(path));
            }

            foreach (var map in Config.MapTable.DataList)
            {
                var path = map.MapImageResourcePath;
                if (string.IsNullOrEmpty(path) || path == "/" || !_preloadedAssetPaths.Add(path))
                {
                    continue;
                }

                loadTasks.Add(Resource.LoadAssetAsync<Sprite>(path));
            }

            if (loadTasks.Count > 0)
            {
                await UniTask.WhenAll(loadTasks);
            }
        }
        #endregion
    }
}
