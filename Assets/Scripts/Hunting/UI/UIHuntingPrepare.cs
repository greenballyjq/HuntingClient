using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core;
using Hunting.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 打猎准备界面
    /// </summary>
    public class UIHuntingPrepare : UIBase
    {
        /// <summary>
        /// 角色按钮组：小蓝人
        /// </summary>
        [SerializeField] private Button _buttonRoleMale;

        /// <summary>
        /// 角色按钮组：小红人
        /// </summary>
        [SerializeField] private Button _buttonRoleFemale;

        /// <summary>
        /// 角色按钮组：随机
        /// </summary>
        [SerializeField] private Button _buttonRoleRandom;

        /// <summary>
        /// 角色名称文本
        /// </summary>
        [SerializeField] private Text _textRoleName;

        /// <summary>
        /// 角色档案文本
        /// </summary>
        [SerializeField] private Text _textRoleProfile;

        /// <summary>
        /// 角色展示图片
        /// </summary>
        [SerializeField] private Image _imageRole;

        /// <summary>
        /// 地图名称文本
        /// </summary>
        [SerializeField] private Text _textMapName;

        /// <summary>
        /// 地图描述文本
        /// </summary>
        [SerializeField] private Text _textMapDescription;

        /// <summary>
        /// 地图展示图片
        /// </summary>
        [SerializeField] private Image _imageMap;

        /// <summary>
        /// 幸运仪式按钮
        /// </summary>
        [SerializeField] private Button _buttonLuckyRitual;

        /// <summary>
        /// 开始单局按钮
        /// </summary>
        [SerializeField] private Button _buttonStartRound;

        /// <summary>
        /// 当前选中的角色数据
        /// </summary>
        private Role _selectedRole;

        /// <summary>
        /// 当前选中的地图数据
        /// </summary>
        private Map _selectedMap;

        /// <summary>
        /// 当前选中的幸运仪式
        /// </summary>
        private ELuckyType _selectedLuckyType = ELuckyType.None;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager Resource => GameServiceLocator.Resource;

        /// <summary>
        /// 单局管理器
        /// </summary>
        private RoundManager Round => GameServiceLocator.GetGameManager<RoundManager>();

        private void Awake()
        {
            _buttonRoleMale.onClick.AddListener(OnClickSelectMaleRole);
            _buttonRoleFemale.onClick.AddListener(OnClickSelectFemaleRole);
            _buttonRoleRandom.onClick.AddListener(OnClickSelectRandomRole);
            _buttonLuckyRitual.onClick.AddListener(OnClickLuckyRitual);
            _buttonStartRound.onClick.AddListener(OnClickStartRound);
        }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            InitializeSelections();
        }

        protected override void OnShow()
        {
            base.OnShow();
            UpdateRoleDisplay(_selectedRole.ID);
            UpdateMapDisplay(_selectedMap.ID);
        }

        public override void OnClose()
        {
            base.OnClose();
            Debug.Log("[UIHuntingPrepare] 界面关闭");
        }

        private void OnDestroy()
        {
            _buttonRoleMale.onClick.RemoveListener(OnClickSelectMaleRole);
            _buttonRoleFemale.onClick.RemoveListener(OnClickSelectFemaleRole);
            _buttonRoleRandom.onClick.RemoveListener(OnClickSelectRandomRole);
            _buttonLuckyRitual.onClick.RemoveListener(OnClickLuckyRitual);
            _buttonStartRound.onClick.RemoveListener(OnClickStartRound);
        }


        #region 私有方法
        /// <summary>
        /// 初始化默认选项
        /// </summary>
        private void InitializeSelections()
        {
            // 默认角色选小蓝人
            _selectedRole = Config.GetDefaultRole();
            
            // 进入界面随机地图
            _selectedMap = Config.GetRandomMap();

            UpdateRoleDisplay(_selectedRole.ID);
            UpdateMapDisplay(_selectedMap.ID);
        }

        /// <summary>
        /// 更新角色展示
        /// </summary>
        /// <param name="roleId">角色ID</param>
        private void UpdateRoleDisplay(int roleId)
        {
            _selectedRole = Config.GetRole(roleId);
            _textRoleName.text = _selectedRole.RoleProfile.Name;
            var profile = _selectedRole.RoleProfile;
            // 拼接角色档案信息
            _textRoleProfile.text =
                $"出生地：{profile.Birthplace}\n性别：{profile.Gender}\n身高：{profile.Height}\n特征：{profile.Traits}\n个性：{profile.Personality}\n背景：{profile.BackgroundStory}";

            // 更新角色图片资源
            UpdateRoleSpriteAsync(_selectedRole.RoleImageResourcePath).Forget();
        }

        /// <summary>
        /// 更新地图展示
        /// </summary>
        /// <param name="mapId">地图ID</param>
        private void UpdateMapDisplay(int mapId)
        {
            _selectedMap = Config.GetMap(mapId);
            _textMapName.text = _selectedMap.Name;
            _textMapDescription.text = _selectedMap.Description;

            // 更新地图图片资源
            UpdateMapSpriteAsync(_selectedMap.MapImageResourcePath).Forget();
        }

        /// <summary>
        /// 应用单局上下文
        /// </summary>
        private void ApplyRoundContext()
        {
            var context = new RoundContext
            {
                RoleId = _selectedRole.ID,
                MapId = _selectedMap.ID,
                SkillId = _selectedRole.LinkedSkillId,
                LuckyType = _selectedLuckyType,
                HasMapAffinity = _selectedRole.LinkedMapId == _selectedMap.ID
            };

            Round.SetRoundContext(context);
        }

        /// <summary>
        /// 更新角色图片
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        private async UniTask UpdateRoleSpriteAsync(string assetPath)
        {
            var sprite = await Resource.LoadAssetAsync<Sprite>(assetPath);
            _imageRole.sprite = sprite;
        }

        /// <summary>
        /// 更新地图图片
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        private async UniTask UpdateMapSpriteAsync(string assetPath)
        {
            var sprite = await Resource.LoadAssetAsync<Sprite>(assetPath);
            _imageMap.sprite = sprite;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 选择小蓝人角色
        /// </summary>
        private void OnClickSelectMaleRole()
        {
            var role = Config.GetRole(ERoleType.Bule);
            UpdateRoleDisplay(role.ID);
            Debug.Log($"[UIHuntingPrepare] 选择角色 {role.RoleProfile.Name}");
        }

        /// <summary>
        /// 选择小红人角色
        /// </summary>
        private void OnClickSelectFemaleRole()
        {
            var role = Config.GetRole(ERoleType.Red);
            UpdateRoleDisplay(role.ID);
            Debug.Log($"[UIHuntingPrepare] 选择角色 {role.RoleProfile.Name}");
        }

        /// <summary>
        /// 随机选择角色
        /// </summary>
        private void OnClickSelectRandomRole()
        {
            var role = Config.GetRandomRole();
            UpdateRoleDisplay(role.ID);
            Debug.Log($"[UIHuntingPrepare] 随机角色 {role.RoleProfile.Name}");
        }

        /// <summary>
        /// 幸运仪式按钮
        /// </summary>
        private void OnClickLuckyRitual()
        {
            // 功能待定，先进行占位提示
            Debug.Log("[UIHuntingPrepare] 幸运仪式暂未开放");

            // 默认为无幸运仪式
            _selectedLuckyType = ELuckyType.None;
        }

        /// <summary>
        /// 开始单局
        /// </summary>
        private void OnClickStartRound()
        {
            // 将当前选项写入单局上下文
            ApplyRoundContext();

            // 朝局内广播开始
            Round.StartRound();

            // 关闭面板
            Close();

            Debug.Log("[UIHuntingPrepare] 开始单局");
        }
        #endregion
    }
}