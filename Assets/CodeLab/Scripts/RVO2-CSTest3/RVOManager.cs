using UnityEngine;
using RVO;

/// <summary>
/// RVO模拟器的Unity包装，负责统一管理所有避障代理
/// </summary>
public class RVOManager : MonoBehaviour
{
    private static RVOManager _instance;
    public static RVOManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("RVOManager");
                _instance = go.AddComponent<RVOManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private Simulator sim;
    private bool hasSimulatedThisFrame = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // 初始化RVO模拟器
        sim = Simulator.Instance;
        sim.Clear();

        // 设置默认参数
        sim.setAgentDefaults(
            neighborDist: 15f,
            maxNeighbors: 10,
            timeHorizon: 2f,      // 建议改小，避免过早反应
            timeHorizonObst: 2f,
            radius: 0.5f,
            maxSpeed: 10f,        // 设大一点，不限制用户的移动速度
            velocity: new RVO2Vector2(0, 0)
        );

        // 时间步长会在ExecuteSimulation中动态设置
    }

    void LateUpdate()
    {
        // 每帧结束后重置标记
        hasSimulatedThisFrame = false;
    }

    /// <summary>
    /// 执行一次RVO模拟（每帧只执行一次）
    /// </summary>
    public void ExecuteSimulation(float deltaTime)
    {
        if (hasSimulatedThisFrame) return;

        // 设置时间步长为当前帧的deltaTime
        sim.setTimeStep(deltaTime);

        // 执行模拟
        sim.doStep();

        hasSimulatedThisFrame = true;
    }

    public Simulator GetSimulator()
    {
        return sim;
    }
}