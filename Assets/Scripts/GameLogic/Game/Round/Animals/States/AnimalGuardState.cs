using GameLogic.Game.Round.Animals;
using UnityEngine;

public class AnimalGuardState : AnimalState
{
    private BossBehaviour _boss;

    private IGuardPolicy _guardPolicy;

    private Vector3 _guardPosition;

    public AnimalGuardState(AnimalBehavior animal, StateMachine stateMachine, string animationName) :
        base(animal, stateMachine, animationName)
    {
    }
}