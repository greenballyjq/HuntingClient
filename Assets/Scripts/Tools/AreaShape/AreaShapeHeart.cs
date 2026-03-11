using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爱心区域形状
/// </summary>
public class AreaShapeHeart : AreaShape
{
    /// <summary>
    /// 尺寸缩放
    /// </summary>
    [SerializeField] private float _scale = 1f;

    /// <summary>
    /// 轮廓精度
    /// </summary>
    [SerializeField] private int _precision = 100;

    /// <summary>
    /// 缓存中心点
    /// </summary>
    private Vector3 _cachedCenter;

    /// <summary>
    /// 缓存包围半径
    /// </summary>
    private float _cachedBoundingRadius;

    /// <summary>
    /// 缓存2D轮廓点列表
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

        // 从点向右发射射线，计算与爱心轮廓边的交点数量
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

        // 计算点到轮廓的最短距离
        float minDistance = float.MaxValue;
        for (int i = 0; i < _cachedVertices2D.Count; i++)
        {
            Vector2 v1 = _cachedVertices2D[i];
            Vector2 v2 = _cachedVertices2D[(i + 1) % _cachedVertices2D.Count];

            float distance = PointToEdgeDistance(worldPos2D, v1, v2);
            if (distance < minDistance)
                minDistance = distance;
        }

        return minDistance <= tolerance;
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
        if (_precision < 3)
        {
            _cachedCenter = transform.position;
            _cachedBoundingRadius = 0f;
            _cachedVertices2D = new List<Vector2>();
            return;
        }

        // 生成爱心轮廓点
        _cachedVertices2D = new List<Vector2>();
        Vector2 center2D = new Vector2(transform.position.x, transform.position.z);

        for (int i = 0; i < _precision; i++)
        {
            float t = (float)i / _precision * 2f * Mathf.PI;
            
            // 爱心参数方程
            float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
            float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);
            
            // 应用缩放并转换到世界坐标
            Vector2 point2D = center2D + new Vector2(x, y) * _scale * 0.1f;
            _cachedVertices2D.Add(point2D);
        }

        // 计算中心点
        Vector2 centerSum = Vector2.zero;
        foreach (var vertex2D in _cachedVertices2D)
        {
            centerSum += vertex2D;
        }
        Vector2 calculatedCenter2D = centerSum / _cachedVertices2D.Count;
        _cachedCenter = new Vector3(calculatedCenter2D.x, transform.position.y, calculatedCenter2D.y);

        // 计算包围半径
        _cachedBoundingRadius = 0f;
        foreach (var vertex2D in _cachedVertices2D)
        {
            float distance = Vector2.Distance(calculatedCenter2D, vertex2D);
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
        if (_precision < 3)
            return;

        Gizmos.color = _borderColor;

        // 绘制爱心轮廓
        Vector3 position = transform.position;
        Vector2 center2D = new Vector2(position.x, position.z);
        
        Vector2 prevPoint2D = Vector2.zero;
        bool isFirst = true;
        Vector2 centerSum2D = Vector2.zero;
        int pointCount = 0;
        
        for (int i = 0; i < _precision; i++)
        {
            float t = (float)i / _precision * 2f * Mathf.PI;
            
            float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
            float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);

            Vector2 point2D = center2D + new Vector2(x, y) * _scale * 0.1f;
            centerSum2D += point2D;
            pointCount++;
            
            if (!isFirst)
            {
                Vector3 worldV1 = new Vector3(prevPoint2D.x, position.y, prevPoint2D.y);
                Vector3 worldV2 = new Vector3(point2D.x, position.y, point2D.y);
                Gizmos.DrawLine(worldV1, worldV2);
            }
            
            prevPoint2D = point2D;
            isFirst = false;
        }

        // 绘制中心点
        Gizmos.color = Color.yellow;
        Vector2 calculatedCenter2D = centerSum2D / pointCount;
        Vector3 center = new Vector3(calculatedCenter2D.x, position.y, calculatedCenter2D.y);
        Gizmos.DrawWireSphere(center, 0.2f);
    }
#endif
}