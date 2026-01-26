namespace Hunting.Game.Animal
{
    public class BossAnimalVisual : BaseAnimalVisual
    {
        public void PlayEnter()
        {
            ResetAnimationStates();
            _animator.SetBool("Enter", true);
        }
        public void PlayCall()
        {
            ResetAnimationStates();
            _animator.SetBool("Call", true);
        }

        protected override void ResetAnimationStates()
        {
            base.ResetAnimationStates();
            _animator.SetBool("Call", false);
        }
    }
}
