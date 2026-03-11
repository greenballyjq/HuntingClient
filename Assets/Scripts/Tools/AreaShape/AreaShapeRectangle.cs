using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 矩形区域形状
/// </summary>
public class AreaShapeRectangle : BaseAreaShape
{
    /// <summary>
    /// 宽度
    /// </summary>
    [SerializeField] private float _width = 10f;

    /// <summary>
    /// 高度
    /// </summary>
    [SerializeField] private float _height = 10f;

    #region 公共方法
    public override bool IsInside(Vector3 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;
        return Mathf.Abs(local.x) <= halfWidth && Mathf.Abs(local.z) <= halfHeight;
    }

    public override bool IsOnBorder(Vector3 worldPos, float tolerance)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;

        bool onHorizontalEdge = Mathf.Abs(Mathf.Abs(local.z) - halfHeight) <= tolerance &&
                                Mathf.Abs(local.x) <= halfWidth;

        bool onVerticalEdge = Mathf.Abs(Mathf.Abs(local.x) - halfWidth) <= tolerance &&
                             Mathf.Abs(local.z) <= halfHeight;

        return onHorizontalEdge || onVerticalEdge;
    }

    public override float GetBoundingRadius()
    {
        return Mathf.Sqrt(_width * _width + _height * _height) * 0.5f;
    }

    public override Vector3 GetCenter()
    {
        return transform.position;
    }

    public override void GetOutlinePoints(List<Vector3> outPoints)
    {
        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;

        outPoints.Add(transform.TransformPoint(new Vector3(-halfWidth, 0, halfHeight)));
        outPoints.Add(transform.TransformPoint(new Vector3(halfWidth, 0, halfHeight)));
        outPoints.Add(transform.TransformPoint(new Vector3(halfWidth, 0, -halfHeight)));
        outPoints.Add(transform.TransformPoint(new Vector3(-halfWidth, 0, -halfHeight)));
    }

    public override Vector3 GetRandomPoint(int maxAttempts = 100)
    {
        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;
        Vector3 local = new Vector3(
            Random.Range(-halfWidth, halfWidth),
            0,
            Random.Range(-halfHeight, halfHeight));
        return transform.TransformPoint(local);
    }
    #endregion

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        Gizmos.color = _borderColor;

        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;

        Vector3 topLeft = transform.TransformPoint(new Vector3(-halfWidth, 0, halfHeight));
        Vector3 topRight = transform.TransformPoint(new Vector3(halfWidth, 0, halfHeight));
        Vector3 bottomLeft = transform.TransformPoint(new Vector3(-halfWidth, 0, -halfHeight));
        Vector3 bottomRight = transform.TransformPoint(new Vector3(halfWidth, 0, -halfHeight));

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        base.OnDrawGizmos();
    }
#endif
}
