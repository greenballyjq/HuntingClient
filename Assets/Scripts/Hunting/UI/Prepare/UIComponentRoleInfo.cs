using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;
using cfg.HuntingConfig.Skill;

namespace Hunting.UI
{
    /// <summary>
    /// 角色信息组件
    /// </summary>
    public class UIComponentRoleInfo : MonoBehaviour, IUIComponent
    {
        /// <summary>
        /// 角色图像
        /// </summary>
        [SerializeField] private Image _imageRole;

        /// <summary>
        /// 技能图像
        /// </summary>
        [SerializeField] private Image _imageSkill;

        /// <summary>
        /// 角色名称文本
        /// </summary>
        [SerializeField] private Text _textRoleName;

        /// <summary>
        /// 角色简介文本
        /// </summary>
        [SerializeField] private Text _textRoleProfile;

        /// <summary>
        /// 技能文本
        /// </summary>
        [SerializeField] private Text _textSkill;

        /// <summary>
        /// 当前角色ID
        /// </summary>
        private int _currentRoleId;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager Resource => GameServiceLocator.Resource;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        public void Init()
        {
            Event.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
            Event.AddListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);

            gameObject.SetActive(false);
        }

        public void CleanUp()
        {
            Event.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
            Event.RemoveListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
        }

        #region 私有方法
        /// <summary>
        /// 更新角色信息显示
        /// </summary>
        /// <param name="roleId">角色ID</param>
        private void UpdateRoleInfo(int roleId)
        {
            Role role = Config.GetRole(roleId);

            var profile = role.RoleProfile;
            _textRoleName.text = profile.Name;

            _textRoleProfile.text =
                $"出生地：{profile.Birthplace}\n" +
                $"性别：{profile.Gender}\n" +
                $"身高：{profile.Height}\n" +
                $"特征：{profile.Traits}\n" +
                $"个性：{profile.Personality}\n" +
                $"背景：{profile.BackgroundStory}";

            LoadRoleSpriteAsync(role.RoleImageResourcePath).Forget();

            UpdateSkillInfo(role.LinkedSkillId);
        }

        /// <summary>
        /// 更新技能信息显示
        /// </summary>
        /// <param name="skillId">技能ID</param>
        private void UpdateSkillInfo(int skillId)
        {
            Skill skill = Config.GetSkill(skillId);

            _textSkill.text = $"{skill.Name}\n{skill.Description}";

            LoadSkillSpriteAsync(skill.IconResourcePath).Forget();
        }

        /// <summary>
        /// 异步加载角色图片
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        private async UniTask LoadRoleSpriteAsync(string assetPath)
        {
            var sprite = await Resource.LoadAssetAsync<Sprite>(assetPath);    
            _imageRole.sprite = sprite;
        }

        /// <summary>
        /// 异步加载技能图标
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        private async UniTask LoadSkillSpriteAsync(string assetPath)
        {
            var sprite = await Resource.LoadAssetAsync<Sprite>(assetPath);
            _imageSkill.sprite = sprite;
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
            gameObject.SetActive(true);
            UpdateRoleInfo(_currentRoleId);
        }
        #endregion
    }
}

