using UnityEngine;

/// <summary>
/// 动物死亡状态
/// </summary>
public class AnimalDeathState : AnimalState
{
    /// <summary>
    /// 死亡动画持续时间
    /// </summary>
    private float _deathDuration = 7f;

    /// <summary>
    /// 是否已播放死亡特效
    /// </summary>
    private bool _hasPlayedDeathEffect = false;

    public AnimalDeathState(AnimalBehavior animal, StateMachine stateMachine, string animationName) 
        : base(animal, stateMachine, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 设置死亡计时器
        stateTimer = _deathDuration;

        // 禁用移动
        animal.Movement.Disable();

        // 禁用碰撞体
        animal.SetColliderEnabled(false);

        // 触发进入死亡事件
        animal.TriggerAnimalDying();

        // 重置特效播放标志
        _hasPlayedDeathEffect = false;
    }

    public override void Update()
    {
        base.Update();

        // 状态计时器倒计时
        stateTimer -= Time.deltaTime;

        // 死亡第二秒时播放特效
        if (!_hasPlayedDeathEffect && stateTimer <= 5f)
        {
            animal.PlayDeathEffect();
            _hasPlayedDeathEffect = true;
        }

        // 触发死亡事件
        if (stateTimer <= 0f)
            animal.TriggerAnimalDied();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
