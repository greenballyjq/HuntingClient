using System;
using UnityEngine;

/// <summary>
/// Boss移动点标记
/// </summary>
public class BossMovePoint : MonoBehaviour
{
    /// <summary>
    /// 目标Transform
    /// </summary>
    public Transform Target => transform;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}