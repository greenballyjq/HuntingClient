using UnityEngine;

/// <summary>
/// 调试区域触发器 - 仅输出文本日志用于测试
/// </summary>
public class DebugAreaTrigger : AreaTriggerBase
{
    /// <summary>
    /// 是否输出进入日志
    /// </summary>
    [SerializeField] private bool _logEnter = true;
    
    /// <summary>
    /// 是否输出停留日志
    /// </summary>
    [SerializeField] private bool _logStay = false;
    
    /// <summary>
    /// 是否输出离开日志
    /// </summary>
    [SerializeField] private bool _logExit = true;
    
    #region AreaTriggerBase 实现
    /// <summary>
    /// 进入时调用
    /// </summary>
    protected override void OnEnter(Collider collider)
    {
        if (collider == null)
            return;
            
        if (_logEnter)
        {
            Debug.Log($"[DebugAreaTrigger] {collider.name} (ID: {collider.GetInstanceID()}) 进入区域");
        }
    }
    
    /// <summary>
    /// 停留时调用
    /// </summary>
    protected override void OnStay(Collider collider)
    {
        if (collider == null)
            return;
            
        if (_logStay)
        {
            Debug.Log($"[DebugAreaTrigger] {collider.name} (ID: {collider.GetInstanceID()}) 停留在区域");
        }
    }
    
    /// <summary>
    /// 离开时调用
    /// </summary>
    protected override void OnExit(Collider collider)
    {
        if (collider == null)
            return;
            
        if (_logExit)
        {
            Debug.Log($"[DebugAreaTrigger] {collider.name} (ID: {collider.GetInstanceID()}) 离开区域");
        }
    }
    #endregion
}

