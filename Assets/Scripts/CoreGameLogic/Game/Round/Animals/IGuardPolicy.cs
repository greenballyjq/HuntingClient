using Hunting.Game.Animal;
using UnityEngine;

public interface IGuardPolicy
{
    public Vector3 CalculateGuardPosition(AnimalBehavior animalBehavior, Transform guardTarget);
}