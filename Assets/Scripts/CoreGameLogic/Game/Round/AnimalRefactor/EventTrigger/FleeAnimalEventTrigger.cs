using Hunting.Game.Animal;


public class FleeAnimalEventTrigger : BaseAnimalEventTrigger
{
    /// <summary>
    /// 触发动物逃跑事件
    /// </summary>
    public void TriggerAnimalFled()
    {
        _eventManager.Trigger(AnimalEvents.AnimalFled, new AnimalFledEventArgs
        {
            Sender = this,
            Animal = _animalBehaviour,
            SpecieData = _animalBehaviour.SpecieData
        });
    }

}
