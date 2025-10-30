using UnityEngine;


/// <summary>
/// 矩形障碍组件
/// </summary>
public class RVOObstacleRect : MonoBehaviour
{
    [Header("矩形尺寸 (X=宽, Y=高[Z方向])")]
    public Vector2 size = new Vector2(10f, 2f);

    [Header("顶点顺序 (true=逆时针=普通障碍)")]
    public bool ccw = true;

    [Header("运行时设置")]
    [Tooltip("运行时自动放置该障碍")] public bool autoPlaceOnRun = true;
    [Tooltip("放置后立即烘焙（使之生效）")] public bool bakeImmediately = true;

    /// <summary>
    /// 是否已注册
    /// </summary>
    private bool _registered = false;

    /// <summary>
    /// RVO管理器
    /// </summary>
    private RVOManager _rvoManager => RVOManager.Instance;

    private void Start()
    {
        if (Application.isPlaying && autoPlaceOnRun)
        {
            PlaceAndBake();
        }
    }

    /// <summary>
    /// 放置障碍
    /// </summary>
    public void Place()
    {
        int idx = _rvoManager.RegisterObstacleRect(transform.position, size, ccw, owner: this);
        _registered = (idx >= 0);
    }

    /// <summary>
    /// 放置并按需立刻烘焙
    /// </summary>
    public void PlaceAndBake()
    {
        Place();
        if (_registered && bakeImmediately)
            _rvoManager.ProcessAllObstacles();
    }

    private void OnDrawGizmos()
    {
        // 可视化矩形区域
        Gizmos.color = new Color(1f, 0f, 0f, 0.7f);

        float hx = size.x * 0.5f;
        float hz = size.y * 0.5f;
        Vector3 c = transform.position;

        Vector3 p0 = new Vector3(c.x - hx, c.y, c.z - hz);
        Vector3 p1 = new Vector3(c.x + hx, c.y, c.z - hz);
        Vector3 p2 = new Vector3(c.x + hx, c.y, c.z + hz);
        Vector3 p3 = new Vector3(c.x - hx, c.y, c.z + hz);

        // 线框
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);

        // 顺序指示（画一小箭头）
        Vector3 mid = (p0 + p1) * 0.5f;
        Vector3 dir = (ccw ? (p1 - p0) : (p0 - p1)).normalized;
        Gizmos.DrawLine(mid, mid + dir * 0.8f);
    }
}


