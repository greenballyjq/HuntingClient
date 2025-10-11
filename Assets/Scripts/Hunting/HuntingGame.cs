using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GooseCatcher.Manager;
using UnityEngine;

namespace Hunting
{
    public class HuntingGame : GameLogic
    {
        // 游戏管理器引用
        private readonly Dictionary<Type, BaseGameManager> _gameManagers = new Dictionary<Type, BaseGameManager>();

        private void RegisterManager<T>() where T : BaseGameManager
        {
            Debug.Log($"[HuntingGame] 注册{typeof(T)}管理器");
            T manager = FindObjectOfType<T>();
            if (manager == null)
            {
                manager = new GameObject(typeof(T).Name).AddComponent<T>();
                Debug.Log($"[HuntingGame] {typeof(T)}不存在, 创建{typeof(T)}管理器");
            }
            _gameManagers.Add(typeof(T), manager);
        }

        public T GetManager<T>() where T : BaseGameManager
        {
            if (_gameManagers.TryGetValue(typeof(T), out var manager))
            {
                return manager as T;
            }
            Debug.LogError($"[HuntingGame] {typeof(T)} 管理器不存在");
            return null;
        }

        private async void InitializeGameManagers()
        {
            Debug.Log("[HuntingGame] 游戏管理器引用初始化开始");
            
            
            
            // 调用管理器的Init方法
            Debug.Log("[HuntingGame] 开始调用管理器的Init方法");
            await GameConfigManager.WaitForLoadComplete();
            foreach (var manager in _gameManagers.Values)
            {
                manager.Init();
            }

            Debug.Log("[HuntingGame] 管理器的Init方法调用完成");
        }
        
        protected override async UniTask OnGameInit()
        {
            Debug.Log($"[HuntingGame] HuntingGame 游戏初始化");
            await UniTask.Delay(100);

            InitializeGameManagers();
        }

        protected override void OnGameReady()
        {
            Debug.Log($"[HuntingGame] HuntingGame 游戏准备");
        }

        protected override void OnGameStart()
        {
            Debug.Log($"[HuntingGame] HuntingGame 游戏开始");
            
            Debug.Log($"[HuntingGame] 开始调用管理器的StartGame方法");
            foreach (var manager in _gameManagers.Values)
            {
                manager.StartGame();
            }

            Debug.Log($"[HuntingGame] 调用管理器的StartGame方法完成");
        }

        protected override void OnGamePause()
        {
            Debug.Log($"[HuntingGame] HuntingGame 游戏暂停");
        }

        protected override void OnGameResume()
        {
            Debug.Log($"[HuntingGame] HuntingGame 游戏恢复");
        }

        protected override void OnGameEnd()
        {
            Debug.Log($"[HuntingGame] HuntingGame 游戏结束");
        }

        protected override void OnGamePlaying()
        {
            
        }

        protected override void OnGamePaused()
        {
        }

        protected override void OnGameOver()
        {
        }
    }
}
