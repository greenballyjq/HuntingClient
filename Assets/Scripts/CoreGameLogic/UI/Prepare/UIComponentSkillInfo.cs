using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using GameFramework.Core.UI;
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
    private async UniTask UpdateSkillInfo(int roleId)
    {
        Role roleData = _configManager.GetRole(roleId);
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);

        _textSkillDescription.text = skillData.Description;

        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(skillData.IconResourcePath);
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
    /// 走格子动画结束事件回调
    /// </summary>
    private async void OnSlotAnimationEnded()
    {
        await UpdateSkillInfo(_currentRoleId);
        gameObject.SetActive(true);
    }
    #endregion
}
