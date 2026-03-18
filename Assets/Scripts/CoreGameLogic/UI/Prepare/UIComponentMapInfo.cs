using cfg.HuntingConfig;
using UnityEngine;
using UnityEngine.UI;
using GameFramework.Core.UI;
using TMPro;

/// <summary>
/// 地图信息组件
/// </summary>
public class UIComponentMapInfo : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 地图图像
    /// </summary>
    [SerializeField] private Image _imageMap;

    /// <summary>
    /// 地图名称文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textMapName;

    /// <summary>
    /// 地图描述文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textMapDescription;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 当前地图数据
    /// </summary>
    private Map _currentMapData;
    public Map CurrentMapData => _currentMapData;

    public void Init()
    {
        Map map = _configManager.GetRandomMainMap();
        _currentMapData = map;
        UpdateMapInfo(map);
    }

    public void CleanUp(){}

    /// <summary>
    /// 更新地图信息显示
    /// </summary>
    private void UpdateMapInfo(Map map)
    {
        _textMapDescription.text = map.Description;
        _textMapName.text = map.Name;
        _imageMap.sprite = _configManager.MapRefSo.GetMapIcon(map.ID);
    }
}