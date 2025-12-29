using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物状态基类
/// </summary>
public class AnimalState : IState
{
    /// <summary>
    /// 状态机引用
    /// </summary>
    protected StateMachine stateMachine;

    /// <summary>
    /// 动画名称
    /// </summary>
    protected string animationName;

    /// <summary>
    /// 动物Owner引用
    /// </summary>
    protected AnimalBehavior animal;

    /// <summary>
    /// 状态计时器（子类按需使用）
    /// </summary>
    protected float stateTimer;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AnimalState(AnimalBehavior animal, StateMachine stateMachine, string animationName)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animationName = animationName;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public virtual void Enter()
    {
        animal.PlayAnimation(animationName);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public virtual void Exit()
    {
        animal.StopAnimation(animationName);
    }

    /// <summary>
    /// 状态更新
    /// </summary>
    public virtual void Update()
    {
        // 死亡判断（从任意状态转换）
        if (animal.CurrentHP <= 0 && !(this is AnimalDeathState))
        {
            stateMachine.ChangeState(animal.GetDeathState());
            return;
        }
    }
}