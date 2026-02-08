using UnityEngine;

/// <summary>
/// 圆形区域形状
/// </summary>
public class AreaShapeCircle : AreaShape
{
    /// <summary>
    /// 半径
    /// </summary>
    [SerializeField] private float _radius = 5f;

    public override bool IsInside(Vector3 worldPos)
    {
        Vector2 center2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);
        float distance = Vector2.Distance(center2D, worldPos2D);
        return distance <= _radius;
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

    protected override Vector3[] GetShapePoints()
    {
        Vector3 center = transform.position;
        int segments = 32;
        Vector3[] points = new Vector3[segments + 1];
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            points[i] = center + new Vector3(
                Mathf.Cos(angle) * _radius,
                0,
                Mathf.Sin(angle) * _radius
            );
        }

        return points;
    }
    
#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        // 绘制圆形轮廓
        Gizmos.color = BorderColor;
        Vector3 center = transform.position;

        int segments = 32;
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
        
        // 绘制中心点
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, 0.2f);
    }
#endif
}

