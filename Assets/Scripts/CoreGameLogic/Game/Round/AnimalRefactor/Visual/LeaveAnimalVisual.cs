namespace Hunting.Game.Animal
{
    public class LeaveAnimalVisual : BaseAnimalVisual
    {
        public void PlayLeave()
        {
            ResetAnimationStates();
            _animator.SetBool("Leave", true);
        }

        protected override void ResetAnimationStates()
        {
            base.ResetAnimationStates();
            _animator.SetBool("Leave", false);
        }
    }
}
