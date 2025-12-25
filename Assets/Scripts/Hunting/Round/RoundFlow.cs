using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using Hunting.Manager;
using Hunting.UI;
using System.Collections.Generic;

namespace Hunting.Round
{
    /// <summary>
    /// 单局流程类
    /// </summary>
    public sealed class RoundFlow
    {
        /// <summary>
        /// 单局流程状态枚举
        /// </summary>
        private enum RoundFlowState
        {
            /// <summary>
            /// 无状态
            /// </summary>
            None,

            /// <summary>
            /// 正在游玩
            /// </summary>
            Playing,

            /// <summary>
            /// 暂停
            /// </summary>
            Paused,

            /// <summary>
            /// 正在切图
            /// </summary>
            ChangingMap
        }

        /// <summary>
        /// 单局流程当前状态
        /// </summary>
        private RoundFlowState _currentState = RoundFlowState.None;

        /// <summary>
        /// 当前单局上下文
        /// </summary>
        private RoundContext _currentRoundContext;

        /// <summary>
        /// 单局管理器列表
        /// </summary>
        private readonly List<IRoundManager> _roundManagers = new List<IRoundManager>();

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager = GameServiceLocator.Event;

        /// <summary>
        /// UI管理器
        /// </summary>

        private UIManager _uiManager = GameServiceLocator.UI;

        #region 公共方法
        /// <summary>
        /// 获取单局管理器
        /// </summary>
        public T GetRoundManager<T>() where T : class, IRoundManager
        {
            foreach (var manager in _roundManagers)
            {
                if (manager is T result)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// 开始单局
        /// </summary>
        /// <param name="context">单局上下文</param>
        public void StartRound(RoundContext context)
        {
            if (_currentState != RoundFlowState.None)
                return;

            _currentRoundContext = context;

            OnRoundStart();

            TriggerRoundStarted(new RoundStartedEventArgs
            {
                Sender = this,
                Context = _currentRoundContext
            });

            _currentState = RoundFlowState.Playing;
        }

        /// <summary>
        /// 结束单局
        /// </summary>
        public void EndRound()
        {
            if (_currentState != RoundFlowState.Playing)
                return;

            OnRoundEnd();

            TriggerRoundEnded(new RoundEndedEventArgs
            {
                Sender = this,
                Context = _currentRoundContext
            });

            _currentState = RoundFlowState.None;
        }

        /// <summary>
        /// 暂停单局
        /// </summary>
        public void PauseRound()
        {
            if (_currentState != RoundFlowState.Playing)
                return;

            OnRoundPause();

            TriggerRoundPaused();

            _currentState = RoundFlowState.Paused;
        }

        /// <summary>
        /// 恢复单局
        /// </summary>
        public void ResumeRound()
        {
            if (_currentState != RoundFlowState.Paused)
                return;

            OnRoundResume();

            TriggerRoundResumed();

            _currentState = RoundFlowState.Playing;
        }

        /// <summary>
        /// 切换地图
        /// </summary>
        /// <param name="mapType">目标地图类型</param>
        public async UniTask ChangeMapAsync(EMapType mapType)
        {
            if (_currentState != RoundFlowState.Playing)
                return;

            _currentState = RoundFlowState.ChangingMap;

            OnRoundChangeMapStart(mapType);
            TriggerRoundMapChangeStarted(new RoundMapChangeEventArgs
            {
                Sender = this,
                Context = _currentRoundContext,
                MapType = mapType
            });

            // TODO: 根据 mapType 进行切图（可能涉及切场景/加载资源/等待完成）
            // TODO: 切图完成后，必要时重新收集场景对象（例如派发点等）
            await UniTask.CompletedTask;

            OnRoundChangeMapFinish(mapType);
            TriggerRoundMapChangeFinished(new RoundMapChangeEventArgs
            {
                Sender = this,
                Context = _currentRoundContext,
                MapType = mapType
            });

            _currentState = RoundFlowState.Playing;
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            if (_currentState != RoundFlowState.Playing)
                return;

            OnRoundPlaying(dt);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 创建单局管理器
        /// </summary>
        private void CreateRoundManagers()
        {
            // TODO: 现阶段先接入可测的局级管理器，后续逐个迁移
            _roundManagers.Add(new AnimalManager());
            _roundManagers.Add(new EnergyProgressManager());
            _roundManagers.Add(new MeatProgressManager());
            _roundManagers.Add(new QuestManager());
            _roundManagers.Add(new SettlementRewardManager());
            _roundManagers.Add(new SpawnerManager());
            _roundManagers.Add(new TrapManager());
            _roundManagers.Add(new LuckyBuffManager());
            _roundManagers.Add(new PropManager());
            _roundManagers.Add(new PlayerControlManager());
            _roundManagers.Add(new WeaponManager());
            _roundManagers.Add(new BulletManager());
            _roundManagers.Add(new SkillManager());

            // TODO: 其它局级管理器在此添加
        }

        /// <summary>
        /// 初始化本局管理器
        /// </summary>
        private void InitRoundManagers()
        {
            for (int i = 0; i < _roundManagers.Count; i++)
                _roundManagers[i].Init(_currentRoundContext);
        }

        /// <summary>
        /// 释放本局管理器
        /// </summary>
        private void DisposeRoundManagers()
        {
            for (int i = _roundManagers.Count - 1; i >= 0; i--)
                _roundManagers[i].Dispose();
        }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 单局开始
        /// </summary>
        private async void OnRoundStart()
        {
            CreateRoundManagers();

            InitRoundManagers();

            await _uiManager.OpenUIAsync<UIGameplay>("UIHuntingGameplay");
        }

        /// <summary>
        /// 游玩中更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        private void OnRoundPlaying(float dt)
        {
            for (int i = 0; i < _roundManagers.Count; i++)
            {
                // 仅调度实现了可更新接口的单局管理器
                if (_roundManagers[i] is IRoundUpdatable updatable)
                    updatable.DoUpdate(dt);
            }
        }

        /// <summary>
        /// 进入暂停
        /// </summary>
        private void OnRoundPause()
        {
            for (int i = 0; i < _roundManagers.Count; i++)
            {
                // 仅调度实现了可暂停接口的单局管理器
                if (_roundManagers[i] is IRoundPausable pausable)
                    pausable.Pause();
            }

            // TODO: 是否需要 Time.timeScale/音效/输入等全局处理，后续按策略补充
        }

        /// <summary>
        /// 退出暂停
        /// </summary>
        private void OnRoundResume()
        {
            for (int i = 0; i < _roundManagers.Count; i++)
            {
                if (_roundManagers[i] is IRoundPausable pausable)
                    pausable.Resume();
            }

            // TODO: 是否需要恢复 Time.timeScale/音效/输入等全局处理，后续按策略补充
        }

        /// <summary>
        /// 切图开始
        /// </summary>
        /// <param name="mapType">目标地图类型</param>
        private void OnRoundChangeMapStart(EMapType mapType)
        {
            // TODO: 切图开始时需要做的准备（例如停输入/停生成/停AI等）
        }

        /// <summary>
        /// 切图完成
        /// </summary>
        /// <param name="mapType">目标地图类型</param>
        private void OnRoundChangeMapFinish(EMapType mapType)
        {
            // TODO: 切图完成后需要做的恢复/重绑（例如重新收集场景对象等）
        }

        /// <summary>
        /// 单局结束
        /// </summary>
        private void OnRoundEnd()
        {
            DisposeRoundManagers();

            _roundManagers.Clear();

            _currentRoundContext = null;

            // TODO: 需要保证单局退出一定会调用 EndRound（例如回准备/退出游戏/异常中断）
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 触发单局开始事件
        /// </summary>
        private void TriggerRoundStarted(RoundStartedEventArgs args)
        {
            _eventManager.Trigger(RoundFlowEvents.RoundStarted, args);
        }

        /// <summary>
        /// 触发单局暂停事件
        /// </summary>
        private void TriggerRoundPaused()
        {
            _eventManager.Trigger(RoundFlowEvents.RoundPaused);
        }

        /// <summary>
        /// 触发单局恢复事件
        /// </summary>
        private void TriggerRoundResumed()
        {
            _eventManager.Trigger(RoundFlowEvents.RoundResumed);
        }

        /// <summary>
        /// 触发单局结束事件
        /// </summary>
        private void TriggerRoundEnded(RoundEndedEventArgs args)
        {
            _eventManager.Trigger(RoundFlowEvents.RoundEnded, args);
        }

        /// <summary>
        /// 触发切图开始事件
        /// </summary>
        private void TriggerRoundMapChangeStarted(RoundMapChangeEventArgs args)
        {
            _eventManager.Trigger(RoundFlowEvents.RoundMapChangeStarted, args);
        }

        /// <summary>
        /// 触发切图完成事件
        /// </summary>
        private void TriggerRoundMapChangeFinished(RoundMapChangeEventArgs args)
        {
            _eventManager.Trigger(RoundFlowEvents.RoundMapChangeFinished, args);
        }
        #endregion
    }
}
