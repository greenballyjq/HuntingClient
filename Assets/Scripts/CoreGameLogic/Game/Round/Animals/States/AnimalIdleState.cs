//using UnityEngine;

//public class AnimalIdleState : AnimalState
//{
//    public AnimalIdleState(AnimalBehavior animal, StateMachine stateMachine, string animationName) : base(animal,
//        stateMachine, animationName)
//    {
//    }

//    public override void Enter()
//    {
//        base.Enter();
//        // 设置速度为0（待机不动）
//        animal.Movement?.SetSpeed(0f);
//        stateTimer = 0f;
//    }

//    public override void Tick()
//    {
//        base.Tick();
//        stateTimer += Time.deltaTime;
//        if (stateTimer > 1f)
//        {
//            stateMachine.ChangeState(animal.GetGuardState());
//        }
//    }
//}