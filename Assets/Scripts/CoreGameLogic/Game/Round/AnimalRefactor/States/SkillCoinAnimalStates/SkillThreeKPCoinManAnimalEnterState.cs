using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 技能三千盘金币人入场状态
/// </summary>
public class SkillThreeKPCoinManAnimalEnterState : AnimalEnterState
{
    /// <summary>
    /// 入场移动速率
    /// </summary>
    protected float _enterMoveSpeedRate = 4f;

    public SkillThreeKPCoinManAnimalEnterState(StateMachine stateMachine, BaseAnimalBehaviour animalBehavior) : base(stateMachine, animalBehavior)
    {
            
    }

    public override void Enter()
    {
        base.Enter();

        animalBehavior.Moveable.SetMoveRate(_enterMoveSpeedRate);

        (animalBehavior.AnimalVisual as SkillThreeKPCoinManAnimalVisual).PlayEnter();
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);

        animalBehavior.Moveable.DoUpdate(dt);

        // 检查是否接近目标位置
        Vector3 currentPos = animalBehavior.transform.position;
        Vector3 targetPos = animalBehavior.Moveable.CurrentTargetPosition;

        if (Vector3.Distance(currentPos, targetPos) <= 0.2f)
        {
            // 随机XZ面方向
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 randomDirection = new Vector3(
                Mathf.Cos(randomAngle),
                0f,
                Mathf.Sin(randomAngle)
            ).normalized;

            // 设置移动方向
            animalBehavior.Moveable.SetDirection(randomDirection);

            // 切换到移动状态
            ChangeState(animalBehavior.MoveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
