using RVO;
using System.Collections.Generic;
using UnityEngine;

public class RVO2Manager : MonoBehaviour
{
    private List<RVO2Agent> agents = new List<RVO2Agent>();

    void Start()
    {
        Simulator.Instance.setTimeStep(0.25f);

        foreach (RVO2Agent agent in FindObjectsOfType<RVO2Agent>())
        {
            agents.Add(agent);
        }

        Debug.Log($"找到 {agents.Count} 个Agent");
    }
    
    void Update()
    {
        if (agents.Count == 0) return;

        // 1. 所有agent设置prefVelocity
        foreach (var agent in agents)
        {
            agent.UpdateRVO();
        }

        // 2. 执行RVO计算
        Simulator.Instance.doStep();

        // 3. 所有agent同步位置
        foreach (var agent in agents)
        {
            agent.SyncPosition();
        }
    }

    void OnDestroy()
    {
        Simulator.Instance.Clear();
    }
}