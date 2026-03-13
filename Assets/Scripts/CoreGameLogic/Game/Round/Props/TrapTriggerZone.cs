using System;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 陷阱触发区域
/// </summary>
public class TrapTriggerZone : MonoBehaviour
{
    /// <summary>
    /// 动物进入触发范围时回调
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
    /// 初始化触发区域
    /// </summary>
    /// <param name="radius">触发半径</param>
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
