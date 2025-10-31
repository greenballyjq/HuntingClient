using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Manager;
using UnityEngine;

namespace Hunting
{
    public class HuntingGame : GameLogic
    {
        protected override void RegisterGameManagers()
        {
            Debug.Log("[HuntingGame] 开始注册游戏业务管理器");
            // 注册所有需要的业务管理器
            Debug.Log("[HuntingGame] 注册游戏业务管理器完成");
        }

        protected override BaseConfigManager GetConfigManager()
        {
            return HuntingGameConfigManager.Instance;
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
