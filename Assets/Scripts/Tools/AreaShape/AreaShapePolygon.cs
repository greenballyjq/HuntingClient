using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多边形区域形状
/// </summary>
public class AreaShapePolygon : AreaShape
{
    /// <summary>
    /// 顶点列表
    /// </summary>
    [SerializeField] private List<Vector3> _vertices = new List<Vector3>();

    /// <summary>
    /// 缓存中心点
    /// </summary>
    private Vector3 _cachedCenter;

    /// <summary>
    /// 缓存包围半径
    /// </summary>
    private float _cachedBoundingRadius;

    /// <summary>
    /// 缓存2D顶点列表
    /// </summary>
    private List<Vector2> _cachedVertices2D;

    private void OnValidate()
    {
        RebuildCache();
    }

    private void Awake()
    {
        RebuildCache();
    }

    #region 公共方法
    public override bool IsInside(Vector3 worldPos)
    {
        if (_cachedVertices2D == null || _cachedVertices2D.Count < 3)
            return false;

        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);

        // 从点向右发射射线，计算与多边形边的交点数量
        int intersectionCount = 0;
        for (int i = 0; i < _cachedVertices2D.Count; i++)
        {
            Vector2 v1 = _cachedVertices2D[i];
            Vector2 v2 = _cachedVertices2D[(i + 1) % _cachedVertices2D.Count];

            if (RayIntersectsEdge(worldPos2D, v1, v2))
                intersectionCount++;
        }

        // 奇数个交点表示在内部
        return intersectionCount % 2 == 1;
    }

    public override bool IsOnBorder(Vector3 worldPos, float tolerance)
    {
        if (_cachedVertices2D == null || _cachedVertices2D.Count < 3)
            return false;

        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);

        // 计算点到每条边的最短距离
        for (int i = 0; i < _cachedVertices2D.Count; i++)
        {
            Vector2 v1 = _cachedVertices2D[i];
            Vector2 v2 = _cachedVertices2D[(i + 1) % _cachedVertices2D.Count];

            float distance = PointToEdgeDistance(worldPos2D, v1, v2);
            if (distance <= tolerance)
                return true;
        }

        return false;
    }

    public override float GetBoundingRadius()
    {
        return _cachedBoundingRadius;
    }

    public override Vector3 GetCenter()
    {
        return _cachedCenter;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 重建缓存
    /// </summary>
    private void RebuildCache()
    {
        if (_vertices.Count < 3)
        {
            _cachedCenter = transform.position;
            _cachedBoundingRadius = 0f;
            _cachedVertices2D = new List<Vector2>();
            return;
        }

        // 计算世界坐标下的2D顶点
        _cachedVertices2D = new List<Vector2>();
        Vector3 position = transform.position;
        foreach (var vertex in _vertices)
        {
            Vector3 worldVertex = new Vector3(vertex.x + position.x, position.y, vertex.z + position.z);
            _cachedVertices2D.Add(new Vector2(worldVertex.x, worldVertex.z));
        }

        // 计算中心点
        Vector2 center2D = Vector2.zero;
        foreach (var vertex2D in _cachedVertices2D)
        {
            center2D += vertex2D;
        }
        center2D /= _cachedVertices2D.Count;
        _cachedCenter = new Vector3(center2D.x, transform.position.y, center2D.y);

        // 计算包围半径
        _cachedBoundingRadius = 0f;
        foreach (var vertex2D in _cachedVertices2D)
        {
            float distance = Vector2.Distance(center2D, vertex2D);
            if (distance > _cachedBoundingRadius)
                _cachedBoundingRadius = distance;
        }
    }


    /// <summary>
    /// 判断射线是否与边相交
    /// </summary>
    private bool RayIntersectsEdge(Vector2 worldPos2D, Vector2 edgeStart, Vector2 edgeEnd)
    {
        // 检查边的两个顶点是否在射线的不同侧
        float y1 = edgeStart.y - worldPos2D.y;
        float y2 = edgeEnd.y - worldPos2D.y;
        
        // 如果两个顶点在射线的同一侧，不相交
        if ((y1 >= 0 && y2 >= 0) || (y1 < 0 && y2 < 0))
            return false;
        
        // 计算射线与边的交点X坐标
        float x1 = edgeStart.x - worldPos2D.x;
        float x2 = edgeEnd.x - worldPos2D.x;
        
        // 如果边的两个顶点都在射线左侧，不相交
        if (x1 < 0 && x2 < 0)
            return false;
        
        // 计算交点的X坐标
        float t = -y1 / (y2 - y1);
        float intersectionX = x1 + t * (x2 - x1);
        
        // 交点必须在射线右侧
        return intersectionX >= 0;
    }

    /// <summary>
    /// 计算点到边的最短距离
    /// </summary>
    private float PointToEdgeDistance(Vector2 worldPos2D, Vector2 edgeStart, Vector2 edgeEnd)
    {
        Vector2 edgeDir = edgeEnd - edgeStart;
        Vector2 toPoint = worldPos2D - edgeStart;

        float edgeLength = edgeDir.magnitude;
        if (edgeLength < 0.0001f)
            return Vector2.Distance(worldPos2D, edgeStart);

        // 计算投影
        float t = Mathf.Clamp01(Vector2.Dot(toPoint, edgeDir) / (edgeLength * edgeLength));
        Vector2 projection = edgeStart + edgeDir * t;

        return Vector2.Distance(worldPos2D, projection);
    }
    #endregion

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if (_vertices.Count < 3)
            return;

        Gizmos.color = _borderColor;

        // 绘制多边形轮廓
        Vector3 position = transform.position;
        for (int i = 0; i < _vertices.Count; i++)
        {
            Vector3 v1 = new Vector3(_vertices[i].x + position.x, position.y, _vertices[i].z + position.z);
            Vector3 v2 = new Vector3(_vertices[(i + 1) % _vertices.Count].x + position.x, position.y, _vertices[(i + 1) % _vertices.Count].z + position.z);
            Gizmos.DrawLine(v1, v2);
        }

        // 绘制中心点
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetCenter(), 0.15f);
    }
#endif
}