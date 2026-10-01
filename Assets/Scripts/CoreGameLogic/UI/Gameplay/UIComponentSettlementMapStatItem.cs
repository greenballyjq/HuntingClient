using cfg.HuntingConfig;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 地图结算统计项组件
/// </summary>
public class UIComponentSettlementMapStatItem : UIComponentSettlementStatItem, IUIComponent<Map>
{
    /// <summary>
    /// 地图图标
    /// </summary>
    [SerializeField] private Image _imageMapIcon;

    private HuntingConfigManager _configManager;

    private void Awake()
    {
        _configManager = GameServiceLocator.ConfigManager;
    }

    public void Init(Map data)
    {
        base.Init();
        LoadMapIcon(data.ID);
    }

    #region 私有方法
    /// <summary>
    /// 加载地图图标
    /// </summary>
    private void LoadMapIcon(int mapId)
    {
        _imageMapIcon.sprite = _configManager.MapRefSo.GetMapIcon(mapId);
    }
    #endregion
}
