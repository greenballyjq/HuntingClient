using UnityEngine;
using Hunting.Game.Animal;

/// <summary>
/// 线性移动组件测试类
/// </summary>
public class LinearMovementTest : MonoBehaviour
{
    /// <summary>
    /// 测试对象
    /// </summary>
    private GameObject _testObject;
    
    /// <summary>
    /// 移动组件
    /// </summary>
    private LinearMovement _movement;
    
    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector3 _initialPosition;
    
    /// <summary>
    /// 测试速度
    /// </summary>
    private float _testSpeed = 5f;
    
    /// <summary>
    /// 测试方向
    /// </summary>
    private Vector3 _testDirection = Vector3.right;

    private void Start()
    {
        Debug.Log("[LinearMovementTest] 开始测试");
        CreateTestObject();
    }

    private void Update()
    {
        // 空格键：开始/停止移动
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleMovement();
        }
        
        // R键：重置位置
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
        
        // 1键：测试基本移动
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestBasicMovement();
        }
        
        // 2键：测试速度设置
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestSpeedSetting();
        }
        
        // 3键：测试方向设置
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestDirectionSetting();
        }
        
        // 4键：测试速率倍数
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestMoveRate();
        }
        
        // 5键：测试停止移动
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestStopMovement();
        }
        
        // 更新移动
        if (_movement != null)
        {
            _movement.DoUpdate(Time.deltaTime);
        }
    }

    /// <summary>
    /// 创建测试对象
    /// </summary>
    private void CreateTestObject()
    {
        _testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _testObject.name = "LinearMovementTestObject";
        _testObject.transform.position = Vector3.zero;
        _initialPosition = Vector3.zero;
        
        _movement = _testObject.AddComponent<LinearMovement>();
        
        Debug.Log("[LinearMovementTest] 测试对象已创建");
    }

    /// <summary>
    /// 切换移动状态
    /// </summary>
    private void ToggleMovement()
    {
        if (_movement == null) return;
        
        if (_movement.IsMoving)
        {
            _movement.StopMove();
            Debug.Log("[LinearMovementTest] 停止移动");
        }
        else
        {
            _movement.StartMove();
            Debug.Log("[LinearMovementTest] 开始移动");
        }
    }

    /// <summary>
    /// 重置位置
    /// </summary>
    private void ResetPosition()
    {
        if (_testObject != null)
        {
            _testObject.transform.position = _initialPosition;
            Debug.Log($"[LinearMovementTest] 位置已重置到: {_initialPosition}");
        }
    }

    /// <summary>
    /// 测试基本移动
    /// </summary>
    private void TestBasicMovement()
    {
        Debug.Log("[LinearMovementTest] ===== 测试基本移动 =====");
        
        _testObject.transform.position = Vector3.zero;
        _movement.SetSpeed(_testSpeed);
        _movement.SetDirection(_testDirection);
        _movement.StartMove();
        
        Debug.Log($"[LinearMovementTest] 速度: {_testSpeed}, 方向: {_testDirection}");
        Debug.Log($"[LinearMovementTest] 当前速度: {_movement.CurrentSpeed}");
        Debug.Log($"[LinearMovementTest] 是否移动: {_movement.IsMoving}");
    }

    /// <summary>
    /// 测试速度设置
    /// </summary>
    private void TestSpeedSetting()
    {
        Debug.Log("[LinearMovementTest] ===== 测试速度设置 =====");
        
        float[] speeds = { 2f, 5f, 10f };
        foreach (float speed in speeds)
        {
            _movement.SetSpeed(speed);
            Debug.Log($"[LinearMovementTest] 设置速度: {speed}, 当前速度: {_movement.CurrentSpeed}");
        }
    }

    /// <summary>
    /// 测试方向设置
    /// </summary>
    private void TestDirectionSetting()
    {
        Debug.Log("[LinearMovementTest] ===== 测试方向设置 =====");
        
        Vector3[] directions = 
        {
            Vector3.right,
            Vector3.forward,
            new Vector3(1, 0, 1),
            Vector3.left
        };
        
        foreach (Vector3 dir in directions)
        {
            _movement.SetDirection(dir);
        }
    }

    /// <summary>
    /// 测试速率倍数
    /// </summary>
    private void TestMoveRate()
    {
        Debug.Log("[LinearMovementTest] ===== 测试速率倍数 =====");
        
        _movement.SetSpeed(5f);
        Debug.Log($"[LinearMovementTest] 基础速度: 5, 速率倍数: 1, 当前速度: {_movement.CurrentSpeed}");
        
        _movement.SetMoveRate(2f);
        Debug.Log($"[LinearMovementTest] 基础速度: 5, 速率倍数: 2, 当前速度: {_movement.CurrentSpeed}");
        
        _movement.SetMoveRate(0.5f);
        Debug.Log($"[LinearMovementTest] 基础速度: 5, 速率倍数: 0.5, 当前速度: {_movement.CurrentSpeed}");
        
        _movement.SetMoveRate(1f);
    }

    /// <summary>
    /// 测试停止移动
    /// </summary>
    private void TestStopMovement()
    {
        Debug.Log("[LinearMovementTest] ===== 测试停止移动 =====");
        
        _movement.StartMove();
        Debug.Log($"[LinearMovementTest] 开始移动后 - 是否移动: {_movement.IsMoving}, 当前速度: {_movement.CurrentSpeed}");
        
        _movement.StopMove();
        Debug.Log($"[LinearMovementTest] 停止移动后 - 是否移动: {_movement.IsMoving}, 当前速度: {_movement.CurrentSpeed}");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label("LinearMovement 测试控制");
        GUILayout.Label("空格键: 开始/停止移动");
        GUILayout.Label("R键: 重置位置");
        GUILayout.Label("1键: 测试基本移动");
        GUILayout.Label("2键: 测试速度设置");
        GUILayout.Label("3键: 测试方向设置");
        GUILayout.Label("4键: 测试速率倍数");
        GUILayout.Label("5键: 测试停止移动");
        
        if (_movement != null)
        {
            GUILayout.Space(10);
            GUILayout.Label($"当前速度: {_movement.CurrentSpeed:F2}");
            GUILayout.Label($"是否移动: {_movement.IsMoving}");
            GUILayout.Label($"位置: {_testObject.transform.position}");
        }
        
        GUILayout.EndArea();
    }
}
