using System;
using UnityEngine;

/// <summary>
/// 陷阱动画控制器
/// </summary>
public class TrapAnimator : MonoBehaviour
{
    /// <summary>
    /// 关闭动画触发时回调
    /// </summary>
    public event Action OnCloseAnimationTriggered;

    /// <summary>
    /// 动画控制器
    /// </summary>
    [SerializeField] private Animator _animator;

    /// <summary>
    /// 播放关闭动画
    /// </summary>
    public void PlayClose()
    {
        _animator.SetTrigger("Close");
    }

    /// <summary>
    /// 动画事件回调
    /// </summary>
    public void TrapCloseAnimationTriggered()
    {
        OnCloseAnimationTriggered?.Invoke();
    }
}
