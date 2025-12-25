using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using Hunting.Game.Weapons;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Props
{
    /// <summary>
    /// 指哪打哪道具处理器
    /// </summary>
    public class PropAimAssistHandler : IPropHandler
    {
        /// <summary>
        /// 瞄准镜游戏对象
        /// </summary>
        private GameObject _uiCrosshairGameObject;

        /// <summary>
        /// 瞄准镜UI
        /// </summary>
        private UICrosshair _uiCrosshair;

        /// <summary>
        /// 最小锁定距离
        /// </summary>
        private float _minLockDistance;

        /// <summary>
        /// 最大锁定距离
        /// </summary>
        private float _maxLockDistance;

        /// <summary>
        /// 当前锁定的目标
        /// </summary>
        private Transform _currentTarget;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 玩家控制管理器
        /// </summary>
        private PlayerControlManager PlayerControl => GameServiceLocator.GetHuntingAppManager<PlayerControlManager>();

        /// <summary>
        /// 道具效果开始
        /// </summary>
        public void OnPropStart(PropContext context)
        {
            // 读取配置参数
            PropAimAssist parameter = Config.GetPropAimAssist(context.PropData.ParamTableID);
            _minLockDistance = parameter.MinLockDistance;
            _maxLockDistance = parameter.MaxLockDistance;
            string prefabPath = parameter.UIPropAimAssistPrefabResourcePath;

            // 创建瞄准镜
            CreateCrosshair(prefabPath);

            // 切换到指哪打哪模式
            PlayerControl.SwitchToAimAssist(_minLockDistance, _maxLockDistance);

            RegisterEvents();
        }

        /// <summary>
        /// 道具效果更新
        /// </summary>
        public void OnPropUpdate(PropContext context, float deltaTime)
        {
            if (_uiCrosshair == null)
                return;

            _uiCrosshair.UpdateByTarget(_currentTarget);
        }

        /// <summary>
        /// 道具效果结束
        /// </summary>
        public void OnPropEnd(PropContext context)
        {
            UnregisterEvents();

            // 切换回默认射击模式
            PlayerControl.SwitchToDefaultShooting();

            // 销毁瞄准镜
            DestroyCrosshair();
        }

        #region 私有方法
        /// <summary>
        /// 创建瞄准镜
        /// </summary>
        /// <param name="prefabPath">预制体资源路径</param>
        private void CreateCrosshair(string prefabPath)
        {
            // TODO: 待以后统一资源管理器，当前使用Resource方式
            GameObject prefab = Resources.Load<GameObject>(prefabPath); ;

            // 实例化并初始化瞄准镜
            _uiCrosshairGameObject = Object.Instantiate(prefab);
            _uiCrosshair = _uiCrosshairGameObject.GetComponent<UICrosshair>();
            _uiCrosshair.SetDistanceRange(_minLockDistance, _maxLockDistance);
            _uiCrosshair.UpdateByTarget(null);
        }

        /// <summary>
        /// 销毁瞄准镜
        /// </summary>
        private void DestroyCrosshair()
        {
            if (_uiCrosshairGameObject != null)
            {
                Object.Destroy(_uiCrosshairGameObject);
                _uiCrosshairGameObject = null;
            }

            _uiCrosshair = null;
            _currentTarget = null;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(PlayerControlEvents.TargetSelected, OnTargetSelected);
            Event.AddListener(PlayerControlEvents.TargetLost, OnTargetLost);
            Event.AddListener(PlayerControlEvents.TargetChanged, OnTargetChanged);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(PlayerControlEvents.TargetSelected, OnTargetSelected);
            Event.RemoveListener(PlayerControlEvents.TargetLost, OnTargetLost);
            Event.RemoveListener(PlayerControlEvents.TargetChanged, OnTargetChanged);
        }

        /// <summary>
        /// 目标已选中事件回调
        /// </summary>
        private void OnTargetSelected(TargetSelectedEventArgs args)
        {
            _currentTarget = args.Target;
        }

        /// <summary>
        /// 目标已丢失事件回调
        /// </summary>
        private void OnTargetLost(TargetLostEventArgs args)
        {
            if (_currentTarget == args.LostTarget)
                _currentTarget = null;
        }

        /// <summary>
        /// 目标已切换事件回调
        /// </summary>
        private void OnTargetChanged(TargetChangedEventArgs args)
        {
            _currentTarget = args.NewTarget;
        }
        #endregion
    }
}

