using cfg.HuntingConfig;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.UI
{
    /// <summary>
    /// 准备界面
    /// </summary>
    public class UIPrepare : UIBase
    {
        /// <summary>
        /// 角色选择组件
        /// </summary>
        [SerializeField] private UIComponentRoleChoice _uiComponentRoleChoice;

        /// <summary>
        /// 角色信息组件
        /// </summary>
        [SerializeField] private UIComponentRoleInfo _uiComponentRoleInfo;

        /// <summary>
        /// 地图信息组件
        /// </summary>
        [SerializeField] private UIComponentMapInfo _uiComponentMapInfo;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 当前选中的角色ID
        /// </summary>
        private int _currentRoleId;

        /// <summary>
        /// 当前选中的地图ID
        /// </summary>
        private int _currentMapId;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            _uiComponentRoleChoice.Init();
            _uiComponentRoleInfo.Init();
            _uiComponentMapInfo.Init();
            Event.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
            Event.AddListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
        }

        public override void OnClose()
        {
            _uiComponentRoleChoice.CleanUp();
            _uiComponentRoleInfo.CleanUp();
            _uiComponentMapInfo.CleanUp();
            Event.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
            Event.RemoveListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
            base.OnClose();
        }

        #region 私有方法
        /// <summary>
        /// 检查地图联动
        /// </summary>
        private void CheckMapAffinity()
        {
            _currentMapId = _uiComponentMapInfo.GetCurrentMapId();
            Role role = Config.GetRole(_currentRoleId);
            bool hasAffinity = role.LinkedMapId == _currentMapId;

            Event.Trigger(PrepareEvents.MapAffinityChecked, new MapAffinityCheckedEventArgs
            {
                HasAffinity = hasAffinity,
                RoleId = _currentRoleId,
                MapId = _currentMapId
            });
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 角色选中事件回调
        /// </summary>
        private void OnRoleSelected(RoleSelectedEventArgs args)
        {
            _currentRoleId = args.RoleId;
        }

        /// <summary>
        /// 完整选角动画结束事件回调
        /// </summary>
        private void OnRoleSelectionAnimationEnded()
        {
            CheckMapAffinity();
        }
        #endregion
    }
}
