using cfg.HuntingConfig;
using GameFramework.Core.UI;
using TMPro;
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

    /// <summary>
    /// 价值肉量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textMeatValue;

    private HuntingConfigManager _configManager;

    private void Awake()
    {
        _configManager = GameServiceLocator.ConfigManager;
    }

    public void Init(Map data)
    {
        base.Init();
        LoadMapIcon(data.ID);
        _textMeatValue.text = "0";
    }

    #region 公共方法
    /// <summary>
    /// 设置价值肉量
    /// </summary>
    public void SetMeatValue(int meatValue)
    {
        _textMeatValue.text = meatValue.ToString();
    }

    /// <summary>
    /// 加载地图图标
    /// </summary>
    private void LoadMapIcon(int mapId)
    {
        _imageMapIcon.sprite = _configManager.MapRefSo.GetMapIcon(mapId);
    }
    #endregion
}
