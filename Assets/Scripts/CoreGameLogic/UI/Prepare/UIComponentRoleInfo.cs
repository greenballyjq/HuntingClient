using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core.UI;
using GameFramework.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    /// 角色阴影图像
    /// </summary>
    [SerializeField] private Image _imageRoleShadow;

    /// <summary>
    /// 角色描述文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textRoleDescription;

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
    /// 更新角色信息
    /// </summary>
    /// <param name="roleId">角色ID</param>
    private async UniTask UpdateRoleInfo(int roleId)
    {
        Role roleData = _configManager.GetRole(roleId);

        var description = roleData.RoleProfile;

        _textRoleDescription.text = $"名称：{description.Name}\n{description.BackgroundStory}";

        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(roleData.IconResourcePath);
        _imageRole.sprite = sprite;
        _imageRoleShadow.sprite = sprite;
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
        await UpdateRoleInfo(_currentRoleId);
        gameObject.SetActive(true);
    }
    #endregion
}