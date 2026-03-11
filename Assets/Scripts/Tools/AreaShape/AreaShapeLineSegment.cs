using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 线段区域形状，支持 Transform 旋转
/// </summary>
public class AreaShapeLineSegment : BaseAreaShape
{
    /// <summary>
    /// 起点（本地空间）
    /// </summary>
    [SerializeField] private Vector3 _start = new Vector3(-5f, 0, 0);

    /// <summary>
    /// 终点（本地空间）
    /// </summary>
    [SerializeField] private Vector3 _end = new Vector3(5f, 0, 0);

    /// <summary>
    /// 线段宽度
    /// </summary>
    [SerializeField] private float _width = 1f;

    #region 公共方法

    public override bool IsInside(Vector3 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        Vector2 pos2D = new Vector2(local.x, local.z);
        Vector2 start2D = new Vector2(_start.x, _start.z);
        Vector2 end2D = new Vector2(_end.x, _end.z);

        float halfWidth = _width * 0.5f;
        float sqrHalfWidth = halfWidth * halfWidth;
        return PointToSegmentSqrDistance(pos2D, start2D, end2D) <= sqrHalfWidth;
    }

    public override bool IsOnBorder(Vector3 worldPos, float tolerance)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        Vector2 pos2D = new Vector2(local.x, local.z);
        Vector2 start2D = new Vector2(_start.x, _start.z);
        Vector2 end2D = new Vector2(_end.x, _end.z);

        float halfWidth = _width * 0.5f;
        float distance = Mathf.Sqrt(PointToSegmentSqrDistance(pos2D, start2D, end2D));
        return Mathf.Abs(distance - halfWidth) <= tolerance;
    }

    public override float GetBoundingRadius()
    {
        float length = Vector2.Distance(new Vector2(_start.x, _start.z), new Vector2(_end.x, _end.z));
        return length * 0.5f + _width * 0.5f;
    }

    public override Vector3 GetCenter()
    {
        Vector3 localCenter = (_start + _end) * 0.5f;
        return transform.TransformPoint(localCenter);
    }

    public override void GetOutlinePoints(List<Vector3> outPoints)
    {
        Vector2 start2D = new Vector2(_start.x, _start.z);
        Vector2 end2D = new Vector2(_end.x, _end.z);
        Vector2 dir = end2D - start2D;
        float len = dir.magnitude;

        if (len < 1e-8f) return;

        Vector2 perp = new Vector2(-dir.y, dir.x) / len;
        float halfWidth = _width * 0.5f;

        outPoints.Add(transform.TransformPoint(new Vector3(start2D.x + perp.x * halfWidth, 0, start2D.y + perp.y * halfWidth)));
        outPoints.Add(transform.TransformPoint(new Vector3(end2D.x + perp.x * halfWidth, 0, end2D.y + perp.y * halfWidth)));
        outPoints.Add(transform.TransformPoint(new Vector3(end2D.x - perp.x * halfWidth, 0, end2D.y - perp.y * halfWidth)));
        outPoints.Add(transform.TransformPoint(new Vector3(start2D.x - perp.x * halfWidth, 0, start2D.y - perp.y * halfWidth)));
    }

    public override Vector3 GetRandomPoint(int maxAttempts = 100)
    {
        Vector2 start2D = new Vector2(_start.x, _start.z);
        Vector2 end2D = new Vector2(_end.x, _end.z);
        Vector2 dir = end2D - start2D;
        float sqrLen = dir.sqrMagnitude;

        if (sqrLen < 1e-8f)
        {
            return transform.TransformPoint(_start);
        }

        float t = Random.Range(0f, 1f);
        float halfWidth = _width * 0.5f;
        float offset = Random.Range(-halfWidth, halfWidth);

        Vector2 perp = new Vector2(-dir.y, dir.x);
        perp /= Mathf.Sqrt(sqrLen);

        Vector2 local2D = start2D + dir * t + perp * offset;
        return transform.TransformPoint(new Vector3(local2D.x, 0, local2D.y));
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 点到线段的最短距离的平方
    /// </summary>
    private float PointToSegmentSqrDistance(Vector2 pos, Vector2 segmentStart, Vector2 segmentEnd)
    {
        Vector2 dir = segmentEnd - segmentStart;
        Vector2 toPos = pos - segmentStart;

        float sqrLen = dir.sqrMagnitude;
        if (sqrLen < 1e-8f)
            return toPos.sqrMagnitude;

        float t = Mathf.Clamp01(Vector2.Dot(toPos, dir) / sqrLen);
        Vector2 projection = segmentStart + dir * t;
        return (pos - projection).sqrMagnitude;
    }

    #endregion

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        Gizmos.color = _borderColor;

        Vector2 start2D = new Vector2(_start.x, _start.z);
        Vector2 end2D = new Vector2(_end.x, _end.z);
        Vector2 dir = end2D - start2D;
        float len = dir.magnitude;

        if (len < 1e-8f)
        {
            Vector3 p = transform.TransformPoint(_start);
            Gizmos.DrawWireSphere(p, _width * 0.5f);
        }
        else
        {
            Vector2 perp = new Vector2(-dir.y, dir.x) / len;
            float halfWidth = _width * 0.5f;

            Vector3 p0 = transform.TransformPoint(new Vector3(start2D.x + perp.x * halfWidth, 0, start2D.y + perp.y * halfWidth));
            Vector3 p1 = transform.TransformPoint(new Vector3(start2D.x - perp.x * halfWidth, 0, start2D.y - perp.y * halfWidth));
            Vector3 p2 = transform.TransformPoint(new Vector3(end2D.x - perp.x * halfWidth, 0, end2D.y - perp.y * halfWidth));
            Vector3 p3 = transform.TransformPoint(new Vector3(end2D.x + perp.x * halfWidth, 0, end2D.y + perp.y * halfWidth));

            Gizmos.DrawLine(p0, p3);
            Gizmos.DrawLine(p3, p2);
            Gizmos.DrawLine(p2, p1);
            Gizmos.DrawLine(p1, p0);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetCenter(), 0.15f);

        base.OnDrawGizmos();
    }
#endif
}
