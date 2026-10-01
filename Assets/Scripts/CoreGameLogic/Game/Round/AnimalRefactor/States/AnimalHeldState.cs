using System.Collections;
using System.Collections.Generic;
using Hunting.Game.Animal;
using UnityEngine;

public class AnimalHeldState : AnimalState
{
    /// <summary>
    /// 被控制持续时间
    /// </summary>
    protected float _heldDuration = 2f;

    public AnimalHeldState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
    {
    }

    public void SetDuration(float duration)
    {
        _heldDuration = duration;
    }

    public override void Enter()
    {
        base.Enter();

        animalBehavior.Moveable.StopMove();
        animalBehavior.AnimalVisual.PlayHeld();
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);

        if (stateTimer >= _heldDuration)
            stateMachine.ExitTempState();
    }

    public override void Exit()
    {
        animalBehavior.Moveable.StartMove();
        base.Exit();
    }
}
