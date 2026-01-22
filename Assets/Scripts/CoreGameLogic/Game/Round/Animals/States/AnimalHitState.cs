//using UnityEngine;

///// <summary>
///// 动物被击中状态
///// </summary>
//public class AnimalHitState : AnimalState
//{
//    /// <summary>
//    /// 受击持续时间
//    /// </summary>
//    private float _hitDuration;

//    public AnimalHitState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
//        : base(animal, stateMachine, animationName)
//    {
//    }

//    public override void Enter()
//    {
//        base.Enter();

//        _hitDuration = animal.GetAnimationLength("Hit");
//        stateTimer = _hitDuration;

//        // 进入受击时清一次命中标记
//        animal.ResetHitFlag();

//        // 应用受击减速倍率（0倍速，即停止）
//        animal.ApplySpeedMultiplier(animal.HitSpeedMultiplier);
//    }

//    public override void Tick()
//    {
//        base.Tick();

//        if (animal.IsHit)
//        {
//            animal.ResetHitFlag();
//            stateMachine.ChangeState(animal.GetHitState());
//            return;
//        }

//        // 状态计时器倒计时
//        stateTimer -= Time.deltaTime;

//        // 本次受击结束后，根据逻辑回到移动或逃跑
//        if (stateTimer <= 0f)
//        {
//            if (animal.IsFleeing || animal.TimeInScene >= animal.StayTime)
//            {
//                stateMachine.ChangeState(animal.GetFleeState());
//            }
//            else
//            {
//                stateMachine.ChangeState(animal.GetMoveState());
//            }
//        }
//    }

//    public override void Exit()
//    {
//        base.Exit();
//        animal.ApplySpeedMultiplier(1f);
//    }
//}