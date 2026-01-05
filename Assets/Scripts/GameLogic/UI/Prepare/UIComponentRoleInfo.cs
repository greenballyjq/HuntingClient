using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using cfg.HuntingConfig.Skill;
using GameFramework.Manager;

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
    /// 角色简介文本
    /// </summary>
    [SerializeField] private Text _textRoleProfile;

    /// <summary>
    /// 技能名称文本
    /// </summary>
    [SerializeField] private Text _textSkillName;

    /// <summary>
    /// 技能描述文本
    /// </summary>
    [SerializeField] private Text _textSkillDescription;

    /// <summary>
    /// 当前角色ID
    /// </summary>
    private int _currentRoleId;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    public void Init()
    {
        _eventManager.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.AddListener(PrepareEvents.SlotAnimationEnded , OnSlotAnimationEnded);

        gameObject.SetActive(false);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.RemoveListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
    }

    #region 私有方法
    /// <summary>
    /// 更新角色信息显示
    /// </summary>
    /// <param name="roleId">角色ID</param>
    private void UpdateRoleInfo(int roleId)
    {
        Role role = _configManager.GetRole(roleId);

        var profile = role.RoleProfile;

        _textRoleProfile.text =
            $"姓名：{profile.Name}\n" +
            $"出生地：{profile.Birthplace}\n" +
            $"性别：{profile.Gender}\n" +
            $"特征：{profile.Traits}\n" +
            $"个性：{profile.Personality}\n";

        LoadRoleSpriteAsync(role.RoleImageResourcePath).Forget();

        UpdateSkillInfo(role.LinkedSkillId);
    }

    /// <summary>
    /// 更新技能信息显示
    /// </summary>
    /// <param name="skillId">技能ID</param>
    private void UpdateSkillInfo(int skillId)
    {
        Skill skill = _configManager.GetSkill(skillId);

        _textSkillName.text = skill.Name;
        _textSkillDescription.text = skill.Description;

        LoadSkillSpriteAsync(skill.IconResourcePath).Forget();
    }

    /// <summary>
    /// 异步加载角色图片
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    private async UniTask LoadRoleSpriteAsync(string assetPath)
    {
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(assetPath);    
        _imageRole.sprite = sprite;
    }

    /// <summary>
    /// 异步加载技能图标
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    private async UniTask LoadSkillSpriteAsync(string assetPath)
    {
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(assetPath);
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
    private void OnSlotAnimationEnded()
    {
        gameObject.SetActive(true);
        UpdateRoleInfo(_currentRoleId);
    }
    #endregion
}