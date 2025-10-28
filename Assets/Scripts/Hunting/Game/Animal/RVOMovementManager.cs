using UnityEngine;
using RVO;
using System.Collections.Generic;

/// <summary>
/// RVO避障移动管理器
/// </summary>
public class RVOMovementManager : MonoBehaviour
{
    private static RVOMovementManager _instance;
    public static RVOMovementManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("RVOMovementManager");
                _instance = go.AddComponent<RVOMovementManager>();
            }
            return _instance;
        }
    }

    private Simulator sim;
    private List<RVOMovement> movements = new List<RVOMovement>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        sim = Simulator.Instance;
        sim.Clear();
        // 不在这里设置timeStep，每帧动态设置
    }

    void Update()
    {
        if (movements.Count == 0) return;

        // 1. 所有代理设置位置和期望速度
        foreach (var movement in movements)
        {
            movement.UpdateRVO();
        }

        // 2. 执行RVO计算 - 使用实际的deltaTime（关键修复！）
        sim.setTimeStep(Time.deltaTime);
        sim.doStep();

        // 3. 所有代理同步位置
        foreach (var movement in movements)
        {
            movement.SyncPosition();
        }
    }

    public void Register(RVOMovement movement)
    {
        if (!movements.Contains(movement))
        {
            movements.Add(movement);
        }
    }

    public void Unregister(RVOMovement movement)
    {
        movements.Remove(movement);
    }

    public Simulator GetSimulator()
    {
        return sim;
    }

    void OnDestroy()
    {
        sim.Clear();
    }
}