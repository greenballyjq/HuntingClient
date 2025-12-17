using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

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
        /// 开始单局按钮
        /// </summary>
        [SerializeField] private Button _buttonStartRound;

        /// <summary>
        /// 幸运仪式按钮
        /// </summary>
        [SerializeField] private Button _buttonLuckyRitual;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager UI => GameServiceLocator.UI;

        /// <summary>
        /// 单局管理器
        /// </summary>
        private RoundManager Round => GameServiceLocator.GetGameManager<RoundManager>();

        /// <summary>
        /// 当前选中的角色ID
        /// </summary>
        private int _currentRoleId;

        /// <summary>
        /// 当前选中的地图ID
        /// </summary>
        private int _currentMapId;

        /// <summary>
        /// 当前选中幸运仪式增益数据
        /// </summary>
        private LuckyBuff _currentLuckyBuffData;

        /// <summary>
        /// 是否已经有合法的角色选择
        /// </summary>
        private bool _hasSelectedRole;

    
        private void Awake()
        {
            _buttonStartRound.onClick.AddListener(OnClickStartRound);
            _buttonLuckyRitual.onClick.AddListener(OnClickLuckyRitual);
        }

        private void OnDestroy()
        {
            _buttonStartRound.onClick.RemoveListener(OnClickStartRound);
            _buttonLuckyRitual.onClick.RemoveListener(OnClickLuckyRitual);
        }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            _uiComponentRoleChoice.Init();
            _uiComponentRoleInfo.Init();
            _uiComponentMapInfo.Init();

            _buttonStartRound.interactable = false;

            Event.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
            Event.AddListener(PrepareEvents.RoleSelectionAnimationStarted, OnRoleSelectionAnimationStarted);
            Event.AddListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
            Event.AddListener(LuckyBuffEvents.GiftOpened, OnGiftOpened);
        }

        public override void OnClose()
        {
            _uiComponentRoleChoice.CleanUp();
            _uiComponentRoleInfo.CleanUp();
            _uiComponentMapInfo.CleanUp();

            Event.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
            Event.RemoveListener(PrepareEvents.RoleSelectionAnimationStarted, OnRoleSelectionAnimationStarted);
            Event.RemoveListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
            Event.RemoveListener(LuckyBuffEvents.GiftOpened, OnGiftOpened);

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

        /// <summary>
        /// 设置下一局的单局上下文
        /// </summary>
        private void SetNextRoundContext()
        {
            Role role = Config.GetRole(_currentRoleId);

            var context = new RoundContext
            {
                RoleId = _currentRoleId,
                MapId = _currentMapId,
                SkillId = role.LinkedSkillId,
                LuckyBuffData = _currentLuckyBuffData,
                HasMapAffinity = role.LinkedMapId == _currentMapId
            };

            Round.SetRoundContext(context);
        }
        #endregion

        #region 事件相关

        /// <summary>
        /// 角色选中事件回调
        /// </summary>
        private void OnRoleSelected(RoleSelectedEventArgs args)
        {
            _currentRoleId = args.RoleId;
            _hasSelectedRole = true;
        }

        /// <summary>
        /// 完整选角动画开始事件回调
        /// </summary>
        private void OnRoleSelectionAnimationStarted()
        {
            _buttonStartRound.interactable = false;
            _buttonLuckyRitual.interactable = false;
        }

        /// <summary>
        /// 完整选角动画结束事件回调
        /// </summary>
        private void OnRoleSelectionAnimationEnded()
        {
            CheckMapAffinity();

            _buttonLuckyRitual.interactable = true;

            if (_hasSelectedRole)
                _buttonStartRound.interactable = true;
        }

        /// <summary>
        /// 开始单局按钮回调
        /// </summary>
        private void OnClickStartRound()
        {
            SetNextRoundContext();
            Round.StartRound();
            Close();
        }

        /// <summary>
        /// 幸运仪式按钮回调
        /// </summary>
        private async void OnClickLuckyRitual()
        {
            await UI.OpenUIAsync<UILucky>("UILucky");
        }

        /// <summary>
        /// 礼包开启事件回调
        /// </summary>
        private void OnGiftOpened(GiftOpenedEventArgs args)
        {
            _currentLuckyBuffData = args.LuckyBuffData;
        }

        #endregion
    }
}
