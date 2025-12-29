using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 障碍类型
/// </summary>
public enum ObstacleType
{
/// <summary>
/// 实体障碍
/// </summary>
Solid,

/// <summary>
/// 区域边界
/// </summary>
Boundary
}

/// <summary>
/// 矩形障碍组件
/// </summary>
public class RVOObstacleRect : MonoBehaviour
{
[Header("障碍尺寸")]
public Vector2 size = new Vector2(10f, 10f);

[Header("障碍类型")]
public ObstacleType obstacleType = ObstacleType.Solid;

/// <summary>
/// RVO管理器
/// </summary>
private RVOManager _rvoManager => RVOManager.Instance;

private void Start()
{
    CreateObstacle();
}

/// <summary>
/// 创建障碍
/// </summary>
private void CreateObstacle()
{
    // 获取顶点
    List<Vector3> vertices = GetVertices();
    
    // 注册障碍
    bool ccw = obstacleType == ObstacleType.Solid;
    _rvoManager.RegisterObstaclePolygon(vertices, owner: this, ccw: ccw);
    
    // 立即处理使其生效
    _rvoManager.ProcessAllObstacles();
}

/// <summary>
/// 获取矩形的四个顶点
/// </summary>
private List<Vector3> GetVertices()
{
    float hx = size.x * 0.5f;
    float hz = size.y * 0.5f;

    Vector3 localP0 = new Vector3(-hx, 0f, -hz);
    Vector3 localP1 = new Vector3(hx, 0f, -hz);
    Vector3 localP2 = new Vector3(hx, 0f, hz);
    Vector3 localP3 = new Vector3(-hx, 0f, hz);

    return new List<Vector3>
    {
        transform.TransformPoint(localP0),
        transform.TransformPoint(localP1),
        transform.TransformPoint(localP2),
        transform.TransformPoint(localP3)
    };
}

private void OnDrawGizmos()
{
    Gizmos.color = new Color(1f, 0f, 0f, 0.7f);

    List<Vector3> vertices = GetVertices();
    
    for (int i = 0; i < vertices.Count; i++)
    {
        int next = (i + 1) % vertices.Count;
        Gizmos.DrawLine(vertices[i], vertices[next]);
    }

    Gizmos.color = Color.red;
    Gizmos.DrawSphere(transform.position, 0.2f);
}
}
