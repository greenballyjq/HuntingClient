using Hunting.Game.Animal;

/// <summary>
/// 技能三千盘金币人动物视觉组件
/// </summary>
public class SkillThreeKPCoinManAnimalVisual : LeaveAnimalVisual
{
    /// <summary>
    /// 播放入场动画
    /// </summary>
    public void PlayEnter()
    {
        ResetAnimationStates();
        _animator.SetBool("Enter", true);
    }

    protected override void ResetAnimationStates()
    {
        base.ResetAnimationStates();
        _animator.SetBool("Enter", false);
    }
}
