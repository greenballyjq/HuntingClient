using UnityEngine;
using RVO;  // RVO2-CS库的命名空间

public class SimpleRVOExample : MonoBehaviour
{
    // ====== 场景对象 ======
    public GameObject agentPrefab;  // 代理的预制体（可以是简单的Cube或Sphere）

    // ====== 内部数据 ======
    private GameObject[] agentObjects;  // Unity游戏对象数组
    private Vector3[] goalPositions;    // 每个代理的目标位置
    private int agentCount = 1000;         // 代理数量

    void Start()
    {
        // ==================== 第1步：获取模拟器单例 ====================
        // RVO2使用单例模式，全局只有一个Simulator实例
        Simulator sim = Simulator.Instance;

        // ==================== 第2步：清空并初始化模拟器 ====================
        // 清除之前可能存在的所有代理和障碍物
        sim.Clear();

        // ==================== 第3步：设置代理的默认参数 ====================
        // 之后添加的代理如果不指定参数，会使用这些默认值
        sim.setAgentDefaults(
            neighborDist: 15f,      // 邻居检测距离：代理会检测15米内的其他代理
            maxNeighbors: 10,       // 最多考虑10个邻居（性能优化）
            timeHorizon: 10f,       // 时间视界：预测未来10秒内与其他代理的碰撞
            timeHorizonObst: 10f,   // 障碍物时间视界：预测未来10秒内与障碍物的碰撞
            radius: 0.5f,           // 代理半径：0.5米（碰撞检测用）
            maxSpeed: 0.2f,           // 最大速度：0.2米/秒
            velocity: new RVO2Vector2(0, 0)  // 初始速度：静止状态
        );

        // ==================== 第4步：设置模拟时间步长 ====================
        // 每次调用doStep()时，模拟会前进的时间（通常设置为固定值）
        sim.setTimeStep(0.25f);  // 0.25秒

        // ==================== 第5步：创建Unity对象和RVO代理 ====================
        agentObjects = new GameObject[agentCount];
        goalPositions = new Vector3[agentCount];

        for (int i = 0; i < agentCount; i++)
        {
            // 5.1 创建Unity游戏对象（用于显示）
            Vector3 startPos = new Vector3(
                Random.Range(-100f, 100f),  // 随机X位置
                0f,                        // Y固定在0（2D平面）
                Random.Range(-100f, 100f)   // 随机Z位置
            );
            agentObjects[i] = Instantiate(agentPrefab, startPos, Quaternion.identity);
            agentObjects[i].name = $"Agent_{i}";

            // 5.2 在RVO模拟器中添加对应的代理
            // 注意：RVO2使用的是2D向量，我们用Unity的X和Z作为RVO的X和Y
            RVO2Vector2 rvoStartPos = new RVO2Vector2(startPos.x, startPos.z);
            int agentId = sim.addAgent(rvoStartPos);
            // agentId应该等于i（按顺序添加，ID从0开始）

            // 5.3 为每个代理设置一个随机目标位置
            goalPositions[i] = new Vector3(
                Random.Range(-100f, 100f),
                0f,
                Random.Range(-100f, 100f)
            );

            Debug.Log($"代理{i}：起点{startPos}，目标{goalPositions[i]}");
        }
    }

    void Update()
    {
        Debug.Log("asd");
        // ==================== 第6步：获取模拟器引用 ====================
        Simulator sim = Simulator.Instance;

        // ==================== 第7步：为每个代理设置期望速度 ====================
        // 这是每帧都要做的：告诉RVO"我想往哪个方向走"
        for (int i = 0; i < sim.getNumAgents(); i++)
        {
            // 7.1 获取代理当前位置（RVO坐标系）
            RVO2Vector2 currentPos = sim.getAgentPosition(i);

            // 7.2 转换为Unity坐标并计算到目标的向量
            Vector3 currentPos3D = new Vector3(currentPos.x(), 0f, currentPos.y());
            Vector3 toGoal = goalPositions[i] - currentPos3D;

            // 7.3 如果已经接近目标，就停下来
            if (toGoal.magnitude < 0.5f)
            {
                // 期望速度设为0（停止）
                sim.setAgentPrefVelocity(i, new RVO2Vector2(0, 0));
                continue;
            }

            // 7.4 计算期望速度：朝向目标，速度为最大速度
            Vector3 desiredVelocity3D = toGoal.normalized * sim.getAgentMaxSpeed(i);

            // 7.5 转换为RVO2Vector2并设置
            RVO2Vector2 prefVelocity = new RVO2Vector2(
                desiredVelocity3D.x,
                desiredVelocity3D.z
            );
            sim.setAgentPrefVelocity(i, prefVelocity);
        }

        // ==================== 第8步：执行一步模拟 ====================
        // 这是核心！RVO会：
        // 1. 为每个代理寻找附近的邻居
        // 2. 根据ORCA算法计算避障速度
        // 3. 更新每个代理的位置和速度
        sim.doStep();

        // ==================== 第9步：从RVO获取结果并应用到Unity对象 ====================
        for (int i = 0; i < sim.getNumAgents(); i++)
        {
            // 9.1 获取RVO计算后的位置
            RVO2Vector2 rvoPos = sim.getAgentPosition(i);

            // 9.2 转换为Unity坐标
            Vector3 newPos = new Vector3(rvoPos.x(), 0f, rvoPos.y());

            // 9.3 更新Unity对象位置
            agentObjects[i].transform.position = newPos;

            // 9.4（可选）获取速度并调整朝向
            RVO2Vector2 rvoVel = sim.getAgentVelocity(i);
            Vector3 velocity3D = new Vector3(rvoVel.x(), 0f, rvoVel.y());

            // 如果在移动，让对象朝向移动方向
            if (velocity3D.magnitude > 0.01f)
            {
                agentObjects[i].transform.forward = velocity3D.normalized;
            }
        }
    }

    // ==================== 辅助方法：在场景中绘制调试信息 ====================
    void OnDrawGizmos()
    {
        if (goalPositions == null || agentObjects == null) return;

        // 为每个代理绘制目标点和连线
        for (int i = 0; i < agentCount; i++)
        {
            if (agentObjects[i] != null)
            {
                // 目标点（绿色球）
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(goalPositions[i], 0.5f);

                // 当前位置到目标的连线（黄色）
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(agentObjects[i].transform.position, goalPositions[i]);
            }
        }
    }
}