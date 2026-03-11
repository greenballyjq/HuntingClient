using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 圆形区域形状
/// </summary>
public class AreaShapeCircle : BaseAreaShape
{
    /// <summary>
    /// 半径
    /// </summary>
    [SerializeField] private float _radius = 5f;

    #region 公共方法
    public override bool IsInside(Vector3 worldPos)
    {
        Vector2 center2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);
        float sqrDistance = (center2D - worldPos2D).sqrMagnitude;
        return sqrDistance <= _radius * _radius;
    }

    public override bool IsOnBorder(Vector3 worldPos, float tolerance)
    {
        Vector2 center2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);
        float distance = Vector2.Distance(center2D, worldPos2D);
        return Mathf.Abs(distance - _radius) <= tolerance;
    }

    public override float GetBoundingRadius()
    {
        return _radius;
    }

    public override Vector3 GetCenter()
    {
        return transform.position;
    }

    public override void GetOutlinePoints(List<Vector3> outPoints)
    {
        Vector3 center = transform.position;
        const int segments = 32;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            outPoints.Add(center + new Vector3(Mathf.Cos(angle) * _radius, 0, Mathf.Sin(angle) * _radius));
        }
    }

    public override Vector3 GetRandomPoint(int maxAttempts = 100)
    {
        Vector3 center = transform.position;
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * _radius;
        
        return center + new Vector3(
            Mathf.Cos(angle) * distance,
            0,
            Mathf.Sin(angle) * distance
        );
    }
    #endregion

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        Gizmos.color = _borderColor;
        Vector3 center = transform.position;

        const int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(_radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 currentPoint = center + new Vector3(
                Mathf.Cos(angle) * _radius,
                0,
                Mathf.Sin(angle) * _radius
            );
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, 0.15f);

        base.OnDrawGizmos();
    }
#endif
}

