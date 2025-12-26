using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多边形障碍组件
/// </summary>
public class RVOObstaclePolygon : MonoBehaviour
{
    [Header("顶点列表")]
    public List<Vector3> vertices = new List<Vector3>();

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
        if (vertices == null || vertices.Count < 2)
        {
            Debug.LogWarning($"[RVOObstaclePolygon] {gameObject.name} 顶点数量不足（至少需要2个）");
            return;
        }

        List<Vector3> worldVertices = GetVertices();
        
        bool ccw = obstacleType == ObstacleType.Solid;
        _rvoManager.RegisterObstaclePolygon(worldVertices, owner: this, ccw: ccw);
        
        _rvoManager.ProcessAllObstacles();
    }

    /// <summary>
    /// 获取世界坐标下的顶点
    /// </summary>
    private List<Vector3> GetVertices()
    {
        List<Vector3> worldVertices = new List<Vector3>(vertices.Count);
        
        foreach (var localVertex in vertices)
        {
            worldVertices.Add(transform.TransformPoint(localVertex));
        }
        
        return worldVertices;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null || vertices.Count < 2)
            return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.7f);

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            int next = (i + 1) % vertices.Count;
            Vector3 nextWorldPos = transform.TransformPoint(vertices[next]);
            
            Gizmos.DrawLine(worldPos, nextWorldPos);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}

