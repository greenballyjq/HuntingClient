using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Manager;
using UnityEngine;

namespace Hunting
{
    public abstract class GameLogic : MonoBehaviour
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        private static GameLogic _instance;
        public static GameLogic Instance => _instance;

        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum GameState
        {
            None,           // 未初始化
            Initializing,   // 初始化中
            Ready,          // 准备开始
            Playing,        // 游戏中
            Paused,         // 暂停
            GameOver,       // 游戏结束
        }

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentState { get; protected set; } = GameState.None;

        /// <summary>
        /// 游戏业务管理器字典
        /// </summary>
        private readonly Dictionary<Type, IGameManager> _gameManagers = new Dictionary<Type, IGameManager>();
        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual async void Start()
        {
            await InitializeGameAsync();
        }

        protected virtual void Update()
        {
            switch (CurrentState)
            {
                case GameState.Playing:
                    OnGamePlaying();
                    break;
                case GameState.Paused:
                    OnGamePaused();
                    break;
                case GameState.GameOver:
                    OnGameOver();
                    break;
            }
        }

        protected virtual void OnDestroy()
        {
            ReleaseGameManagers();

            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// 异步初始化游戏
        /// </summary>
        private async UniTask InitializeGameAsync()
        {
            Debug.Log("[GameLogic] 开始初始化游戏");
            CurrentState = GameState.Initializing;

            // 等待游戏启动器初始化完成
            await WaitForGameLauncher();

            // 等待配置管理器初始化完成
            await WaitForConfigManager();

            // 注册游戏业务管理器
            RegisterGameManagers();

            // 初始化游戏业务管理器
            InitializeGameManagers();

            // 调用子类初始化
            await OnGameInit();

            // 完成初始化
            CurrentState = GameState.Ready;

            Debug.Log("[GameLogic] 游戏初始化完成");
        }

        /// <summary>
        /// 等待游戏启动器初始化
        /// </summary>
        private async UniTask WaitForGameLauncher()
        {
            Debug.Log("[GameLogic] 等待游戏启动器初始化...");

            if (GameLauncher.Instance == null)
                Debug.LogError("[GameLogic] 未在场景中找到GameLauncher。请确保在初始场景中挂载并配置好GameLauncher组件");

            await GameLauncher.Instance.WaitForInitializationAsync();
            Debug.Log("[GameLogic] 游戏启动器初始化完成");
        }

        /// <summary>
        /// 获取配置管理器实例（由子类实现）
        /// </summary>
        protected abstract BaseConfigManager GetConfigManager();

        /// <summary>
        /// 等待配置管理器初始化
        /// </summary>
        private async UniTask WaitForConfigManager()
        {
            Debug.Log("[GameLogic] 等待配置管理器初始化...");

            var configManager = GetConfigManager();
            configManager.Init();
            await configManager.WaitForInitializationAsync();

            Debug.Log("[GameLogic] 配置管理器初始化完成");
        }

        /// <summary>
        /// 注册游戏业务管理器（由子类实现）
        /// </summary>
        protected abstract void RegisterGameManagers();

        /// <summary>
        /// 注册单个游戏业务管理器
        /// </summary>
        protected void RegisterManager<T>() where T : BaseGameManager
        {
            var managerType = typeof(T);

            if (_gameManagers.ContainsKey(managerType))
            {
                Debug.LogWarning($"[GameLogic] 管理器 {managerType.Name} 已注册");
                return;
            }

            var manager = FindObjectOfType<T>();
            if (manager == null)
            {
                var go = new GameObject(managerType.Name);
                manager = go.AddComponent<T>();
            }

            _gameManagers[managerType] = manager;
            Debug.Log($"[GameLogic] 注册管理器: {managerType.Name}");
        }

        /// <summary>
        /// 初始化所有游戏业务管理器
        /// </summary>
        private void InitializeGameManagers()
        {
            Debug.Log("[GameLogic] 初始化游戏业务管理器");

            foreach (var manager in _gameManagers.Values)
                manager.Init();

            Debug.Log("[GameLogic] 初始化游戏业务管理器完成");
        }

        /// <summary>
        /// 获取框架管理器
        /// </summary>
        public T GetFrameworkManager<T>() where T : class, IManager
        {
            return GameFrameworkManager.Instance.GetManager<T>();
        }

        /// <summary>
        /// 获取游戏业务管理器
        /// </summary>
        public T GetGameManager<T>() where T : class, IGameManager
        {
            return _gameManagers[typeof(T)] as T;
        }

        /// <summary>
        /// 释放所有游戏业务管理器
        /// </summary>
        private void ReleaseGameManagers()
        {
            Debug.Log("[GameLogic] 释放游戏业务管理器");

            foreach (var manager in _gameManagers.Values)
                manager.Release();

            _gameManagers.Clear();
        }

        #region 游戏状态控制方法
        public virtual void StartGame()
        {
            if (CurrentState != GameState.Ready)
            {
                Debug.LogWarning("[GameLogic] 游戏未准备好，无法开始");
                return;
            }

            CurrentState = GameState.Playing;
            OnGameStart();

            // 触发游戏开始事件
            GetFrameworkManager<EventManager>().Trigger(HuntingEvents.GameStarted);
        }

        public virtual void PauseGame()
        {
            if (CurrentState != GameState.Playing)
            {
                Debug.LogWarning("[GameLogic] 游戏未进行中，无法暂停");
                return;
            }

            CurrentState = GameState.Paused;
            OnGamePause();

            // 触发游戏暂停事件
            GetFrameworkManager<EventManager>().Trigger(HuntingEvents.GamePaused);
        }

        public virtual void ResumeGame()
        {
            if (CurrentState != GameState.Paused)
            {
                Debug.LogWarning("[GameLogic] 游戏未暂停，无法恢复");
                return;
            }

            CurrentState = GameState.Playing;
            OnGameResume();

            // 触发游戏恢复事件
            GetFrameworkManager<EventManager>().Trigger(HuntingEvents.GameResumed);
        }

        public virtual void EndGame()
        {
            if (CurrentState != GameState.Playing && CurrentState != GameState.Paused)
            {
                Debug.LogWarning("[GameLogic] 游戏未进行中或暂停，无法结束");
                return;
            }

            CurrentState = GameState.GameOver;
            OnGameEnd();

            // 触发游戏结束事件
            GetFrameworkManager<EventManager>().Trigger(HuntingEvents.GameEnded);
        }
        #endregion

        #region 钩子虚方法供子类重写
        protected virtual UniTask OnGameInit() => UniTask.CompletedTask;
        protected virtual void OnGameStart() { }
        protected virtual void OnGamePause() { }
        protected virtual void OnGameResume() { }
        protected virtual void OnGameEnd() { }
        protected virtual void OnGamePlaying() { }
        protected virtual void OnGamePaused() { }
        protected virtual void OnGameOver() { }
        #endregion

        /// <summary>
        /// 等待初始化完成
        /// </summary>
        public async UniTask WaitForInitialization()
        {
            while (CurrentState == GameState.None || CurrentState == GameState.Initializing)
            {
                await UniTask.Yield();
            }
        }
    }
}