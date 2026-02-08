namespace Hunting.Game.Animal
{
    public class LeaveAnimalVisual : BaseAnimalVisual
    {
        /// <summary>
        /// 播放离场动画
        /// </summary>
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
