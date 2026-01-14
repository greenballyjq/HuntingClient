using System;
using UnityEngine;

public class AnimalGuardState : AnimalState
{
    private IGuardPolicy _guardPolicy;

    private Vector3 _guardPosition;
    
    private Transform _guardTarget;
    private readonly Func<Transform> _getGuardTarget;
    private readonly Func<int> _getGuardIndex;

    public AnimalGuardState(AnimalBehavior animal, StateMachine stateMachine, string animationName
        , Func<Transform> getGuardTarget, Func<int> getGuardIndex) :
        base(animal, stateMachine, animationName)
    {
        _getGuardTarget = getGuardTarget;
        _getGuardIndex = getGuardIndex;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"[{GetType().Name}] 进入Guard状态");
        stateTimer = 0f;
        _guardTarget = _getGuardTarget.Invoke();
        var guardIndex = _getGuardIndex.Invoke();
        _guardPolicy = new FanFormationGuardPolicy(BossBehaviour.MAX_FOLLOW_ANIMALS, guardIndex, 6f, 120f);
        // _eventManager.AddListener(BossEvents.BossCall, OnBossCall);
        animal.RVO.SetMaxSpeed(animal.CurrentMoveSpeed);
    }

    public override void Update()
    {
        base.Update();
        // 守卫逻辑
        stateTimer += Time.deltaTime;
        if (stateTimer < BossBehaviour.FOLLOW_DURATION)
        {
            var guardPosition = _guardPolicy.CalculateGuardPosition(animal, _guardTarget);
            float minDistance = 2f;
            if (Vector3.Distance(guardPosition, animal.transform.position) <= minDistance)
            {
                // 停止
                animal.PlayAnimationBool("Idle");
                animal.StopAnimation("Move");
                animal.RVO.SetMaxSpeed(0f);
            }
            else
            {
                animal.PlayAnimationBool("Move");
                animal.StopAnimation("Idle");
                animal.RVO.SetMaxSpeed(animal.CurrentMoveSpeed);
                var guardDirection = (guardPosition - animal.transform.position).normalized;
                animal.RVO.SetMoveDirection(guardDirection);
            }
        }
        else
        {
            var x = UnityEngine.Random.Range(0.7f, 1f);
            var z = UnityEngine.Random.Range(0, 0.3f);
            var randomDirection = new Vector3(x, 0, z);
            animal.RVO.SetMoveDirection(randomDirection);
            stateMachine.ChangeState(animal.GetMoveState());
        }
    }

    public override void Exit()
    {
        base.Exit();
        // _eventManager.RemoveListener(BossEvents.BossCall, OnBossCall);
    }
}