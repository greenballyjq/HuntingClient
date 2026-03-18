using System.Collections.Generic;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 动物统计组件
/// </summary>
public class UIComponentAnimalCounter : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 动物统计项
    /// </summary>
    [SerializeField] private UIComponentAnimalCounterItem[] _animalCounterItems;

    /// <summary>
    /// 动物统计项字典
    /// </summary>
    private Dictionary<int, UIComponentAnimalCounterItem> _animalCounterItemsDic = new Dictionary<int, UIComponentAnimalCounterItem>();

    private EventManager _eventManager;
    private AnimalManager _animalManager;

    private void Awake()
    {
        RegisterServers();
    }

    public void Init()
    {
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
        InitializeItems();
    }

    public void CleanUp()
    {
        _animalCounterItemsDic.Clear();
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _eventManager = GameServiceLocator.EventManager;
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
    }

    /// <summary>
    /// 初始化动物统计项组件
    /// </summary>
    private void InitializeItems()
    {
        var animals = new List<Specie>();
        foreach (var s in _animalManager.AnimalDatas.Values)
        {
            if (s.SpecieType == ESpecieType.Small || s.SpecieType == ESpecieType.Medium || s.SpecieType == ESpecieType.Large)
                animals.Add(s);
        }
        for (int i = 0; i < animals.Count; i++)
        {
            _animalCounterItems[i].Init(animals[i]);
            _animalCounterItemsDic.Add(animals[i].ID, _animalCounterItems[i]);
        }
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (_animalCounterItemsDic.TryGetValue(args.SpecieData.ID, out var item))
            item.SetCount(_animalManager.GetHuntingCount(args.SpecieData.ID));
    }
    #endregion
}
