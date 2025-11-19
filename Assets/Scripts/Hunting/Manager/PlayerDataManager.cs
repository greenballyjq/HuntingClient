using cfg.HuntingConfig.Enum;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 玩家数据管理器（占位，模拟道具数据逻辑）
    /// </summary>
    public class PlayerDataManager : BaseGameManager
    {
        public override void Init()
        {
            Debug.Log("[PlayerDataManager] 初始化占位管理器");
        }

        public override void Update()
        {
        }

        public override void Release()
        {
            Debug.Log("[PlayerDataManager] 已释放");
        }

        #region 道具接口
        /// <summary>
        /// 判断是否可使用指定道具（当前始终返回可用）
        /// </summary>
        public bool CanUseProp(EPropType propType)
        {
            return true;
        }

        /// <summary>
        /// 尝试消耗道具（当前始终视为成功）
        /// </summary>
        public bool TryConsumeProp(EPropType propType)
        {
            return true;
        }

        /// <summary>
        /// 增加道具数量（占位，暂不处理）
        /// </summary>
        public void AddProp(EPropType propType, int count)
        {
        }
        #endregion
    }
}

