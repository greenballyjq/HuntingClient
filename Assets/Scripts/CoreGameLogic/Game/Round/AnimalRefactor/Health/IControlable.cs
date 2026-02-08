using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可被控制接口
/// </summary>
public interface IControlable
{
    /// <summary>
    /// 被控制
    /// </summary>
    void TakeControl();
}
