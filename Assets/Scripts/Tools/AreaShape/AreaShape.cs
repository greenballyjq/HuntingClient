using UnityEngine;

/// <summary>
/// 区域形状基类
/// </summary>
public abstract class AreaShape : MonoBehaviour
{
    /// <summary>
    /// 边界颜色
    /// </summary>
    [SerializeField] protected Color _borderColor = Color.green;

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
    /// 获取随机点
    /// </summary>
    /// <param name="maxAttempts">最大尝试次数</param>
    /// <returns>随机点</returns>
    public virtual Vector3 GetRandomPoint(int maxAttempts = 100)
    {
        Vector3 center = GetCenter();
        float boundingRadius = GetBoundingRadius();
        
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * boundingRadius;
            Vector3 randomPoint = center + new Vector3(
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
    public virtual void OnDrawGizmos() {}
#endif
}
