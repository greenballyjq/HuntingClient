using System.Collections.Generic;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 动物转向区域触发器
/// </summary>
public class AnimalChangeDirectionAreaTrigger : AreaTriggerBase
{
    /// <summary>
    /// 角度范围结构
    /// </summary>
    [System.Serializable]
    public class AngleRange
    {
        /// <summary>
        /// 最小角度
        /// </summary>
        public float MinAngle;

        /// <summary>
        /// 最大角度
        /// </summary>
        public float MaxAngle;

        public AngleRange(float min, float max)
        {
            MinAngle = min;
            MaxAngle = max;
        }
    }

    /// <summary>
    /// 角度范围列表
    /// </summary>
    [SerializeField] private List<AngleRange> _angleRanges = new List<AngleRange>();

    protected override void OnEnter(Collider collider)
    {
        AngleRange selectedRange = _angleRanges[Random.Range(0, _angleRanges.Count)];
        float randomAngle = Random.Range(selectedRange.MinAngle, selectedRange.MaxAngle);

        // 计算新方向
        Vector3 newDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * transform.forward;

        // 设置移动方向
        collider.GetComponent<IMoveable>().SetDirection(newDirection);

        
    }

    protected override void OnStay(Collider collider) { }

    protected override void OnExit(Collider collider) { }

#if UNITY_EDITOR
    /// <summary>
    /// 绘制角度范围
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_angleRanges == null || _angleRanges.Count == 0)
            return;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // 绘制forward方向
        Gizmos.color = Color.white;
        Gizmos.DrawRay(origin, forward * 3);

        // 绘制每个角度范围
        foreach (var range in _angleRanges)
            DrawAngleRange(origin, forward, range.MinAngle, range.MaxAngle);
    }

    /// <summary>
    /// 绘制单个角度范围扇形
    /// </summary>
    private void DrawAngleRange(Vector3 origin, Vector3 forward, float minAngle, float maxAngle)
    {
        Gizmos.color = Color.cyan;

        // 绘制边界线
        Vector3 minDir = Quaternion.AngleAxis(minAngle, Vector3.up) * forward;
        Vector3 maxDir = Quaternion.AngleAxis(maxAngle, Vector3.up) * forward;

        Gizmos.DrawRay(origin, minDir * 3);
        Gizmos.DrawRay(origin, maxDir * 3);

        // 绘制扇形弧线
        int segments = 20;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(minAngle, maxAngle, t);
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;

            if (i > 0)
            {
                float prevAngle = Mathf.Lerp(minAngle, maxAngle, (float)(i - 1) / segments);
                Vector3 prevDir = Quaternion.AngleAxis(prevAngle, Vector3.up) * forward;
                Gizmos.DrawLine(origin + prevDir * 3, origin + dir * 3);
            }
        }
    }
#endif
}

