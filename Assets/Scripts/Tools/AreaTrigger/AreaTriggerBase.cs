using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区域触发器基类
/// </summary>
public abstract class AreaTriggerBase : MonoBehaviour
{
    /// <summary>
    /// 触发模式
    /// </summary>
    public enum TriggerMode
    {
        /// <summary>
        /// 区域触发
        /// </summary>
        Inside,
        /// <summary>
        /// 内边界触发（在形状内部，但接近边界）
        /// </summary>
        InnerBorder,
        /// <summary>
        /// 外边界触发（在形状外部，但接近边界）
        /// </summary>
        OuterBorder
    }
    
    /// <summary>
    /// 触发模式
    /// </summary>
    [SerializeField] private TriggerMode _triggerMode = TriggerMode.Inside;
    
    /// <summary>
    /// 目标层遮罩
    /// </summary>
    [SerializeField] private LayerMask _targetLayerMask = -1;
    
    /// <summary>
    /// 检测间隔（秒）
    /// </summary>
    [SerializeField] private float _checkInterval = 0.1f;
    
    /// <summary>
    /// 边界检测容差
    /// </summary>
    [SerializeField] private float _borderTolerance = 0.1f;
    
    /// <summary>
    /// 检测形状
    /// </summary>
    [SerializeField] private AreaShape _shape;

    /// <summary>
    /// 检测计时器
    /// </summary>
    private float _checkTimer;
    
    /// <summary>
    /// 上一帧在区域内的对象ID集合
    /// </summary>
    private HashSet<int> _previousIds = new HashSet<int>();
    
    /// <summary>
    /// 当前帧在区域内的对象ID集合
    /// </summary>
    private HashSet<int> _currentIds = new HashSet<int>();
    
    /// <summary>
    /// ID到碰撞体的映射缓存
    /// </summary>
    private Dictionary<int, Collider> _colliderCache = new Dictionary<int, Collider>();

    /// <summary>
    /// 进入触发
    /// </summary>
    /// <param name="collider">碰撞体</param>
    protected abstract void OnEnter(Collider collider);

    /// <summary>
    /// 停留触发
    /// </summary>
    /// <param name="collider">碰撞体</param>
    protected abstract void OnStay(Collider collider);

    /// <summary>
    /// 离开触发
    /// </summary>
    /// <param name="collider">碰撞体</param>
    protected abstract void OnExit(Collider collider);
    
    private void Update()
    {
        _checkTimer += Time.deltaTime;

        if (_checkTimer < _checkInterval)
            return;

        _checkTimer = 0f;
        
        PerformDetection();
    }
    
    /// <summary>
    /// 执行检测
    /// </summary>
    private void PerformDetection()
    {
        // 清空当前帧集合
        _currentIds.Clear();

        // 区域检测
        Collider[] candidates = Physics.OverlapSphere(
        _shape.GetCenter(),
        _shape.GetBoundingRadius(),
        _targetLayerMask);
        foreach (var collider in candidates)
        {
            if (collider == null) continue;

            Vector3 worldPos = collider.transform.position;
            bool isInside = _shape.IsInside(worldPos);
            bool isOnBorder = _shape.IsOnBorder(worldPos, _borderTolerance);

            bool isInRange = _triggerMode switch
            {
                TriggerMode.Inside => isInside,
                TriggerMode.InnerBorder => isInside && isOnBorder,
                TriggerMode.OuterBorder => !isInside && isOnBorder,
                _ => false
            };

            if (isInRange)
            {
                int id = collider.GetInstanceID();
                _currentIds.Add(id);
                _colliderCache[id] = collider;
            }
        }

        // 触发回调
        ProcessStateChanges();
        
        // 同步状态
        _previousIds.Clear();
        foreach (var id in _currentIds)
            _previousIds.Add(id);
    }
    
    /// <summary>
    /// 处理状态变化并触发回调
    /// </summary>
    private void ProcessStateChanges()
    {
        foreach (var currentId in _currentIds)
        {
            if (!_colliderCache.TryGetValue(currentId, out var collider) || collider == null)
                continue;

            if (!_previousIds.Contains(currentId))
                OnEnter(collider);
            else
                OnStay(collider);
        }

        foreach (var previousId in _previousIds)
        {
            if (_currentIds.Contains(previousId)) continue;

            if (_colliderCache.TryGetValue(previousId, out var collider))
            {
                OnExit(collider);
                _colliderCache.Remove(previousId);
            }
        }
    }
}