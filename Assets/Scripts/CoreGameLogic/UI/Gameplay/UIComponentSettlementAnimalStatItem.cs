using cfg.HuntingConfig;
using GameFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 动物结算统计项组件
/// </summary>
public class UIComponentSettlementAnimalStatItem : UIComponentSettlementStatItem,IUIComponent<int>
{
    /// <summary>
    /// 动物头像
    /// </summary>
    [SerializeField] private Image _imageAnimalHead;

    /// <summary>
    /// 价值肉量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textMeatValue;

    private HuntingConfigManager _configManager;

    private void Awake()
    {
        _configManager = GameServiceLocator.ConfigManager;
    }

    public void Init(int specieId)
    {
        base.Init();
        LoadHeadImage(specieId);
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
    /// 加载动物头像
    /// </summary>
    private void LoadHeadImage(int specieId)
    {
        _imageAnimalHead.sprite = _configManager._AnimalRefSo.GetAnimalIcon(specieId);
    }
    #endregion
}
