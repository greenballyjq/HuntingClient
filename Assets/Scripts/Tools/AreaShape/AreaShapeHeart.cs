using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爱心区域形状
/// </summary>
public class AreaShapeHeart : BaseAreaShape
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
    /// 本地空间2D顶点缓存
    /// </summary>
    private Vector2[] _cachedVertices2D;

    /// <summary>
    /// 本地空间质心
    /// </summary>
    private Vector2 _cachedCenterLocal;

    /// <summary>
    /// 质心到顶点的最大距离的平方
    /// </summary>
    private float _cachedBoundingRadiusSqr;

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
        // 射线法：从点向右发射水平射线，与爱心轮廓边交点为奇数则在内部

        if (_cachedVertices2D == null || _cachedVertices2D.Length < 3)
            return false;

        Vector3 local = transform.InverseTransformPoint(worldPos);
        Vector2 pos2D = new Vector2(local.x, local.z);

        int intersectionCount = 0;
        int vertexCount = _cachedVertices2D.Length;
        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 v1 = _cachedVertices2D[i];
            Vector2 v2 = _cachedVertices2D[(i + 1) % vertexCount];

            if (RayIntersectsEdge(pos2D, v1, v2))
                intersectionCount++;
        }

        return (intersectionCount & 1) == 1;
    }

    public override bool IsOnBorder(Vector3 worldPos, float tolerance)
    {
        if (_cachedVertices2D == null || _cachedVertices2D.Length < 3)
            return false;

        Vector3 local = transform.InverseTransformPoint(worldPos);
        Vector2 pos2D = new Vector2(local.x, local.z);
        float toleranceSqr = tolerance * tolerance;

        int vertexCount = _cachedVertices2D.Length;
        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 v1 = _cachedVertices2D[i];
            Vector2 v2 = _cachedVertices2D[(i + 1) % vertexCount];

            if (PointToEdgeSqrDistance(pos2D, v1, v2) <= toleranceSqr)
                return true;
        }

        return false;
    }

    public override float GetBoundingRadius()
    {
        return Mathf.Sqrt(_cachedBoundingRadiusSqr);
    }

    public override Vector3 GetCenter()
    {
        return transform.TransformPoint(new Vector3(_cachedCenterLocal.x, 0, _cachedCenterLocal.y));
    }

    public override void GetOutlinePoints(List<Vector3> outPoints)
    {
        if (_precision < 3) return;

        if (_cachedVertices2D == null || _cachedVertices2D.Length != _precision)
            RebuildCache();

        int count = _cachedVertices2D.Length;
        for (int i = 0; i < count; i++)
        {
            Vector2 p = _cachedVertices2D[i];
            outPoints.Add(transform.TransformPoint(new Vector3(p.x, 0, p.y)));
        }
    }

    #endregion

    #region 私有方法
    /// <summary>
    /// 重建本地空间顶点、质心、包围半径缓存
    /// </summary>
    private void RebuildCache()
    {
        if (_precision < 3)
        {
            _cachedVertices2D = null;
            _cachedCenterLocal = Vector2.zero;
            _cachedBoundingRadiusSqr = 0f;
            return;
        }

        int count = _precision;
        if (_cachedVertices2D == null || _cachedVertices2D.Length != count)
            _cachedVertices2D = new Vector2[count];

        Vector2 sum = Vector2.zero;
        float scale = _scale * 0.1f;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count * 2f * Mathf.PI;
            float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
            float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);

            Vector2 p = new Vector2(x, y) * scale;
            _cachedVertices2D[i] = p;
            sum += p;
        }

        _cachedCenterLocal = sum / count;

        float maxSqrDist = 0f;
        for (int i = 0; i < count; i++)
        {
            float sqrDist = (_cachedVertices2D[i] - _cachedCenterLocal).sqrMagnitude;
            if (sqrDist > maxSqrDist)
                maxSqrDist = sqrDist;
        }
        _cachedBoundingRadiusSqr = maxSqrDist;
    }

    /// <summary>
    /// 判断从pos向右的水平射线是否与边 (edgeStart, edgeEnd) 相交
    /// </summary>
    private bool RayIntersectsEdge(Vector2 pos, Vector2 edgeStart, Vector2 edgeEnd)
    {
        // 算法：两端点在射线上下两侧，且交点 x 非负

        float y1 = edgeStart.y - pos.y;
        float y2 = edgeEnd.y - pos.y;
        if ((y1 >= 0 && y2 >= 0) || (y1 < 0 && y2 < 0))
            return false;

        float x1 = edgeStart.x - pos.x;
        float x2 = edgeEnd.x - pos.x;
        if (x1 < 0 && x2 < 0)
            return false;

        float t = -y1 / (y2 - y1);
        float intersectionX = x1 + t * (x2 - x1);
        return intersectionX >= 0;
    }

    /// <summary>
    /// 点到边的最短距离的平方
    /// </summary>
    private float PointToEdgeSqrDistance(Vector2 pos, Vector2 edgeStart, Vector2 edgeEnd)
    {
        // 算法：点在边上的投影 Clamp 到 [0,1]，最近点即为投影点或端点

        Vector2 edgeDir = edgeEnd - edgeStart;
        Vector2 toPoint = pos - edgeStart;

        float sqrEdgeLen = edgeDir.sqrMagnitude;
        if (sqrEdgeLen < 1e-8f)
            return toPoint.sqrMagnitude;

        float t = Mathf.Clamp01(Vector2.Dot(toPoint, edgeDir) / sqrEdgeLen);
        Vector2 projection = edgeStart + edgeDir * t;
        return (pos - projection).sqrMagnitude;
    }

    #endregion

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if (_precision >= 3 && (_cachedVertices2D == null || _cachedVertices2D.Length < 3))
            RebuildCache();

        if (_cachedVertices2D == null || _cachedVertices2D.Length < 3)
            return;

        Gizmos.color = _borderColor;

        int count = _cachedVertices2D.Length;
        for (int i = 0; i < count; i++)
        {
            Vector3 v1 = transform.TransformPoint(new Vector3(_cachedVertices2D[i].x, 0, _cachedVertices2D[i].y));
            Vector3 v2 = transform.TransformPoint(new Vector3(_cachedVertices2D[(i + 1) % count].x, 0, _cachedVertices2D[(i + 1) % count].y));
            Gizmos.DrawLine(v1, v2);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetCenter(), 0.15f);

        base.OnDrawGizmos();
    }
#endif
}
