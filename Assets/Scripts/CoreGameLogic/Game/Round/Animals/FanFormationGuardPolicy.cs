using Hunting.Game.Animal;
using UnityEngine;

public class FanFormationGuardPolicy : IGuardPolicy
{
    private readonly int _totalGuards;
    private readonly int _guardIndex;
    private readonly float _defenseRadius;
    private readonly float _fanAngle;

    public FanFormationGuardPolicy(int totalGuards, int guardIndex, float defenseRadius, float fanAngle)
    {
        _totalGuards = totalGuards;
        _guardIndex = guardIndex;
        _defenseRadius = defenseRadius;
        _fanAngle = fanAngle;
    }

    public Vector3 CalculateGuardPosition(AnimalBehavior animalBehavior, Transform guardTarget)
    {
        float startAngle = -_fanAngle / 2f;
        float angleStep = _fanAngle / (_totalGuards - 1);
        float currentAngle = startAngle + angleStep * _guardIndex;

        // 将角度转换为位置
        Vector3 offset = Quaternion.Euler(0, currentAngle, 0) * Vector3.back * _defenseRadius;
        return guardTarget.position + offset;
    }
}