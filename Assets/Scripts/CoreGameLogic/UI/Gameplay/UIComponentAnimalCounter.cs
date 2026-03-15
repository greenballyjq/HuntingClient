using System.Collections.Generic;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 动物统计组件
/// </summary>
public class UIComponentAnimalCounter : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 动物统计项组件数组
    /// </summary>
    [SerializeField] private UIComponentAnimalCounterItem[] _animalCounterItems;

    /// <summary>
    /// 动物统计项组件字典
    /// </summary>
    private Dictionary<int, UIComponentAnimalCounterItem> _animalCounterItemsDic = new Dictionary<int, UIComponentAnimalCounterItem>();

    private EventManager _eventManager;
    private AnimalManager _animalManager;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
    }

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

    /// <summary>
    /// 初始化动物统计项组件
    /// </summary>
    private void InitializeItems()
    {
        var cachedSpecies = _animalManager.MapSpeciesDataCacheDic;
        int index = 0;

        foreach (var specieData in cachedSpecies.Values)
        {
            if (index >= _animalCounterItems.Length)
                break;

            var item = _animalCounterItems[index];
            item.Init(specieData);
            _animalCounterItemsDic.Add(specieData.ID, item);
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
