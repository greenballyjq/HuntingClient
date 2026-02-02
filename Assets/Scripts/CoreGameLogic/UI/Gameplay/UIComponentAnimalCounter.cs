using System.Collections.Generic;
using GameFramework.Core.UI;
using UnityEngine;

/// <summary>
/// 动物统计组件
/// </summary>
public class UIComponentAnimalCounter : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 统计项数组
    /// </summary>
    [SerializeField] private UIComponentAnimalCounterItem[] _animalCounterItems;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 动物管理器
    /// </summary>
    private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    /// <summary>
    /// 统计项组件字典
    /// </summary>
    private Dictionary<int, UIComponentAnimalCounterItem> _animalCounterItemsDic = new Dictionary<int, UIComponentAnimalCounterItem>();

    public void Init()
    {
        InitializeItems();
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
        _animalCounterItemsDic.Clear();
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    /// <summary>
    /// 初始化统计项
    /// </summary>
    private void InitializeItems()
    {
        var cachedSpecies = _animalManager.GetCachedSpeciesData();
        int index = 0;

        foreach (var specieEntry in cachedSpecies)
        {
            if (index >= _animalCounterItems.Length)
                break;

            int specieId = specieEntry.Key;
            var specieData = specieEntry.Value;
            var item = _animalCounterItems[index];

            item.Init(specieData);
            _animalCounterItemsDic.Add(specieId, item);
            index++;
        }
    }

    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (_animalCounterItemsDic.TryGetValue(args.SpecieData.ID, out var item))
            item.AddCount();
    }
}
