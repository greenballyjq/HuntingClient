using GameLogic.Game.Round.Animals;
using UnityEngine;

/// <summary>
/// 保护模式移动组件
/// </summary>
public class GuardMovement : MonoBehaviour
{
    private IGuardPolicy _guardPolicy;

    private AnimalBehavior _animalBehavior;
    private BossBehaviour _bossBehaviour;

    private Vector3 _guardPosition;

    public void InitGuard(BossBehaviour bossBehaviour)
    {
        _bossBehaviour = bossBehaviour;
        _guardPolicy = new FanFormationGuardPolicy(BossBehaviour.MAX_FOLLOW_ANIMALS,
            bossBehaviour.GetAvailableFollowIndex(), 10f, 120f);
        _animalBehavior = GetComponent<AnimalBehavior>();
        _guardPosition = _guardPolicy.CalculateGuardPosition(_animalBehavior, _bossBehaviour);
    }

    private void Update()
    {
        _guardPosition = _guardPolicy.CalculateGuardPosition(_animalBehavior, _bossBehaviour);
        if (Vector3.Distance(transform.position, _guardPosition) < 2f)
        {
            // 到达保护位置，停止移动
        }
        else
        {
            MoveToGuardPosition();
        }
    }

    private void MoveToGuardPosition()
    {
        Debug.Log($"[{GetType().Name}] MoveToGuardPosition");
        var direction = (_guardPosition - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        transform.Translate(direction * (Time.deltaTime * 5f), Space.World);
    }
}