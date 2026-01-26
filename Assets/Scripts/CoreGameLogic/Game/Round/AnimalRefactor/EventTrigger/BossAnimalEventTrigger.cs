namespace Hunting.Game.Animal
{
    public class BossAnimalEventTrigger : BaseAnimalEventTrigger
    {
        private BossAnimalBehaviour _bossAnimalBehaviour;

        private void Awake()
        {
            _bossAnimalBehaviour = GetComponent<BossAnimalBehaviour>();
        }


        public void TriggerBossDied()
        {
            _eventManager.Trigger(BossEvents.BossDied, new BossDiedEventArgs
            {
                Boss = _bossAnimalBehaviour
            });
        }
    }
}
