using UnityEngine;

public class BossAnimalEventTrigger : MonoBehaviour
{
    private EventManager _eventManager => GameServiceLocator.EventManager;

    public void TriggerBossDied()
    {
        _eventManager.Trigger(BossEvents.BossDied, new BossDiedEventArgs());
    }
}