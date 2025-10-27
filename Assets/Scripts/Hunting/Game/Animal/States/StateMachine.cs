using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机
/// </summary>
public class StateMachine
{
    /// <summary>
    /// 当前状态
    /// </summary>
    public IState currentState { get; private set; }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    /// <param name="startState"></param>
    public void Initialize(IState startState)
    {
        currentState = startState;
        startState.Enter();
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    /// <summary>
    /// 更新当前状态
    /// </summary>
    public void Update()
    {
        currentState.Update();
    }
}
