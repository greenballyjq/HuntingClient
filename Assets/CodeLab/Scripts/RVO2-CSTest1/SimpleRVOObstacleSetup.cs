using System.Collections.Generic;
using UnityEngine;
using RVO;

/// <summary>
/// 仅用于在 RVO2-CS 中注册静态障碍，并在场景中可视化；不创建任何 agent。
/// </summary>
public class SimpleRVOObstacleSetup : MonoBehaviour
{
    [Header("是否在Start中自动创建示例障碍")]
    public bool createDemoObstacles = true;

    // 本地缓存的障碍顶点，用于Gizmos可视化（Unity坐标：x,z）
    private readonly List<List<Vector3>> _obstaclePolygons = new List<List<Vector3>>();

    void Start()
    {
        // 1) 获取并初始化模拟器（只做必要初始化；不添加agent）
        var sim = Simulator.Instance;
        sim.Clear();              // 清空之前的代理与障碍
        sim.setTimeStep(0.1f);    // 障碍本身不需要步进，但保持默认即可
        sim.SetNumWorkers(0);     // 与障碍无关，仅保持一致

        if (createDemoObstacles)
        {
            // 2) 添加一个“中间大矩形墙体”（逆时针顺序 = 普通障碍/实体墙）
            AddRectangleObstacle(new Vector3(0f, 0f, 0f), new Vector2(100f, 20f), ccw: true);

            // 3) 添加一个“小方块”
            AddRectangleObstacle(new Vector3(20f, 0f, -20f), new Vector2(20f, 20f), ccw: true);
        }

        // 5) 处理障碍，使其纳入 KD 树并生效
        sim.processObstacles();

        // 提示：如果之后还要动态再添加障碍，需要再次调用 processObstacles()
    }

    /// <summary>
    /// 添加一个矩形障碍（中心+尺寸）；ccw=true 逆时针（普通障碍），ccw=false 顺时针（负障碍/外壳）。
    /// </summary>
    public void AddRectangleObstacle(Vector3 center, Vector2 size, bool ccw = true)
    {
        float hx = size.x * 0.5f;
        float hz = size.y * 0.5f;

        // 以Unity的x,z作为RVO的x,y
        var p0 = new Vector3(center.x - hx, 0f, center.z - hz);
        var p1 = new Vector3(center.x + hx, 0f, center.z - hz);
        var p2 = new Vector3(center.x + hx, 0f, center.z + hz);
        var p3 = new Vector3(center.x - hx, 0f, center.z + hz);

        var vertsUnity = ccw ? new List<Vector3> { p0, p1, p2, p3 } : new List<Vector3> { p0, p3, p2, p1 };
        _obstaclePolygons.Add(vertsUnity);

        var vertsRVO = new List<RVOVector2>(4);
        foreach (var v in vertsUnity)
            vertsRVO.Add(new RVOVector2(v.x, v.z));

        Simulator.Instance.addObstacle(vertsRVO);
    }

    /// <summary>
    /// 添加任意多边形障碍（顶点请按逆时针=普通障碍；顺时针=负障碍）。
    /// </summary>
    public void AddPolygonObstacle(IList<Vector3> unityVerts, bool ccw = true)
    {
        if (unityVerts == null || unityVerts.Count < 2) return;

        List<Vector3> vertsUnity = new List<Vector3>(unityVerts);
        if (!ccw) vertsUnity.Reverse();

        _obstaclePolygons.Add(vertsUnity);

        var vertsRVO = new List<RVOVector2>(vertsUnity.Count);
        foreach (var v in vertsUnity)
            vertsRVO.Add(new RVOVector2(v.x, v.z));

        Simulator.Instance.addObstacle(vertsRVO);
    }

    void OnDrawGizmos()
    {
        // 在编辑器/运行时可视化已注册的障碍多边形
        Gizmos.color = Color.red;
        foreach (var poly in _obstaclePolygons)
        {
            for (int i = 0; i < poly.Count; i++)
            {
                var a = poly[i];
                var b = poly[(i + 1) % poly.Count];
                Gizmos.DrawLine(a, b);
            }
        }
    }
}