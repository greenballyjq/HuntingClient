using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 区域形状基类
/// </summary>
public abstract class BaseAreaShape : MonoBehaviour
{
    /// <summary>
    /// 边界颜色
    /// </summary>
    [SerializeField] protected Color _borderColor = Color.green;
    public Color BorderColor => _borderColor;

    /// <summary>
    /// 是否显示包围半径
    /// </summary>
    [SerializeField] protected bool _showBoundingRadius = false;

    /// <summary>
    /// 是否在内部
    /// </summary>
    /// <param name="worldPos">世界坐标</param>
    /// <returns>是否在内部</returns>
    public abstract bool IsInside(Vector3 worldPos);

    /// <summary>
    /// 判断是否在边界上
    /// </summary>
    /// <param name="worldPos">世界坐标</param>
    /// <param name="tolerance">容差值</param>
    /// <returns>是否在边界上</returns>
    public abstract bool IsOnBorder(Vector3 worldPos, float tolerance);

    /// <summary>
    /// 获取外接圆半径
    /// </summary>
    public abstract float GetBoundingRadius();

    /// <summary>
    /// 获取中心点
    /// </summary>
    /// <returns>中心点坐标</returns>
    public abstract Vector3 GetCenter();

    /// <summary>
    /// 获取轮廓顶点
    /// </summary>
    /// <param name="outPoints">输出顶点列表</param>
    public abstract void GetOutlinePoints(List<Vector3> outPoints);

    /// <summary>
    /// 获取随机点
    /// </summary>
    /// <param name="maxAttempts">最大尝试次数</param>
    /// <returns>随机点</returns>
    public virtual Vector3 GetRandomPoint(int maxAttempts = 100)
    {
        Vector3 center = GetCenter();
        float boundingRadius = GetBoundingRadius();
        float angle;
        float distance;
        Vector3 randomPoint;
        for (int i = 0; i < maxAttempts; i++)
        {
            angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            distance = Mathf.Sqrt(Random.Range(0f, 1f)) * boundingRadius;
            randomPoint = center + new Vector3(
                Mathf.Cos(angle) * distance,
                0,
                Mathf.Sin(angle) * distance
            );
            if (IsInside(randomPoint))
                return randomPoint;
        }

        return center;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器可视化绘制
    /// </summary>
    public virtual void OnDrawGizmos()
    {
        DrawBoundingRadiusGizmo();
    }

    /// <summary>
    /// 绘制包围半径圆环
    /// </summary>
    protected void DrawBoundingRadiusGizmo()
    {
        if (!_showBoundingRadius) return;

        Vector3 center = GetCenter();
        float radius = GetBoundingRadius();

        Gizmos.color = Color.cyan;
        const int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 currentPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }
#endif
}
