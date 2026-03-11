using System.Collections.Generic;
using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// 区域触发器 PerformDetection 性能测试
/// </summary>
public class AreaTriggerTest : MonoBehaviour
{
    /// <summary>
    /// 检测形状
    /// </summary>
    [SerializeField] private BaseAreaShape _shape;

    /// <summary>
    /// 目标层遮罩
    /// </summary>
    [SerializeField] private LayerMask _targetLayerMask = -1;

    /// <summary>
    /// 触发模式
    /// </summary>
    [SerializeField] private AreaTriggerBase.TriggerMode _triggerMode = AreaTriggerBase.TriggerMode.Inside;

    /// <summary>
    /// 边界检测容差
    /// </summary>
    [SerializeField] private float _borderTolerance = 0.1f;

    /// <summary>
    /// 执行次数
    /// </summary>
    [SerializeField] private int _iterations = 10000;

    /// <summary>
    /// 触发按键
    /// </summary>
    [SerializeField] private KeyCode _triggerKey = KeyCode.T;

    private readonly HashSet<int> _currentIds = new HashSet<int>();

    #region Unity 生命周期

    private void Update()
    {
        if (!Input.GetKeyDown(_triggerKey)) return;

        using (CodeTimer.Start("PerformDetection", _iterations))
        {
            for (int i = 0; i < _iterations; i++)
            {
                _currentIds.Clear();

                Collider[] candidates = Physics.OverlapSphere(
                    _shape.GetCenter(),
                    _shape.GetBoundingRadius(),
                    _targetLayerMask);

                foreach (var collider in candidates)
                {
                    Vector3 worldPos = collider.transform.position;

                    bool isInRange = _triggerMode switch
                    {
                        AreaTriggerBase.TriggerMode.Inside => _shape.IsInside(worldPos),
                        AreaTriggerBase.TriggerMode.InnerBorder => _shape.IsInside(worldPos) && _shape.IsOnBorder(worldPos, _borderTolerance),
                        AreaTriggerBase.TriggerMode.OuterBorder => _shape.IsOnBorder(worldPos, _borderTolerance) && !_shape.IsInside(worldPos),
                        _ => false
                    };

                    if (isInRange)
                        _currentIds.Add(collider.GetInstanceID());
                }
            }
        }
    }

    #endregion
}
