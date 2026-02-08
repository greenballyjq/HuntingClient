using Hunting.Game.Animal;

/// <summary>
/// 离场动物事件触发器组件
/// </summary>
public class LeaveAnimalEventTrigger : BaseAnimalEventTrigger
{
    /// <summary>
    /// 触发动物离场事件
    /// </summary>
    public void TriggerAnimalLeft()
    {
        _eventManager.Trigger(AnimalEvents.AnimalLeft, new AnimalLeftEventArgs
        {
            Sender = this,
            Animal = _animalBehaviour,
        });
    }

}
