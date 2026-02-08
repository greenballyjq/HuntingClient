using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 动物控制区域触发器
/// </summary>
public class AnimalControlAreaTrigger : AreaTriggerBase
{
    protected override void OnEnter(Collider collider)
    {
       
    }

    protected override void OnStay(Collider collider) 
    {
        collider.GetComponent<IControlable>().TakeControl();
    }

    protected override void OnExit(Collider collider) { }
}

