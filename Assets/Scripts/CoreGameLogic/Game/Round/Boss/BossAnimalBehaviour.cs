using Hunting.Events;
using Hunting.Game.Animal;

public class BossAnimalBehaviour : BaseAnimalBehaviour
{
    private EventManager _eventManager => GameServiceLocator.EventManager;

    private void Start()
    {
        _eventManager.AddListener(HiddenMapEvents.HiddenMapPlayStart, OnHiddenMapStart);
    }

    private void OnDestroy()
    {
        _eventManager.RemoveListener(HiddenMapEvents.HiddenMapPlayStart, OnHiddenMapStart);
    }

    private void OnHiddenMapStart()
    {
        Moveable.StartMove();
    }
}