using GameFramework.Core;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 动物离开区域触发器
/// </summary>
public class AnimalLeaveAreaTrigger : AreaTriggerBase
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
    }

    protected override void OnEnter(Collider collider)
    {
        var animal = collider.GetComponent<BaseAnimalBehaviour>();
        _eventManager.Trigger(AnimalEvents.AnimalLeft, new AnimalLeftEventArgs
        {
            Animal = animal
        });
    }

    protected override void OnStay(Collider collider) {}

    protected override void OnExit(Collider collider) {}
}

