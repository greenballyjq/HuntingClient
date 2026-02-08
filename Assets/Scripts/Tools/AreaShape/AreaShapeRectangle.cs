using UnityEngine;

/// <summary>
/// 矩形区域形状
/// </summary>
public class AreaShapeRectangle : AreaShape
{
    /// <summary>
    /// 宽度
    /// </summary>
    [SerializeField] private float _width = 10f;

    /// <summary>
    /// 高度
    /// </summary>
    [SerializeField] private float _height = 10f;

    public override bool IsInside(Vector3 worldPos)
    {
        Vector2 center2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);
        Vector2 offset = worldPos2D - center2D;

        return Mathf.Abs(offset.x) <= _width * 0.5f && Mathf.Abs(offset.y) <= _height * 0.5f;
    }

    public override bool IsOnBorder(Vector3 worldPos, float tolerance)
    {
        Vector2 center2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.z);
        Vector2 offset = worldPos2D - center2D;

        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;

        bool onHorizontalEdge = Mathf.Abs(offset.y) <= halfHeight + tolerance &&
                                 Mathf.Abs(offset.y) >= halfHeight - tolerance &&
                                 Mathf.Abs(offset.x) <= halfWidth;
        
        bool onVerticalEdge = Mathf.Abs(offset.x) <= halfWidth + tolerance &&
                              Mathf.Abs(offset.x) >= halfWidth - tolerance &&
                              Mathf.Abs(offset.y) <= halfHeight;

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
    
    public override Vector3 GetRandomPoint(int maxAttempts = 100)
    {
        Vector3 center = transform.position;
        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;
        
        return center + new Vector3(
            Random.Range(-halfWidth, halfWidth),
            0,
            Random.Range(-halfHeight, halfHeight)
        );
    }

    protected override Vector3[] GetShapePoints()
    {
        Vector3 center = transform.position;
        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;

        return new Vector3[]
        {
            center + new Vector3(-halfWidth, 0, halfHeight),
            center + new Vector3(halfWidth, 0, halfHeight),
            center + new Vector3(halfWidth, 0, -halfHeight),
            center + new Vector3(-halfWidth, 0, -halfHeight),
            center + new Vector3(-halfWidth, 0, halfHeight)
        };
    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        // 绘制矩形四条边
        Gizmos.color = BorderColor;
        Vector3 center = transform.position;

        float halfWidth = _width * 0.5f;
        float halfHeight = _height * 0.5f;

        Vector3 topLeft = center + new Vector3(-halfWidth, 0, halfHeight);
        Vector3 topRight = center + new Vector3(halfWidth, 0, halfHeight);
        Vector3 bottomLeft = center + new Vector3(-halfWidth, 0, -halfHeight);
        Vector3 bottomRight = center + new Vector3(halfWidth, 0, -halfHeight);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // 绘制中心点
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, 0.2f);
    }
#endif
}

