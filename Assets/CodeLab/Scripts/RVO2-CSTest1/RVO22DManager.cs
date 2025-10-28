using UnityEngine;
using RVO;

public class RVO22DManager : MonoBehaviour
{
    private System.Collections.Generic.List<RVO22DAgent> agents = new System.Collections.Generic.List<RVO22DAgent>();

    void Start()
    {
        Simulator.Instance.setTimeStep(0.25f);

        foreach (RVO22DAgent agent in FindObjectsOfType<RVO22DAgent>())
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