using CoreGameLogic.Game.Round.Animals;
using UnityEngine;

public class AnimalGuardState : AnimalState
{
    private BossBehaviour _boss;

    private IGuardPolicy _guardPolicy;

    private Vector3 _guardPosition;

    private EventManager _eventManager => GameServiceLocator.EventManager;

    private bool _startGuard;
    private float _guardDuration;

    public AnimalGuardState(AnimalBehavior animal, StateMachine stateMachine, string animationName) :
        base(animal, stateMachine, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"[{GetType().Name}] 进入Guard状态");
        stateTimer = 0f;
        _eventManager.AddListener(BossEvents.BossCall, OnBossCall);
        animal.RVO.SetMaxSpeed(animal.CurrentMoveSpeed);
    }

    public override void Update()
    {
        base.Update();
        if (_startGuard)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer > _guardDuration)
            {
                // animal.DisableGuard();
                // animal.EnableRVO();
                _startGuard = false;
                stateTimer = 0f;
                _boss.RemoveFollowAnimal(animal);
            }
            else
            {
                var guardPosition = _guardPolicy.CalculateGuardPosition(animal, _boss);

                if (Vector3.Distance(guardPosition, animal.transform.position) < 1f)
                {
                    // 切换至idle状态
                    stateMachine.ChangeState(animal.GetIdleState());
                }
                else
                {
                    var guardDirection = (guardPosition - animal.transform.position).normalized;
                    animal.RVO.SetMoveDirection(guardDirection);
                }
            }
        }
    }

    private void OnBossCall(BossCallEventArgs obj)
    {
        // if (!animal.HasGuardTarget()) return;
        _boss = obj.Boss;
        Debug.Log($"[{GetType().Name}] 监听到Boss召唤");
        // 响应Boss召唤，实现保护逻辑
        _guardDuration = obj.GuardDuration;

        var availableFollowIndex = obj.Boss.GetAvailableFollowIndex();

        if (availableFollowIndex == -1) return;

        _guardPolicy = new FanFormationGuardPolicy(BossBehaviour.MAX_FOLLOW_ANIMALS, availableFollowIndex, 7f, 100f);

        obj.Boss.AddFollowAnimal(animal);

        _startGuard = true;
    }

    public override void Exit()
    {
        base.Exit();
        _eventManager.RemoveListener(BossEvents.BossCall, OnBossCall);
    }
}