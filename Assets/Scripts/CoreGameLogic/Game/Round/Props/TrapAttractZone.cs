using System;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 陷阱吸引区域
/// </summary>
public class TrapAttractZone : MonoBehaviour
{
    /// <summary>
    /// 动物进入事件回调
    /// </summary>
    public event Action<BaseAnimalBehaviour> OnAnimalEntered;

    /// <summary>
    /// 碰撞体
    /// </summary>
    private SphereCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// 初始化吸引区域
    /// </summary>
    /// <param name="radius">吸引半径</param>
    public void Init(float radius)
    {
        _collider.radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.GetComponent<BaseAnimalBehaviour>();

        if (animal == null) 
            return;

        OnAnimalEntered?.Invoke(animal);
    }
}
