using Hunting.Game.Animal;
using UnityEngine;

public interface IGuardPolicy
{
    public Vector3 CalculateGuardPosition(AnimalBehaviour animalBehavior, Transform guardTarget);
}