using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using GameFramework.Manager;

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
    [SerializeField] private Text _textMapName;

    /// <summary>
    /// 地图描述文本
    /// </summary>
    [SerializeField] private Text _textMapDescription;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    /// <summary>
    /// 当前地图ID
    /// </summary>
    private int _currentMapId;

    public void Init()
    {
        Map map = _configManager.GetRandomMap();
        _currentMapId = map.ID;
        UpdateMapInfo(map);
    }

    public void CleanUp()
    {

    }

    #region 公共方法
    /// <summary>
    /// 获取当前地图ID
    /// </summary>
    /// <returns>地图ID</returns>
    public int GetCurrentMapId()
    {
        return _currentMapId;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 更新地图信息显示
    /// </summary>
    /// <param name="map">地图配置</param>
    private void UpdateMapInfo(Map map)
    {
        _textMapName.text = map.Name;
        _textMapDescription.text = map.Description;

        LoadMapSpriteAsync(map.MapImageResourcePath).Forget();
    }

    /// <summary>
    /// 异步加载地图图片
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    private async UniTask LoadMapSpriteAsync(string assetPath)
    {
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(assetPath);
        _imageMap.sprite = sprite;
    }
    #endregion
}