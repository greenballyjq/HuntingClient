using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;

namespace Hunting.Manager
{
    /// <summary>
    /// 游戏专用配置管理器
    /// </summary>
    public class GameConfigManager : ConfigManager<GameConfigManager>
    {
        
        public bool LoadComplete { get; private set; }

        /// <summary>
        /// 配置表名称列表
        /// </summary>
        protected override List<string> TableNames => new List<string>
        {
            "gooseconfig_tbdifficulty",
            "gooseconfig_tbegg",
            "gooseconfig_tbglobal",
            "gooseconfig_tbgoose",
            "gooseconfig_tbwave",
            "demo_tbitem"
        };

        public async override void Init()
        {
            base.Init();
            await WaitForInitializationAsync();
            
        }

        public async Task WaitForLoadComplete()
        {
            while (!LoadComplete)
            {
                await UniTask.Delay(10);
            }
        }
    }
}