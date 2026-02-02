using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
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
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    /// <summary>
    /// 当前数量
    /// </summary>
    private int _currentCount;

    public void Init(Specie specieData)
    {
        _currentCount = 0;
        _textAnimalCount.text = "0";
        LoadIconAsync(specieData.IconResourcePath).Forget();
    }

    public void CleanUp()
    {
    }

    /// <summary>
    /// 增加数量
    /// </summary>
    public void AddCount()
    {
        _currentCount++;
        _textAnimalCount.text = _currentCount.ToString();
    }

    /// <summary>
    /// 异步加载图标
    /// </summary>
    /// <param name="iconPath">图标资源路径</param>
    private async UniTask LoadIconAsync(string iconPath)
    {
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(iconPath);
        _imageAnimal.sprite = sprite;
    }
}
