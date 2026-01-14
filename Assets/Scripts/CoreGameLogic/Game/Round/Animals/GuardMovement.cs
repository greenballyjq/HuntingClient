using System;
using CoreGameLogic.Game.Round.Animals;
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

    private EventManager _eventManager => GameServiceLocator.EventManager;

    private bool _startGuard;

    public void InitGuard(BossBehaviour bossBehaviour)
    {
        _bossBehaviour = bossBehaviour;
        var guardIndex = bossBehaviour.GetAvailableFollowIndex();
        Debug.Log($"[{GetType().Name}] guardIndex: {guardIndex}");
        _guardPolicy = new FanFormationGuardPolicy(BossBehaviour.MAX_FOLLOW_ANIMALS,
            guardIndex, 7f, 120f);
        _animalBehavior = GetComponent<AnimalBehavior>();
        _guardPosition = _guardPolicy.CalculateGuardPosition(_animalBehavior, _bossBehaviour);
        
        _eventManager.AddListener(AnimalEvents.AnimalDying, OnAnimalDying);
        SetGuard(false);
    }

    public void SetGuard(bool guard) => _startGuard = guard;

    private void OnDisable()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDying, OnAnimalDying);
    }

    private void OnAnimalDying(AnimalDyingEventArgs obj)
    {
        if (obj.Animal == _animalBehavior)
        {
            _startGuard = false;
        }
    }

    private void Update()
    {
        if (!_bossBehaviour || !_startGuard) return;
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
        _guardPosition = _guardPolicy.CalculateGuardPosition(_animalBehavior, _bossBehaviour);
        var direction = (_guardPosition - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        transform.Translate(direction * (Time.deltaTime * 5f), Space.World);
    }

    public bool HasTarget()
    {
        return _bossBehaviour != null;
    }
}