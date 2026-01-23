using cfg.HuntingConfig.Enum;
using GameFramework.Core.Pool;
using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 陷阱行为组件
/// </summary>
public class TrapBehavior : MonoBehaviour, IPoolItem
{
    /// <summary>
    /// 吸引间隔（秒）
    /// </summary>
    [Tooltip("吸引间隔（秒）")]
    [SerializeField]
    private float _attractionInterval = 0.1f;

    /// <summary>
    /// 触发检测间隔（秒）
    /// </summary>
    [Tooltip("触发检测间隔（秒）")]
    [SerializeField]
    private float _triggerCheckInterval = 0.1f;

    /// <summary>
    /// 吸引半径
    /// </summary>
    private float _attractRadius;

    /// <summary>
    /// 触发半径
    /// </summary>
    private float _triggerRadius;

    /// <summary>
    /// 吸引计时器
    /// </summary>
    private float _attractionCheckTimer;

    /// <summary>
    /// 触发检测计时器
    /// </summary>
    private float _triggerCheckTimer;

    /// <summary>
    /// 按体型划分的吸引半径范围比例
    /// </summary>
    private Dictionary<ESpecieType, float[]> _attractRadiusRangeByVolume;

    /// <summary>
    /// 已吸引的动物集合
    /// </summary>
    private HashSet<BaseAnimalBehaviour> _attractedAnimals = new HashSet<BaseAnimalBehaviour>();

    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    #region 对象池接口
    public string PrefabPath { get; set; }

    public void OnSpawned()
    {
        gameObject.SetActive(true);
    }

    public void OnDespawned()
    {
        // 清理运行时状态
        _attractionCheckTimer = 0f;
        _triggerCheckTimer = 0f;
        _attractedAnimals.Clear();
        IsInitialized = false;

        gameObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 初始化陷阱
    /// </summary>
    /// <param name="attractRadius">吸引半径</param>
    /// <param name="triggerRadius">触发半径</param>
    /// <param name="attractRadiusRangeByVolume">按体型划分的吸引半径范围比例</param>
    public void Init(float attractRadius, float triggerRadius, Dictionary<ESpecieType, float[]> attractRadiusRangeByVolume)
    {
        _attractRadius = attractRadius;
        _triggerRadius = triggerRadius;
        _attractRadiusRangeByVolume = attractRadiusRangeByVolume;

        IsInitialized = true;
    }

    private void Update()
    {
        if (!IsInitialized)
            return;

        // 定时更新吸引逻辑
        _attractionCheckTimer += Time.deltaTime;
        if (_attractionCheckTimer >= _attractionInterval)
        {
            Attraction();
            _attractionCheckTimer = 0f;
        }

        // 定时检测触发
        _triggerCheckTimer += Time.deltaTime;
        if (_triggerCheckTimer >= _triggerCheckInterval)
        {
            CheckTrigger();
            _triggerCheckTimer = 0f;
        }
    }

    #region 私有方法
    /// <summary>
    /// 吸引逻辑
    /// </summary>
    private void Attraction()
    {
        // 检测吸引范围内的所有动物
        Collider[] animalColliders = Physics.OverlapSphere(
            transform.position,
            _attractRadius,
            LayerMask.GetMask("Animal")
        );

        // 为每个动物选择目标点并设置移动方向
        foreach (Collider collider in animalColliders)
        {
            BaseAnimalBehaviour animal = collider.GetComponent<BaseAnimalBehaviour>();

            // 检查是否已经被吸引过
            if (_attractedAnimals.Contains(animal))
                continue;

            // 进行吸引
            Vector3 targetPoint = GetTargetPointForAnimal(animal);
            Vector3 direction = (targetPoint - animal.transform.position).normalized;
            animal.GetComponent<IMoveable>().SetDirection(direction);

            // 记录已吸引
            _attractedAnimals.Add(animal);
        }
    }

    /// <summary>
    /// 检测触发
    /// </summary>
    private void CheckTrigger()
    {
        // 检测触发范围内的所有动物
        Collider[] animalColliders = Physics.OverlapSphere(
            transform.position,
            _triggerRadius,
            LayerMask.GetMask("Animal")
        );

        // 检查是否有动物进入触发范围
        foreach (Collider collider in animalColliders)
        {
            BaseAnimalBehaviour animal = collider.GetComponent<BaseAnimalBehaviour>();

            // 检查动物是否已死亡
            if (animal.Health.CurrentHealth <= 0)
                continue;

            // 触发陷阱
            TriggerTrap(animal);
            return;
        }
    }

    /// <summary>
    /// 根据动物类型获取目标点
    /// </summary>
    private Vector3 GetTargetPointForAnimal(BaseAnimalBehaviour animal)
    {
        // 根据体型从配置中获取范围比例
        ESpecieType volumeType = animal.SpecieData.SpecieType;
        float[] range = _attractRadiusRangeByVolume[volumeType];

        // 计算动物相对于陷阱的方向角度
        Vector3 directionToAnimal = (animal.transform.position - transform.position);
        directionToAnimal.y = 0f; // 保持水平
        float baseAngle = Mathf.Atan2(directionToAnimal.z, directionToAnimal.x);

        // 在动物进入方向的半圆范围内随机选择角度
        float angle = baseAngle + Random.Range(-90f, 90f) * Mathf.Deg2Rad;

        // 在圆环上随机选择点
        float minRadius = range[0] * _attractRadius;
        float maxRadius = range[1] * _attractRadius;

        return GetRandomPointInRing(angle, minRadius, maxRadius);
    }

    /// <summary>
    /// 在圆环上随机选择点
    /// </summary>
    private Vector3 GetRandomPointInRing(float angle, float minRadius, float maxRadius)
    {
        // 随机距离
        float distance = Random.Range(minRadius, maxRadius);

        // 计算坐标
        Vector3 trapPosition = transform.position;
        Vector3 point = trapPosition + new Vector3(
            Mathf.Cos(angle) * distance,
            0f,
            Mathf.Sin(angle) * distance
        );

        return point;
    }

    /// <summary>
    /// 触发陷阱
    /// </summary>
    private void TriggerTrap(BaseAnimalBehaviour animal)
    {
        // 触发陷阱触发事件
        _eventManager.Trigger(PropEvents.TrapTriggered, new TrapTriggeredEventArgs
        {
            Trap = this,
            TriggeredAnimal = animal,
            TriggerPosition = transform.position
        });

        // 造成伤害
        animal.GetComponent<IDamageable>().TakeDamage(animal.GetComponent<IHealth>().MaxHealth);
    }
    #endregion

    #region 调试可视化
    /// <summary>
    /// 绘制调试信息
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !IsInitialized)
            return;

        Vector3 pos = transform.position;

        // 吸引范围（黄色圈）
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(pos, _attractRadius);

        // 触发范围（红色圈）
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(pos, _triggerRadius);
    }
    #endregion
}
