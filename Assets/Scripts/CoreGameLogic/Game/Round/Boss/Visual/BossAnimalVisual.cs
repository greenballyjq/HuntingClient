using Hunting.Game.Animal;

public class BossAnimalVisual : BaseAnimalVisual
{

    public void PlayEnter()
    {
        _animator.SetBool("Enter", true);
    }
    
    protected override void ResetAnimationStates()
    {
        base.ResetAnimationStates();
    }
}