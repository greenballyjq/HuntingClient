using Hunting.Game.Animal;

public class BossAnimalVisual : BaseAnimalVisual
{
    public void PlayCall()
    {
        _animator.SetBool("Call", true);
    }
    
    protected override void ResetAnimationStates()
    {
        base.ResetAnimationStates();
        _animator.SetBool("Call", false);
    }
}