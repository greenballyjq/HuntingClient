using UnityEngine;

/// <summary>
/// 动物死亡状态
/// </summary>
public class AnimalDeathState : AnimalState
{
    /// <summary>
    /// 死亡动画持续时间
    /// </summary>
    private float _deathDuration = 1f;

    public AnimalDeathState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
        : base(animal, stateMachine, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 设置死亡计时器
        stateTimer = _deathDuration;

        animal.RVO.Disable();
        //animal.Movement.SetEnabled(false);

        // 禁用碰撞体
        animal.SetColliderEnabled(false);

        // 触发进入死亡事件
        animal.TriggerAnimalDying();
    }

    public override void Update()
    {
        base.Update();

        // 状态计时器倒计时
        stateTimer -= Time.deltaTime;

        // 触发死亡事件
        if (stateTimer <= 0f)
            animal.TriggerAnimalDied();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
