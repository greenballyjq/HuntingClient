using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
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
    /// 动物图像
    /// </summary>
    [SerializeField] private Image _imageAnimal;

    /// <summary>
    /// 动物数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textAnimalCount;

    /// <summary>
    /// 当前计数
    /// </summary>
    private int _currentCount;

    private HuntingConfigManager _configManager;

    private void Awake()
    {
        RegisterServers();
    }

    public void Init(Specie specieData)
    {
        _currentCount = 0;
        _textAnimalCount.text = "0";
        LoadIcon(specieData.ID);
    }

    public void CleanUp() {}

    #region 公共方法
    /// <summary>
    /// 设置数量
    /// </summary>
    public void SetCount(int count)
    {
        _currentCount = count;
        _textAnimalCount.text = count.ToString();
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
    /// 加载动物图标
    /// </summary>
    private void LoadIcon(int ID)
    {
        _imageAnimal.sprite = _configManager._AnimalRefSo.GetAnimalIcon(ID);
    }
    #endregion
}
