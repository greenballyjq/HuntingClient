using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using Hunting.App;
using Hunting.Round;
using Hunting.UI;
using UnityEngine;


namespace Hunting.Manager
{
    /// 单局管理器
    /// </summary>
    public sealed class RoundManager : IAppManager
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        #region 公共方法
        /// <summary>
        /// 开始单局
        /// </summary>
        public async void StartRound(RoundContext context)
        {
            Event.Trigger(RoundFlowEvents.RoundStarted, new RoundStartedEventArgs
            {
                Sender = this,
                Context = context
            });
        }
        #endregion

        public void Init()
        {
        }

        public void Dispose()
        {
        }
    }
}


