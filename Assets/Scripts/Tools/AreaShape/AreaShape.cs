using UnityEngine;

/// <summary>
/// 区域形状基类
/// </summary>
public abstract class AreaShape : MonoBehaviour
{
    /// <summary>
    /// 边界颜色
    /// </summary>
    [SerializeField] private Color _borderColor = Color.green;
    protected Color BorderColor => _borderColor;

    /// <summary>
    /// 是否在游戏视图显示
    /// </summary>
    [SerializeField] private bool _showInGame = true;

    /// <summary>
    /// 线条宽度
    /// </summary>
    [SerializeField] private float _lineWidth = 0.1f;

    /// <summary>
    /// 线条渲染组件
    /// </summary>
    private LineRenderer _lineRenderer;

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
    /// 获取包围半径
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
        
        // 在包围圆内随机采样，直到找到形状内的点
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

        // 尝试失败返回中心点
        return center;
    }

    /// <summary>
    /// 获取形状点集
    /// </summary>
    /// <returns>形状轮廓的点集</returns>
    protected abstract Vector3[] GetShapePoints();

    protected virtual void Awake()
    {
        if (_showInGame)
        {       
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = _borderColor;
            _lineRenderer.endColor = _borderColor;
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = true;
        }
    }

    private void LateUpdate()
    {
        if (_showInGame && _lineRenderer != null)
        {
            Vector3[] points = GetShapePoints();
            if (points != null && points.Length >= 2)
            {
                _lineRenderer.positionCount = points.Length;
                _lineRenderer.SetPositions(points);
                _lineRenderer.startColor = _borderColor;
                _lineRenderer.endColor = _borderColor;
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器可视化绘制
    /// </summary>
    public virtual void OnDrawGizmos() {}
#endif
}

