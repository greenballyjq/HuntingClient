namespace Hunting.Game.Animal
{
    public class FleeAnimalVisual : BaseAnimalVisual
    {
        public void PlayFlee()
        {
            ResetAnimationStates();
            _animator.SetBool("Flee", true);
        }

        protected override void ResetAnimationStates()
        {
            base.ResetAnimationStates();
            _animator.SetBool("Flee", false);
        }
    }
}
