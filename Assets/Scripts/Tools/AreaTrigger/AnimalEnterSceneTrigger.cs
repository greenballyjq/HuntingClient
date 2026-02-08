using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 动物进入场景区域触发器 
/// </summary>
public class AnimalEnterSceneAreaTrigger : AreaTriggerBase
{
    protected override void OnEnter(Collider collider)
    {
        if(collider.GetComponent<BaseAnimalBehaviour>() is LeaveAnimalBehaviour leaveAnimal){
            leaveAnimal.StartStay();
        }
    }

    protected override void OnStay(Collider collider) { }

    protected override void OnExit(Collider collider) { }
}

