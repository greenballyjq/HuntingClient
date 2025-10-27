using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态接口
/// </summary>
public interface IState
{
    /// <summary>
    /// 进入状态时调用
    /// </summary>
    void Enter();

    /// <summary>
    /// 状态更新时调用
    /// </summary>
    void Update();

    /// <summary>
    /// 退出状态时调用
    /// </summary>
    void Exit();
}