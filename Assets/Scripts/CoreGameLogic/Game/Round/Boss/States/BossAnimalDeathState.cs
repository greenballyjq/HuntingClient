using Hunting.Game.Animal;

public class BossAnimalDeathState : AnimalDeathState
{
    public BossAnimalDeathState(StateMachine stateMachine, BaseAnimalBehaviour animal) : base(stateMachine, animal)
    {
    }

    protected override void HandleDeathExecution()
    {
        if(stateTimer >= 4f && !_isAnimalDropRewardTriggered){
            animalBehavior.AnimalEventTrigger.TriggerAnimalDropReward();
            animalBehavior.AnimalVisual.PlayDeathEffect();
            _isAnimalDropRewardTriggered = true;
        }

        if (stateTimer >= _deathDuration)
        {
            ((BossAnimalEventTrigger)animalBehavior.AnimalEventTrigger).TriggerBossDied();
        }
    }
}