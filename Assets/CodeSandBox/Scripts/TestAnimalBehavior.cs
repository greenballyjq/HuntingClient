using UnityEngine;
using cfg.HuntingConfig;

/// <summary>
/// 测试用简化动物类
/// 只保留：在场上跑、进入逃跑、消失
/// 去掉所有事件调用和复杂状态机
/// </summary>
public class TestAnimalBehavior : MonoBehaviour
{
    /// <summary>
    /// 动物状态枚举
    /// </summary>
    private enum AnimalState
    {
        Move,      // 移动状态
        Flee,      // 逃跑状态
        Destroyed  // 已销毁
    }

    /// <summary>
    /// 配置数据（通过Init方法设置）
    /// </summary>
    private Specie _specieData;
    private float _stayTime;

    /// <summary>
    /// 运行时数据
    /// </summary>
    private float _currentHP;
    private float _timeInScene;
    private AnimalState _currentState;

    /// <summary>
    /// 当前移动速度
    /// </summary>
    private float _currentMoveSpeed;

    /// <summary>
    /// 当前移动方向
    /// </summary>
    private Vector3 _currentDirection;

    /// <summary>
    /// 逃跑持续时间
    /// </summary>
    private float _fleeDuration = 30f;

    /// <summary>
    /// 逃跑剩余时间
    /// </summary>
    private float _fleeTimeRemaining;

    /// <summary>
    /// 逃跑加速倍率
    /// </summary>
    private float _fleeSpeedMultiplier = 2f;

    /// <summary>
    /// RVO避障移动组件
    /// </summary>
    private RVOMovement _rvo;

    /// <summary>
    /// 配置数据（只读）
    /// </summary>
    public Specie SpecieData => _specieData;

    /// <summary>
    /// 最大驻场时间（只读）
    /// </summary>
    public float StayTime => _stayTime;

    /// <summary>
    /// 在场景中的时间（只读）
    /// </summary>
    public float TimeInScene => _timeInScene;

    /// <summary>
    /// 初始化动物
    /// </summary>
    /// <param name="data">物种配置数据</param>
    /// <param name="stayTime">驻场时间</param>
    public void Init(Specie data, float stayTime)
    {
        _specieData = data;
        _stayTime = stayTime;
        _currentHP = data.HP;
        _currentMoveSpeed = 1;
        _currentDirection = transform.forward;
        _timeInScene = 0f;
        _currentState = AnimalState.Move;
        _fleeTimeRemaining = 0f;

        // 初始化RVO
        if (_rvo != null)
        {
            _rvo.SetMoveDirection(_currentDirection);
            _rvo.SetMaxSpeed(_currentMoveSpeed);
            _rvo.SyncPosition();
        }
    }

    private void Awake()
    {
        // 获取RVO组件
        _rvo = GetComponent<RVOMovement>();
    }

    private void Update()
    {
        if (_currentState == AnimalState.Destroyed)
            return;

        // 更新在场景中的时间
        _timeInScene += Time.deltaTime;

        // 根据状态执行逻辑
        switch (_currentState)
        {
            case AnimalState.Move:
                UpdateMoveState();
                break;

            case AnimalState.Flee:
                UpdateFleeState();
                break;
        }
    }

    /// <summary>
    /// 更新移动状态
    /// </summary>
    private void UpdateMoveState()
    {
        // 检查是否到达驻场时间，进入逃跑
        if (_timeInScene >= _stayTime)
        {
            EnterFleeState();
        }
    }

    /// <summary>
    /// 更新逃跑状态
    /// </summary>
    private void UpdateFleeState()
    {
        // 逃跑时间倒计时
        _fleeTimeRemaining -= Time.deltaTime;

        // 逃跑时间到，销毁动物
        if (_fleeTimeRemaining <= 0f)
        {
            DestroyAnimal();
        }
    }

    /// <summary>
    /// 进入逃跑状态
    /// </summary>
    private void EnterFleeState()
    {
        _currentState = AnimalState.Flee;
        _fleeTimeRemaining = _fleeDuration;

        // 应用逃跑加速倍率
        if (_rvo != null)
        {
            float fleeSpeed = _currentMoveSpeed * _fleeSpeedMultiplier;
            _rvo.SetMaxSpeed(fleeSpeed);
        }
    }

    /// <summary>
    /// 销毁动物
    /// </summary>
    private void DestroyAnimal()
    {
        _currentState = AnimalState.Destroyed;

        // 禁用RVO
        if (_rvo != null)
        {
            _rvo.Disable();
        }

        // 销毁GameObject
        Destroy(gameObject);
    }

    /// <summary>
    /// 设置移动方向（可选，用于外部控制）
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        _currentDirection = direction;
        if (_rvo != null)
        {
            _rvo.SetMoveDirection(direction);
        }
    }
}

