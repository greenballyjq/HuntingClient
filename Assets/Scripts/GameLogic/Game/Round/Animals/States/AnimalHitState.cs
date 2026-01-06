using UnityEngine;

/// <summary>
/// 动物被击中状态
/// </summary>
public class AnimalHitState : AnimalState
{
    /// <summary>
    /// 受击状态持续时间（从动画获取）
    /// </summary>
    private float _hitDuration;

    public AnimalHitState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
        : base(animal, stateMachine, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // 从动画片段获取持续时间
        _hitDuration = animal.GetAnimationLength("Clicked");
        stateTimer = _hitDuration;
        
        // 应用减速倍率
        animal.ApplySpeedMultiplier(animal.HitSpeedMultiplier);
        
        // 播放视觉效果
        animal.PlayHitEffect();
    }

    public override void Update()
    {
        base.Update();
        
        // 状态计时器倒计时
        stateTimer -= Time.deltaTime;

        // 重复命中：不切换状态，重置计时器并播放效果
        if (animal.IsHit)
        {
            stateTimer = _hitDuration;
            animal.ResetHitFlag();
            animal.PlayHitEffect();
            return;
        }

        // 受击状态结束
        if (stateTimer <= 0f)
        {
            // 如果是从逃跑状态进入的，返回逃跑（保持逃跑倒计时）
            if (animal.IsFleeing)
            {
                stateMachine.ChangeState(animal.GetFleeState());
            }
            // 驻场时间到，进入逃跑（第一次进入逃跑）
            else if (animal.TimeInScene >= animal.StayTime)
            {
                stateMachine.ChangeState(animal.GetFleeState());
            }
            // 否则回到移动状态
            else
            {
                stateMachine.ChangeState(animal.GetMoveState());
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // 恢复正常速度
        animal.ApplySpeedMultiplier(1f);
    }
}