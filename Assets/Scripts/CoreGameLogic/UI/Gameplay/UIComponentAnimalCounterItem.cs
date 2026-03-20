using cfg.HuntingConfig;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 动物统计项组件
/// </summary>
public class UIComponentAnimalCounterItem : MonoBehaviour, IUIComponent<Specie>
{
    /// <summary>
    /// 动物头像
    /// </summary>
    [SerializeField] private Image _imageAnimalHead;

    /// <summary>
    /// 数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textCount;


    private HuntingConfigManager _configManager;

    private void Awake()
    {
        RegisterServers();
    }

    public void Init(Specie specieData)
    {
        _textCount.text = "0";
        LoadHeadImage(specieData.ID);
    }

    public void CleanUp() {}

    #region 公共方法
    /// <summary>
    /// 设置数量
    /// </summary>
    public void SetCount(int count)
    {
        _textCount.text = count.ToString();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _configManager = GameServiceLocator.ConfigManager;
    }

    /// <summary>
    /// 加载动物头像
    /// </summary>
    private void LoadHeadImage(int ID)
    {
        _imageAnimalHead.sprite = _configManager._AnimalRefSo.GetAnimalIcon(ID);
    }
    #endregion
}
