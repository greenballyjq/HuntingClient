using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using GameFramework.UI;
using GameFramework.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能信息组件
/// </summary>
public class UIComponentSkillInfo : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 技能图像
    /// </summary>
    [SerializeField] private Image _imageSkill;

    /// <summary>
    /// 技能描述文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textSkillDescription;

    /// <summary>
    /// 当前角色ID
    /// </summary>
    private int _currentRoleId;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    public void Init()
    {
        _configManager = GameServiceLocator.ConfigManager;
        _eventManager = GameServiceLocator.EventManager;
        _eventManager.AddListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.AddListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);

        gameObject.SetActive(false);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(PrepareEvents.RoleSelected, OnRoleSelected);
        _eventManager.RemoveListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
    }

    #region 私有方法
    /// <summary>
    /// 更新技能信息
    /// </summary>
    /// <param name="roleId">角色ID</param>
    private void UpdateSkillInfo(int roleId)
    {
        Role roleData = _configManager.GetRole(roleId);
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);

        _textSkillDescription.text = skillData.Description;

        _imageSkill.sprite = _configManager.SkillRefSo.GetSkillIcon(skillData.ID);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 角色选中时先填内容，揭晓入场时再显示。
    /// </summary>
    private void OnRoleSelected(RoleSelectedEventArgs args)
    {
        _currentRoleId = args.RoleId;
        UpdateSkillInfo(_currentRoleId);
    }

    /// <summary>
    /// 金币人停稳后显示，由入场动画带到休息位。
    /// </summary>
    private void OnSlotAnimationEnded()
    {
        gameObject.SetActive(true);
    }
    #endregion
}
