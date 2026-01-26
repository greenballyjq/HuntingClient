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

        /// <summary>
        /// 触发Boss受伤事件
        /// </summary>
        /// <param name="maxHealth">最大血量</param>
        /// <param name="currentHealth">当前血量</param>
        public void TriggerBossDamaged(float maxHealth, float currentHealth)
        {
            _eventManager.Trigger(AnimalEvents.BossDamaged, new BossDamagedEventArgs
            {
                MaxHealth = maxHealth,
                CurrentHealth = currentHealth
            });
        }
    }
}
